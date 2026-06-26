using System.Collections.Generic;
using Core.Health;
using TowerDefense.Affectors;
using TowerDefense.Towers.Data;
using TowerDefense.UI.HUD;
using UnityEngine;

namespace TowerDefense.Towers
{
	/// <summary>
	/// タワーの個別レベル
	/// </summary>
	[DisallowMultipleComponent]
	public class TowerLevel : MonoBehaviour, ISerializationCallbackReceiver
	{
		/// <summary>
		/// シーン上で配置位置を伝えるためのプレハブ
		/// </summary>
		public TowerPlacementGhost towerGhostPrefab;

		/// <summary>
		/// 開始時に生成する建築エフェクトの GameObject
		/// </summary>
		public GameObject buildEffectPrefab;

		/// <summary>
		/// レベルデータを持つ ScriptableObject への参照
		/// </summary>
		public  TowerLevelData levelData;

		/// <summary>
		/// このタワーの親タワーコントローラー
		/// </summary>
		protected Tower m_ParentTower;

		/// <summary>
		/// タワーにアタッチされているエフェクトのリスト
		/// </summary>
		Affector[] m_Affectors;

		/// <summary>
		/// タワーにアタッチされているエフェクトのリストを取得します
		/// </summary>
		protected Affector[] Affectors
		{
			get
			{
				if (m_Affectors == null)
				{
					m_Affectors = GetComponentsInChildren<Affector>();
				}
				return m_Affectors;
			}
		}

		/// <summary>
		/// タワーが探索に使う物理レイヤーマスク
		/// </summary>
		public LayerMask mask { get; protected set; }

		/// <summary>
		/// コスト値を取得します
		/// </summary>
		public int cost
		{
			get { return levelData.cost; }
		}

		/// <summary>
		/// 売却値を取得します
		/// </summary>
		public int sell
		{
			get { return levelData.sell; }
		}

		/// <summary>
		/// 最大体力を取得します
		/// </summary>
		public int maxHealth
		{
			get { return levelData.maxHealth; }
		}

		/// <summary>
		/// 開始時の体力を取得します
		/// </summary>
		public int startingHealth
		{
			get { return levelData.startingHealth; }
		}

		/// <summary>
		/// タワーの説明を取得します
		/// </summary>
		public string description
		{
			get { return levelData.description; }
		}

		/// <summary>
		/// タワーのアップグレード説明を取得します
		/// </summary>
		public string upgradeDescription
		{
			get { return levelData.upgradeDescription; }
		}

		/// <summary>
		/// このオブジェクトにアタッチされているエフェクトを初期化します
		/// </summary>
		public virtual void Initialize(Tower tower, LayerMask enemyMask, IAlignmentProvider alignment)
		{
			mask = enemyMask;
			
			foreach (Affector effect in Affectors)
			{
				effect.Initialize(alignment, mask);
			}
			m_ParentTower = tower;
		}

		/// <summary>
		/// アタッチされている <see cref="Affectors"/> を有効化または無効化するメソッド
		/// </summary>
		public void SetAffectorState(bool state)
		{
			foreach (Affector affector in Affectors)
			{
				if (affector != null)
				{
					affector.enabled = state;
				}
			}
		}

		/// <summary>
		/// ITowerRadiusVisualizer を実装している Affector のリストを返します
		/// </summary>
		/// <returns>タワーの ITowerRadiusVisualizer</returns>
		public List<ITowerRadiusProvider> GetRadiusVisualizers()
		{
			List<ITowerRadiusProvider> visualizers = new List<ITowerRadiusProvider>();
			foreach (Affector affector in Affectors)
			{
				var visualizer = affector as ITowerRadiusProvider;
				if (visualizer != null)
				{
					visualizers.Add(visualizer);
				}
			}
			return visualizers;
		}

		/// <summary>
		/// タワーの DPS を返します
		/// </summary>
		/// <returns>タワーの DPS</returns>
		public float GetTowerDps()
		{
			float dps = 0;
			foreach (Affector affector in Affectors)
			{
				var attack = affector as AttackAffector;
				if (attack != null && attack.damagerProjectile != null)
				{
					dps += attack.GetProjectileDamage() * attack.fireRate;
				}
			}
			return dps;
		}

		public void Kill()
		{
			m_ParentTower.KillTower();
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			// この値はエディター上で実行後も残るプレハブに設定されるため、このメンバーを null に戻す必要があります
			// 毎回確実に再設定されるように、このメンバーを null にします
			m_Affectors = null;
		}

		/// <summary>
		/// 建築用パーティクルエフェクトのオブジェクトを生成します
		/// </summary>
		void Start()
		{
			if (buildEffectPrefab != null)
			{
				Instantiate(buildEffectPrefab, transform);
			}
		}
	}
}
