using Core.Utilities;
using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// 指定したDamageableの死亡時にParticleSystemを生成するシンプルなクラス
	/// </summary>
	public class DeathEffect : MonoBehaviour
	{
		/// <summary>
		/// Damageableの割り当てに使うDamageableBehaviour
		/// </summary>
		[Tooltip("This field does not need to be populated here, it can be set up in code using AssignDamageable")]
		public DamageableBehaviour damageableBehaviour;
		
		/// <summary>
		/// 死亡時のParticleSystem
		/// </summary>
		public ParticleSystem deathParticleSystemPrefab;

		/// <summary>
		/// <see cref="deathParticleSystemPrefab"/> の位置に加えるワールド空間オフセット
		/// </summary>
		public Vector3 deathEffectOffset;

		/// <summary>
		/// 対象の Damageable
		/// </summary>
		protected Damageable m_Damageable;

		/// <summary>
		/// damageableのdiedイベントを購読します
		/// </summary>
		/// <param name="damageable"></param>
		public void AssignDamageable(Damageable damageable)
		{
			if (m_Damageable != null)
			{
				m_Damageable.died -= OnDied;
			}
			m_Damageable = damageable;
			m_Damageable.died += OnDied;
		}

		/// <summary>
		/// damageableBehaviourが設定されている場合、damageableを割り当てます
		/// </summary>
		protected virtual void Awake () 
		{
			if (damageableBehaviour != null)
			{
				AssignDamageable(damageableBehaviour.configuration);
			}
		}

		/// <summary>
		/// 死亡時のParticleSystemを生成します
		/// </summary>
		void OnDied(HealthChangeInfo healthChangeInfo)
		{
			if (deathParticleSystemPrefab == null)
			{
				return;
			}

			var pfx = Poolable.TryGetPoolable<ParticleSystem>(deathParticleSystemPrefab.gameObject);
			pfx.transform.position = transform.position + deathEffectOffset;
			pfx.Play();
		}
	}
}
