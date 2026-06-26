using System;
using ActionGameFramework.Health;
using Core.Utilities;
using TowerDefense.Level;
using TowerDefense.Towers.Placement;
using TowerDefense.UI.HUD;
using UnityEngine;

namespace TowerDefense.Towers
{
	/// <summary>
	/// すべてのTower種別で共通する機能
	/// </summary>
	public class Tower : Targetable
	{
		/// <summary>
		/// このTowerに関連付けられたTowerレベル
		/// </summary>
		public TowerLevel[] levels;

		/// <summary>
		/// 各レベルで共通して使う汎用名
		/// </summary>
		public string towerName;

		/// <summary>
		/// Towerの占有サイズ
		/// </summary>
		public IntVector2 dimensions;

		/// <summary>
		/// Towerが検索に使うPhysics Mask
		/// </summary>
		public LayerMask enemyLayerMask;

		/// <summary>
		/// Towerの現在レベル
		/// </summary>
		public int currentLevel { get; protected set; }

		/// <summary>
		/// 現在レベルのデータへの参照
		/// </summary>
		public TowerLevel currentTowerLevel { get; protected set; }

		/// <summary>
		/// Towerがこれ以上レベルアップできるかを取得します
		/// </summary>
		public bool isAtMaxLevel
		{
			get { return currentLevel == levels.Length - 1; }
		}

		/// <summary>
		/// 最初のレベルのTower Ghost Prefabを取得します
		/// </summary>
		public TowerPlacementGhost towerGhostPrefab
		{
			get { return levels[currentLevel].towerGhostPrefab; }
		}

		/// <summary>
		/// <see cref="placementArea"/>上でのこのTowerのグリッド位置を取得します
		/// </summary>
		public IntVector2 gridPosition { get; private set; }

		/// <summary>
		/// このTowerが建てられている配置エリア
		/// </summary>
		public IPlacementArea placementArea { get; private set; }

		/// <summary>
		/// Towerの購入コスト
		/// </summary>
		public int purchaseCost
		{
			get { return levels[0].cost; }
		}

		/// <summary>
		/// プレイヤーがTowerを削除したときに発火するイベント
		/// </summary>
		public Action towerDeleted;

		/// <summary>
		/// Towerが破壊されたときに発火するイベント
		/// </summary>
		public Action towerDestroyed;

		/// <summary>
		/// 初期化に使うデータをTowerへ渡します
		/// </summary>
		/// <param name="targetArea">配置エリアの設定</param>
		/// <param name="destination">目的地の位置</param>
		public virtual void Initialize(IPlacementArea targetArea, IntVector2 destination)
		{
			placementArea = targetArea;
			gridPosition = destination;

			if (targetArea != null)
			{
				transform.position = placementArea.GridToWorld(destination, dimensions);
				transform.rotation = placementArea.transform.rotation;
				targetArea.Occupy(destination, dimensions);
			}

			SetLevel(0);
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.levelStateChanged += OnLevelStateChanged;
			}
		}

		/// <summary>
		/// アップグレードコストの情報を返します
		/// </summary>
		/// <returns>Towerがすでに最大レベルなら-1、それ以外はアップグレードコストを返します</returns>
		public int GetCostForNextLevel()
		{
			if (isAtMaxLevel)
			{
				return -1;
			}
			return levels[currentLevel + 1].cost;
		}

		/// <summary>
		/// このTowerを破壊します
		/// </summary>
		public void KillTower()
		{
			// 基底のKillメソッドを呼び出します
			Kill();
		}

		/// <summary>
		/// このTowerを売却したときに得られる値を返します
		/// </summary>
		/// <returns>Towerの売却値</returns>
		public int GetSellLevel()
		{
			return GetSellLevel(currentLevel);
		}

		/// <summary>
		/// 指定レベルのこのTowerを売却したときに得られる値を返します
		/// </summary>
		/// <param name="level">Towerのレベル</param>
		/// <returns>Towerの売却値</returns>
		public int GetSellLevel(int level)
		{
			// Wave開始前なら全額で売却します
			if (LevelManager.instance.levelState == LevelState.Building)
			{
				int cost = 0;
				for (int i = 0; i <= level; i++)
				{
					cost += levels[i].cost;
				}

				return cost;
			}
			return levels[currentLevel].sell;
		}

		/// <summary>
		/// Towerデータのアップグレードを試みるために使います
		/// </summary>
		public virtual bool UpgradeTower()
		{
			if (isAtMaxLevel)
			{
				return false;
			}
			SetLevel(currentLevel + 1);
			return true;
		}

		/// <summary>
		/// Towerをダウングレードするメソッド
		/// </summary>
		/// <returns>
		/// Towerが最低レベルの場合は<value>false</value>
		/// </returns>
		public virtual bool DowngradeTower()
		{
			if (currentLevel == 0)
			{
				return false;
			}
			SetLevel(currentLevel - 1);
			return true;
		}

		/// <summary>
		/// Towerを任意の有効なレベルに設定するために使います
		/// </summary>
		/// <param name="level">
		/// Towerをアップグレードする先のレベル
		/// </param>
		/// <returns>
		/// 成功した場合はtrue
		/// </returns>
		public virtual bool UpgradeTowerToLevel(int level)
		{
			if (level < 0 || isAtMaxLevel || level >= levels.Length)
			{
				return false;
			}
			SetLevel(level);
			return true;
		}

		public void Sell()
		{
			Remove();
		}

		/// <summary>
		/// Towerを配置エリアから取り除いて破棄します
		/// </summary>
		public override void Remove()
		{
			base.Remove();
			
			placementArea.Clear(gridPosition, dimensions);
			Destroy(gameObject);
		}

		/// <summary>
		/// 必要に応じて購読解除します
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.levelStateChanged += OnLevelStateChanged;
			}
		}

		/// <summary>
		/// よく使うデータをキャッシュして更新します
		/// </summary>
		protected void SetLevel(int level)
		{
			if (level < 0 || level >= levels.Length)
			{
				return;
			}
			currentLevel = level;
			if (currentTowerLevel != null)
			{
				Destroy(currentTowerLevel.gameObject);
			}

			// 表示用オブジェクトを生成します
			currentTowerLevel = Instantiate(levels[currentLevel], transform);

			// TowerLevelを初期化します
			currentTowerLevel.Initialize(this, enemyLayerMask, configuration.alignmentProvider);

			// 体力データ
			ScaleHealth();

			// Affectorを無効化します
			LevelState levelState = LevelManager.instance.levelState;
			bool initialise = levelState == LevelState.AllEnemiesSpawned || levelState == LevelState.SpawningEnemies;
			currentTowerLevel.SetAffectorState(initialise);
		}

		/// <summary>
		/// 以前の体力を基準に体力をスケールします
		/// アップグレード時の体力スケール規則が変わる場合はoverrideが必要です
		/// </summary>
		protected virtual void ScaleHealth()
		{
			configuration.SetMaxHealth(currentTowerLevel.maxHealth);
			
			if (currentLevel == 0)
			{
				configuration.SetHealth(currentTowerLevel.maxHealth);
			}
			else
			{
				int currentHealth = Mathf.FloorToInt(configuration.normalisedHealth * currentTowerLevel.maxHealth);
				configuration.SetHealth(currentHealth);
			}
		}

		/// <summary>
		/// レベル状態に応じてAffectorを初期化します
		/// </summary>
		protected virtual void OnLevelStateChanged(LevelState previous, LevelState current)
		{
			bool initialise = current == LevelState.AllEnemiesSpawned || current == LevelState.SpawningEnemies;
			currentTowerLevel.SetAffectorState(initialise);
		}
	}
}