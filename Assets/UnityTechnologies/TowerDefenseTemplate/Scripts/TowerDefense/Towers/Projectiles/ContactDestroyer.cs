using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Towers.Projectiles
{
	/// <summary>
	/// 接触時に自身を破棄するオブジェクト用
	/// </summary>
	public class ContactDestroyer : MonoBehaviour
	{
		/// <summary>
		/// オブジェクトが自身を破棄するY座標値
		/// </summary>
		public float yDestroyPoint = -50;

		/// <summary>
		/// アタッチされているCollider
		/// </summary>
		protected Collider m_AttachedCollider;

		/// <summary>
		/// アタッチされているColliderをキャッシュします
		/// </summary>
		protected virtual void Awake()
		{
			m_AttachedCollider = GetComponent<Collider>();
		}

		/// <summary>
		/// Y座標を<see cref="yDestroyPoint"/>と比較します
		/// </summary>
		protected virtual void Update()
		{
			if (transform.position.y < yDestroyPoint)
			{
				ReturnToPool();
			}
		}

		void OnCollisionEnter(Collision other)
		{
			ReturnToPool();
		}

		/// <summary>
		/// 可能ならオブジェクトをPoolに戻し、できなければ破棄します
		/// </summary>
		void ReturnToPool()
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}
			Poolable.TryPool(gameObject);
		}
	}
}