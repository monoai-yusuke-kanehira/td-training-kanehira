using System;
using Core.Economy;
using Core.Health;
using Core.Utilities;
using TowerDefense.Economy;
using TowerDefense.Towers.Data;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// レベルマネージャー - レベルの状態を処理し、プレーヤーの通貨を追跡します
	/// </summary>
	[RequireComponent(typeof(WaveManager))]
	public class LevelManager : Singleton<LevelManager>
	{
		/// <summary>
		/// 設定されたレベルのイントロ。これが null の場合、LevelManager はゲームプレイ状態 (つまり SpawningEnemies) に移行します
		/// </summary>
		public LevelIntro intro;

		/// <summary>
		/// このレベルのタワーライブラリー
		/// </summary>
		public TowerLibrary towerLibrary;

		/// <summary>
		/// プレイヤーが開始する通貨
		/// </summary>
		public int startingCurrency;

		/// <summary>
		/// 通貨を獲得するためのコントローラー
		/// </summary>
		public CurrencyGainer currencyGainer;

		/// <summary>
		/// プレビルドフェーズでもプレイヤーが通貨を獲得する場合の設定
		/// </summary>
		[Header("Setting this will allow currency gain during the Intro and Pre-Build phase")]
		public bool alwaysGainCurrency;

		/// <summary>
		/// プレイヤーが守らなければならない本拠地
		/// </summary>
		public PlayerHomeBase[] homeBases;

		public Collider[] environmentColliders;

		/// <summary>
		/// 付属のウェーブマネージャー
		/// </summary>
		public WaveManager waveManager { get; protected set; }

		/// <summary>
		/// 現在レベル内の敵の数
		/// </summary>
		public int numberOfEnemies { get; protected set; }

		/// <summary>
		/// レベルの現在の状態
		/// </summary>
		public LevelState levelState { get; protected set; }

		/// <summary>
		/// 通貨管理者
		/// </summary>
		public Currency currency { get; protected set; }

		/// <summary>
		/// 残り本塁数
		/// </summary>
		public int numberOfHomeBasesLeft { get; protected set; }

		/// <summary>
		/// 開始本塁数
		/// </summary>
		public int numberOfHomeBases { get; protected set; }

		/// <summary>
		/// ホームベース用アクセサー
		/// </summary>
		public PlayerHomeBase[] playerHomeBases
		{
			get { return homeBases; }
		}

		/// <summary>
		/// ゲームオーバーの場合
		/// </summary>
		public bool isGameOver
		{
			get { return (levelState == LevelState.Win) || (levelState == LevelState.Lose); }
		}

		/// <summary>
		/// すべてのウェーブが終了し、敵がいなくなったときに発射されます
		/// </summary>
		public event Action levelCompleted;

		/// <summary>
		/// 本拠地がすべて破壊された場合に発射
		/// </summary>
		public event Action levelFailed;

		/// <summary>
		/// レベルの状態が変更されたときに発生します - 最初のパラメータは古い状態、2 番目のパラメータは新しい状態です
		/// </summary>
		public event Action<LevelState, LevelState> levelStateChanged;

		/// <summary>
		/// 敵の数が変化したときに発射
		/// </summary>
		public event Action<int> numberOfEnemiesChanged;

		/// <summary>
		/// 本拠地破壊イベント
		/// </summary>
		public event Action homeBaseDestroyed;

		/// <summary>
		/// 敵の数を増やします。エージェントの生成時に呼び出されます
		/// </summary>
		public virtual void IncrementNumberOfEnemies()
		{
			numberOfEnemies++;
			SafelyCallNumberOfEnemiesChanged();
		}

		/// <summary>
		/// すべてのホームベースの健全性の合計を返します
		/// </summary>
		public float GetAllHomeBasesHealth()
		{
			float health = 0.0f;
			foreach (PlayerHomeBase homebase in homeBases)
			{
				health += homebase.configuration.currentHealth;
			}
			return health;
		}

		/// <summary>
		/// 敵の数を減らします。エージェントの死亡時に呼び出される
		/// </summary>
		public virtual void DecrementNumberOfEnemies()
		{
			numberOfEnemies--;
			SafelyCallNumberOfEnemiesChanged();
			if (numberOfEnemies < 0)
			{
				Debug.LogError("[LEVEL] There should never be a negative number of enemies. Something broke!");
				numberOfEnemies = 0;
			}

			if (numberOfEnemies == 0 && levelState == LevelState.AllEnemiesSpawned)
			{
				ChangeLevelState(LevelState.Win);
			}
		}

		/// <summary>
		/// 構築フェーズを完了し、敵を出現させる状態を設定します
		/// </summary>
		public virtual void BuildingCompleted()
		{
			ChangeLevelState(LevelState.SpawningEnemies);
		}

		/// <summary>
		/// アタッチされた Wave Manager をキャッシュし、生成完了イベントをサブスクライブします
		/// レベルの状態をイントロに設定し、敵の数が確実に 0 に設定されるようにします
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			waveManager = GetComponent<WaveManager>();
			waveManager.spawningCompleted += OnSpawningCompleted;

			// このデフォルト値のイベントをブロードキャストする必要がないため、状態変更関数は使用されません
			levelState = LevelState.Intro;
			numberOfEnemies = 0;

			// 通貨変更リスナーが割り当てられていることを確認する
			currency = new Currency(startingCurrency);
			currencyGainer.Initialize(currency);

			// イントロがある場合はそれを使用し、そうでない場合はゲームプレイに落ちます
			if (intro != null)
			{
				intro.introCompleted += IntroCompleted;
			}
			else
			{
				IntroCompleted();
			}

			// ホームベースを順に処理して購読します
			numberOfHomeBases = homeBases.Length;
			numberOfHomeBasesLeft = numberOfHomeBases;
			for (int i = 0; i < numberOfHomeBases; i++)
			{
				homeBases[i].died += OnHomeBaseDestroyed;
			}
		}

		/// <summary>
		/// 通貨ゲインコントローラーを更新します
		/// </summary>
		protected virtual void Update()
		{
			if (alwaysGainCurrency ||
			    (!alwaysGainCurrency && levelState != LevelState.Building && levelState != LevelState.Intro))
			{
				currencyGainer.Tick(Time.deltaTime);
			}
		}

		/// <summary>
		/// イベントの購読を解除する
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (waveManager != null)
			{
				waveManager.spawningCompleted -= OnSpawningCompleted;
			}
			if (intro != null)
			{
				intro.introCompleted -= IntroCompleted;
			}

			// ホームベースを順に処理して購読を解除します
			for (int i = 0; i < numberOfHomeBases; i++)
			{
				homeBases[i].died -= OnHomeBaseDestroyed;
			}
		}

		/// <summary>
		/// イントロが完了したとき、またはイントロが指定されていない場合はすぐに起動されます
		/// </summary>
		protected virtual void IntroCompleted()
		{
			ChangeLevelState(LevelState.Building);
		}

		/// <summary>
		/// WaveManager が敵の生成を終了したときに発生します
		/// </summary>
		protected virtual void OnSpawningCompleted()
		{
			ChangeLevelState(LevelState.AllEnemiesSpawned);
		}

		/// <summary>
		/// 状態を変更し、イベントをブロードキャストします
		/// </summary>
		/// <param name="newState">移行する新しい状態</param>
		protected virtual void ChangeLevelState(LevelState newState)
		{
			// 状態が変わっていない場合は戻ります
			if (levelState == newState)
			{
				return;
			}

			LevelState oldState = levelState;
			levelState = newState;
			if (levelStateChanged != null)
			{
				levelStateChanged(oldState, newState);
			}
			
			switch (newState)
			{
				case LevelState.SpawningEnemies:
					waveManager.StartWaves();
					break;
				case LevelState.AllEnemiesSpawned:
					// 敵が全員すでに死んでいる場合は即座に勝利します
					if (numberOfEnemies == 0)
					{
						ChangeLevelState(LevelState.Win);
					}
					break;
				case LevelState.Lose:
					SafelyCallLevelFailed();
					break;
				case LevelState.Win:
					SafelyCallLevelCompleted();
					break;
			}
		}

		/// <summary>
		/// 本拠地破壊時に発射
		/// </summary>
		protected virtual void OnHomeBaseDestroyed(DamageableBehaviour homeBase)
		{
			// ホームベースの数を減らす
			numberOfHomeBasesLeft--;

			// 破棄されたイベントを呼び出す
			if (homeBaseDestroyed != null)
			{
				homeBaseDestroyed();
			}

			// ホームベースが残っておらず、レベルが終了していない場合は、レベルをロストに設定します
			if ((numberOfHomeBasesLeft == 0) && !isGameOver)
			{
				ChangeLevelState(LevelState.Lose);
			}
		}

		/// <summary>
		/// を呼び出します <see cref="levelCompleted"/> イベント
		/// </summary>
		protected virtual void SafelyCallLevelCompleted()
		{
			if (levelCompleted != null)
			{
				levelCompleted();
			}
		}

		/// <summary>
		/// を呼び出します <see cref="numberOfEnemiesChanged"/> イベント
		/// </summary>
		protected virtual void SafelyCallNumberOfEnemiesChanged()
		{
			if (numberOfEnemiesChanged != null)
			{
				numberOfEnemiesChanged(numberOfEnemies);
			}
		}

		/// <summary>
		/// を呼び出します <see cref="levelFailed"/> イベント
		/// </summary>
		protected virtual void SafelyCallLevelFailed()
		{
			if (levelFailed != null)
			{
				levelFailed();
			}
		}
	}
}
