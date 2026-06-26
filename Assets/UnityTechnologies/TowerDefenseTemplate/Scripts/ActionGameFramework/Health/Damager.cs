using System;
using Core.Health;
using Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ActionGameFramework.Health
{
	/// <summary>
	/// Damageable にダメージを与えるコンポーネント
	/// </summary>
	public class Damager : MonoBehaviour
	{
		/// <summary>
		/// この Damager が与えるダメージ量
		/// </summary>
		public float damage;

		/// <summary>
		/// Damager がダメージを与えたときに発火するイベント
		/// </summary>
		public Action<Vector3> hasDamaged;

		/// <summary>
		/// 衝突時の Projectile Prefab を生成するランダム確率
		/// </summary>
		[Range(0, 1)]
		public float chanceToSpawnCollisionPrefab = 1.0f;

		/// <summary>
		/// Damager が攻撃したときに再生する ParticleSystem
		/// </summary>
		public ParticleSystem collisionParticles;

		/// <summary>
		/// Damager の所属
		/// </summary>
		public SerializableIAlignmentProvider alignment;

		/// <summary>
		/// Damager の所属を取得する
		/// </summary>
		public IAlignmentProvider alignmentProvider
		{
			get { return alignment != null ? alignment.GetInterface() : null; }
		}

		/// <summary>
		/// ダメージ値を設定する処理
		/// </summary>
		/// <param name="damageAmount">
		/// 設定するダメージ量。
		/// 0 未満の値は設定されない
		/// </param>
		public void SetDamage(float damageAmount)
		{
			if (damageAmount < 0)
			{
				return;
			}
			damage = damageAmount;
		}

		/// <summary>
		/// Damageable が正常にダメージを受けたことを Damager に通知する
		/// </summary>
		public void HasDamaged(Vector3 point, IAlignmentProvider otherAlignment)
		{
			if (hasDamaged != null)
			{
				hasDamaged(point);
			}
		}

		/// <summary>
		/// ParticleSystem を生成して再生する
		/// </summary>
		void OnCollisionEnter(Collision other)
		{
			if (collisionParticles == null || Random.value > chanceToSpawnCollisionPrefab)
			{
				return;
			}

			var pfx = Poolable.TryGetPoolable<ParticleSystem>(collisionParticles.gameObject);

			pfx.transform.position = transform.position;
			pfx.Play();
		}
	}
}
