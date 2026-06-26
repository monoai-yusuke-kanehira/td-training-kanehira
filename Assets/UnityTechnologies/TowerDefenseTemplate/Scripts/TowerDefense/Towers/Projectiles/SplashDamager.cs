using ActionGameFramework.Health;
using Core.Health;
using UnityEngine;

namespace TowerDefense.Towers.Projectiles
{
	/// <summary>
	/// 衝突時に範囲ダメージを与えるコンポーネント
	/// </summary>
	public class SplashDamager : MonoBehaviour
	{
		/// <summary>
		/// このProjectileが攻撃する範囲
		/// </summary>
		public float attackRange = 0.6f;

		/// <summary>
		/// 与えるダメージ量。Damagerのダメージに対する割合です
		/// </summary>
		public float damageAmount;

		/// <summary>
		/// 検索対象のPhysics Layer Mask
		/// </summary>
		public LayerMask mask = -1;

		/// <summary>
		/// Projectileの属性
		/// </summary>
		public SerializableIAlignmentProvider alignment;

		static readonly Collider[] s_Enemies = new Collider[64];

		public float damage
		{
			get { return damageAmount; }
		}

		/// <summary>
		/// このDamagerの属性を取得します
		/// </summary>
		public IAlignmentProvider alignmentProvider
		{
			get { return alignment != null ? alignment.GetInterface() : null; }
		}

		/// <summary>
		/// <see cref="attackRange"/>の半径内にいるTargetableを検索します
		/// 有効であればダメージを与えます
		/// </summary>
		protected virtual void OnCollisionEnter(Collision other)
		{
			int number = Physics.OverlapSphereNonAlloc(transform.position, attackRange, s_Enemies, mask);
			for (int index = 0; index < number; index++)
			{
				Collider enemy = s_Enemies[index];
				var damageable = enemy.GetComponent<Targetable>();
				if (damageable == null)
				{
					continue;
				}
				damageable.TakeDamage(damageAmount, damageable.position, alignmentProvider);
			}
		}
	}
}