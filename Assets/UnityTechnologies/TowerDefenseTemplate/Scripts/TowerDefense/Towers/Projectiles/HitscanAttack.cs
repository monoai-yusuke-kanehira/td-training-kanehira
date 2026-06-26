using ActionGameFramework.Health;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Towers.Projectiles
{
	/// <summary>
	/// Hitscan Projectileの実装
	/// この武器は敵を即座に攻撃する仕組みです
	/// </summary>
	[RequireComponent(typeof(Damager))]
	public class HitscanAttack : MonoBehaviour
	{
		/// <summary>
		/// 遅延させる時間
		/// </summary>
		public float delay;

		/// <summary>
		/// 遅延タイマー
		/// </summary>
		protected Timer m_Timer;

		/// <summary>
		/// このProjectileが攻撃する敵
		/// </summary>
		protected Targetable m_Enemy;

		/// <summary>
		/// オブジェクトにアタッチされたDamager
		/// </summary>
		protected Damager m_Damager;

		/// <summary>
		/// TowerのProjectile位置
		/// </summary>
		protected Vector3 m_Origin;

		/// <summary>
		/// 遅延タイマーを一時停止するための設定
		/// Time.timeScaleを0にせずに行います
		/// </summary>
		protected bool m_PauseTimer;

		/// <summary>
		/// 攻撃用の遅延設定
		/// </summary>
		/// <param name="origin">
		/// 攻撃の発射元となる点
		/// </param>
		/// <param name="enemy">
		/// 攻撃する敵
		/// </param>
		public void AttackEnemy(Vector3 origin, Targetable enemy)
		{
			m_Enemy = enemy;
			m_Origin = origin;
			m_Timer.Reset();
			m_PauseTimer = false;
		}

		/// <summary>
		/// Hitscan攻撃の実際の攻撃処理。
		/// 攻撃する敵がいない場合はメソッドから早期returnします。
		/// </summary>
		protected void DealDamage()
		{
			Poolable.TryPool(gameObject);

			if (m_Enemy == null)
			{
				return;
			}
			
			// エフェクト
			ParticleSystem pfxPrefab = m_Damager.collisionParticles;
			var attackEffect = Poolable.TryGetPoolable<ParticleSystem>(pfxPrefab.gameObject);
			attackEffect.transform.position = m_Enemy.position;
			attackEffect.Play();
			
			m_Enemy.TakeDamage(m_Damager.damage, m_Enemy.position, m_Damager.alignmentProvider);
			m_PauseTimer = true;
		}

		/// <summary>
		/// このオブジェクトにアタッチされたDamagerコンポーネントをキャッシュします
		/// </summary>
		protected virtual void Awake()
		{
			m_Damager = GetComponent<Damager>();
			m_Timer = new Timer(delay, DealDamage);
		}

		/// <summary>
		/// m_Timerが利用可能な場合は更新します
		/// </summary>
		protected virtual void Update()
		{
			if (!m_PauseTimer)
			{
				m_Timer.Tick(Time.deltaTime);
			}
		}
	}
}