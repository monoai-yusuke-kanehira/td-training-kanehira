using System.Collections.Generic;
using ActionGameFramework.Audio;
using ActionGameFramework.Health;
using Core.Health;
using TowerDefense.Targetting;
using TowerDefense.Towers;
using TowerDefense.Towers.Projectiles;
using UnityEngine;

namespace TowerDefense.Affectors
{
	/// <summary>
	/// 攻撃用の弾を発射する処理を扱う共通効果
	/// 
	/// ILauncherが必要だが、自動では追加されない
	/// このスクリプトを追加する前に、このGameObjectへILauncherの実装を追加する
	/// </summary>
	[RequireComponent(typeof(ILauncher))]
	public class AttackAffector : Affector, ITowerRadiusProvider
	{
		/// <summary>
		/// 攻撃に使う弾
		/// </summary>
		public GameObject projectile;

		/// <summary>
		/// 弾を発射する位置のリスト
		/// </summary>
		public Transform[] projectilePoints;

		/// <summary>
		/// タワーが検索を開始する中心点への参照
		/// </summary>
		public Transform epicenter;

		/// <summary>
		/// タワーが範囲ダメージを与える場合の設定
		/// </summary>
		public bool isMultiAttack;


		/// <summary>
		/// 1秒あたりの発射回数
		/// </summary>
		public float fireRate;

		/// <summary>
		/// 発射時に再生するAudioSource
		/// </summary>
		public RandomAudioSource randomAudioSource;

		/// <summary>
		/// Targetterを取得する
		/// </summary>
		public Targetter towerTargetter;

		/// <summary>
		/// 効果範囲を可視化するときの色
		/// </summary>
		public Color radiusEffectColor;

		/// <summary>
		/// 検索条件
		/// </summary>
		public Filter searchCondition;

		/// <summary>
		/// 発射条件
		/// </summary>
		public Filter fireCondition;

		/// <summary>
		/// アタッチされているLauncherへの参照
		/// </summary>
		protected ILauncher m_Launcher;

		/// <summary>
		/// 発射可能になるまでの時間
		/// </summary>
		protected float m_FireTimer;

		/// <summary>
		/// 現在追跡している敵への参照
		/// </summary>
		protected Targetable m_TrackingEnemy;

		/// <summary>
		/// Targetterから検索頻度を取得する
		/// </summary>
		public float searchRate
		{
			get { return towerTargetter.searchRate; }
			set { towerTargetter.searchRate = value; }
		}

		/// <summary>
		/// Targetableを取得する
		/// </summary>
		public Targetable trackingEnemy
		{
			get { return m_TrackingEnemy; }
		}

		/// <summary>
		/// 攻撃範囲を取得または設定する
		/// </summary>
		public float effectRadius
		{
			get { return towerTargetter.effectRadius; }
		}

		public Color effectColor 
		{
			get { return radiusEffectColor; }
		}

		public Targetter targetter 
		{
			get { return towerTargetter; }
		}

		/// <summary>
		/// 攻撃Affectorを初期化する
		/// </summary>
		public override void Initialize(IAlignmentProvider affectorAlignment)
		{
			Initialize(affectorAlignment, -1);
		}

		/// <summary>
		/// レイヤーマスクを使って攻撃Affectorを初期化する
		/// </summary>
		public override void Initialize(IAlignmentProvider affectorAlignment, LayerMask mask)
		{
			base.Initialize(affectorAlignment, mask);
			SetUpTimers();

			towerTargetter.ResetTargetter();
			towerTargetter.alignment = affectorAlignment;
			towerTargetter.acquiredTarget += OnAcquiredTarget;
			towerTargetter.lostTarget += OnLostTarget;
		}

		void OnDestroy()
		{
			towerTargetter.acquiredTarget -= OnAcquiredTarget;
			towerTargetter.lostTarget -= OnLostTarget;
		}

		void OnLostTarget()
		{
			m_TrackingEnemy = null;
		}

		void OnAcquiredTarget(Targetable acquiredTarget)
		{
			m_TrackingEnemy = acquiredTarget;
		}

		public Damager damagerProjectile
		{
			get { return projectile == null ? null : projectile.GetComponent<Damager>(); }
		}

		/// <summary>
		/// 弾の合計ダメージを返す
		/// </summary>
		public float GetProjectileDamage()
		{
			var splash = projectile.GetComponent<SplashDamager>();
			float splashDamage = splash != null ? splash.damage : 0;
			return damagerProjectile.damage + splashDamage;
		}

		/// <summary>
		/// RepeatingTimerを初期化する
		/// </summary>
		protected virtual void SetUpTimers()
		{
			m_FireTimer = 1 / fireRate;
			m_Launcher = GetComponent<ILauncher>();
		}

		/// <summary>
		/// タイマーを更新する
		/// </summary>
		protected virtual void Update()
		{
			m_FireTimer -= Time.deltaTime;
			if (trackingEnemy != null && m_FireTimer <= 0.0f)
			{
				OnFireTimer();
				m_FireTimer = 1 / fireRate;
			}
		}

		/// <summary>
		/// 発射間隔タイマーの各ポーリング時に呼ばれる
		/// </summary>
		protected virtual void OnFireTimer()
		{
			if (fireCondition != null)
			{
				if (!fireCondition())
				{
					return;
				}
			}
			FireProjectile();
		}

		/// <summary>
		/// 攻撃時の共通処理
		/// </summary>
		protected virtual void FireProjectile()
		{
			if (m_TrackingEnemy == null)
			{
				return;
			}

			if (isMultiAttack)
			{
				List<Targetable> enemies = towerTargetter.GetAllTargets();
				m_Launcher.Launch(enemies, projectile, projectilePoints);
			}
			else
			{
				m_Launcher.Launch(m_TrackingEnemy, damagerProjectile.gameObject, projectilePoints);
			}
			if (randomAudioSource != null)
			{
				randomAudioSource.PlayRandomClip();
			}
		}

		/// <summary>
		/// コンポーネント間の距離を比較するdelegate
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		protected virtual int ByDistance(Targetable first, Targetable second)
		{
			float firstSqrMagnitude = Vector3.SqrMagnitude(first.position - epicenter.position);
			float secondSqrMagnitude = Vector3.SqrMagnitude(second.position - epicenter.position);
			return firstSqrMagnitude.CompareTo(secondSqrMagnitude);
		}

#if UNITY_EDITOR
		/// <summary>
		/// 検索範囲を描画する
		/// </summary>
		void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(epicenter.position, towerTargetter.effectRadius);
		}
#endif
	}

	/// <summary>
	/// bool値を計算するロジック用のdelegate
	/// </summary>
	public delegate bool Filter();
}
