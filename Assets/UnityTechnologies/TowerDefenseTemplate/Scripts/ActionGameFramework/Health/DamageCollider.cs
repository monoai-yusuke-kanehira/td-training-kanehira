using UnityEngine;

namespace ActionGameFramework.Health
{
	/// <summary>
	/// DamageZone を Collider ベースで実装したダメージ用コライダー
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class DamageCollider : DamageZone
	{
		/// <summary>
		/// 衝突したオブジェクトに Damager があるか確認し、あれば damageableBehaviour にダメージを与える
		/// </summary>
		/// <param name="c">衝突情報</param>
		protected void OnCollisionEnter(Collision c)
		{
			var damager = c.gameObject.GetComponent<Damager>();
			if (damager == null)
			{
				return;
			}
			LazyLoad();
			
			float scaledDamage = ScaleDamage(damager.damage);
			Vector3 collisionPosition = ConvertContactsToPosition(c.contacts);
			damageableBehaviour.TakeDamage(scaledDamage, collisionPosition, damager.alignmentProvider);
			
			damager.HasDamaged(collisionPosition, damageableBehaviour.configuration.alignmentProvider);
		}

		/// <summary>
		/// 接触点の平均から位置を取得する
		/// </summary>
		/// <returns>平均位置。</returns>
		/// <param name="contacts">接触点。</param>
		protected Vector3 ConvertContactsToPosition(ContactPoint[] contacts)
		{
			Vector3 output = Vector3.zero;
			int length = contacts.Length;

			if (length == 0)
			{
				return output;
			}

			for (int i = 0; i < length; i++)
			{
				output += contacts[i].point;
			}

			output = output / length;
			return output;
		}
	}
}
