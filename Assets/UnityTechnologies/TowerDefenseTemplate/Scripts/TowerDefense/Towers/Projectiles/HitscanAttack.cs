using ActionGameFramework.Health;
using Core.Utilities;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TowerDefense.Towers.Projectiles
{
	[RequireComponent(typeof(Damager))]
	public class HitscanAttack : MonoBehaviour
	{
		public float delay;
		protected Targetable m_Enemy;
		protected Damager m_Damager;
		protected Vector3 m_Origin;
        CancellationTokenSource m_AttackCts;

		public void AttackEnemy(Vector3 origin, Targetable enemy)
		{
			m_Enemy = enemy;
			m_Origin = origin;

            m_AttackCts?.Cancel();
            m_AttackCts?.Dispose();
            m_AttackCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            DelayAttackAsync(m_AttackCts.Token).Forget();
		}

        async UniTaskVoid DelayAttackAsync(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
                DealDamage();
            }
            catch(OperationCanceledException)
            {
            }
        }

        protected virtual void OnDisable()
        {
            m_AttackCts?.Cancel();
            m_AttackCts?.Dispose();
            m_AttackCts = null;
        }

		protected void DealDamage()
		{
			Poolable.TryPool(gameObject);

			if (m_Enemy == null)
			{
				return;
			}
			
			ParticleSystem pfxPrefab = m_Damager.collisionParticles;
			var attackEffect = Poolable.TryGetPoolable<ParticleSystem>(pfxPrefab.gameObject);
			attackEffect.transform.position = m_Enemy.position;
			attackEffect.Play();
			
			m_Enemy.TakeDamage(m_Damager.damage, m_Enemy.position, m_Damager.alignmentProvider);
		}

		protected virtual void Awake()
		{
			m_Damager = GetComponent<Damager>();
		}
	}
}