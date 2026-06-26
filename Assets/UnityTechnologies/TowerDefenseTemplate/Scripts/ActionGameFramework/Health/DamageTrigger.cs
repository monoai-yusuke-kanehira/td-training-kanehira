using UnityEngine;

namespace ActionGameFramework.Health
{
	/// <summary>
	/// DamageZone を Trigger ベースで実装したダメージ用トリガー
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class DamageTrigger : DamageZone
	{
		/// <summary>
		/// Trigger に入った Collider に Damager があるか確認し、あれば damageableBehaviour にダメージを与える
		/// </summary>
		/// <param name="triggeredCollider">Trigger に入った Collider</param>
		protected void OnTriggerEnter(Collider triggeredCollider)
		{
			var damager = triggeredCollider.GetComponent<Damager>();
			if (damager == null)
			{
				return;
			}
			LazyLoad();
			
			float scaledDamage = ScaleDamage(damager.damage);
			Vector3 collisionPosition = triggeredCollider.ClosestPoint(damager.transform.position);
			damageableBehaviour.TakeDamage(scaledDamage, collisionPosition, damager.alignmentProvider);
			
			damager.HasDamaged(collisionPosition, damageableBehaviour.configuration.alignmentProvider);
		}
	}
}
