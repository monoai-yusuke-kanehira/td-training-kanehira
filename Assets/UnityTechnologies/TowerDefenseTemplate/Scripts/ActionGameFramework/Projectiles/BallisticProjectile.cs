using System;
using System.Collections.Generic;
using ActionGameFramework.Helpers;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
	/// <summary>
	/// 追加の m_Acceleration なしで放物線軌道を飛ぶ Projectile 向けのシンプルな IProjectile 実装
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class BallisticProjectile : MonoBehaviour, IProjectile
	{
		public BallisticArcHeight arcPreference;

		public BallisticFireMode fireMode;

		[Range(-90, 90)]
		public float firingAngle;

		public float startSpeed;

		/// <summary>
		/// この GameObject の Collider と指定した Collider の衝突を無視する時間
		/// </summary>
		public float collisionIgnoreTime = 0.35f;

		protected bool m_Fired, m_IgnoringCollsions;
		protected float m_CollisionIgnoreCount = 0;
		protected Rigidbody m_Rigidbody;
		protected List<Collider> m_CollidersIgnoring = new List<Collider>();
		
		/// <summary>
		/// この GameObject と子オブジェクトにアタッチされているすべての Collider
		/// </summary>
		protected Collider[] m_Colliders;

		public event Action fired;

		/// <summary>
		/// 指定した開始位置から指定したワールド座標へ向けてこの Projectile を発射する
		/// 角度が上書きされていない場合は発射速度に合わせて発射角度を自動設定し、上書きされている場合は角度に合わせて発射速度を上書きする
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="targetPoint">飛行先の目標位置。</param>
		public virtual void FireAtPoint(Vector3 startPoint, Vector3 targetPoint)
		{
			transform.position = startPoint;

			Vector3 firingVector;

			switch (fireMode)
			{
				case BallisticFireMode.UseLaunchSpeed:
					firingVector =
						Ballistics.CalculateBallisticFireVectorFromVelocity(startPoint, targetPoint, startSpeed, arcPreference);
					firingAngle = Ballistics.CalculateBallisticFireAngle(startPoint, targetPoint, startSpeed, arcPreference);
					break;
				case BallisticFireMode.UseLaunchAngle:
					firingVector = Ballistics.CalculateBallisticFireVectorFromAngle(startPoint, targetPoint, firingAngle);
					startSpeed = firingVector.magnitude;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Fire(firingVector);
		}

		/// <summary>
		/// 指定した方向へ発射速度でこの Projectile を発射する
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="fireVector">発射方向を表す Vector。</param>
		public virtual void FireInDirection(Vector3 startPoint, Vector3 fireVector)
		{
			transform.position = startPoint;

			Fire(fireVector.normalized * startSpeed);
		}

		/// <summary>
		/// 指定した初速でこの Projectile を発射し、既存の開始速度を上書きする
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="fireVelocity">発射速度を表す Vector3。</param>
		public void FireAtVelocity(Vector3 startPoint, Vector3 fireVelocity)
		{
			transform.position = startPoint;

			startSpeed = fireVelocity.magnitude;

			Fire(fireVelocity);
		}

		/// <summary>
		/// 指定した時間、この Projectile と指定した Collider 間のすべての衝突を無視する
		/// </summary>
		/// <param name="collidersToIgnore">無視する Collider。</param>
		public void IgnoreCollision(Collider[] collidersToIgnore)
		{
			if (collisionIgnoreTime > 0)
			{
				m_IgnoringCollsions = true;
				m_CollisionIgnoreCount = 0.0f;
				foreach (Collider colliderToIgnore in collidersToIgnore)
				{
					if (m_CollidersIgnoring.Contains(colliderToIgnore))
					{
						continue;
					}
					foreach (Collider projectileCollider in m_Colliders)
					{
						Physics.IgnoreCollision(colliderToIgnore, projectileCollider, true);
					}
					m_CollidersIgnoring.Add(colliderToIgnore);
				}
			}
		}

		protected virtual void Awake()
		{
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Colliders = GetComponentsInChildren<Collider>();
		}

		protected virtual void Update()
		{
			if (!m_Fired)
			{
				return;
			}
			// 衝突を無視している間はカウンターを進める
			// カウンターが完了したら衝突を再度有効にする
			if (m_IgnoringCollsions)
			{
				m_CollisionIgnoreCount += Time.deltaTime;
				if (m_CollisionIgnoreCount >= collisionIgnoreTime)
				{
					m_IgnoringCollsions = false;
					foreach (Collider colliderIgnoring in m_CollidersIgnoring)
					{
						foreach (Collider projectileCollider in m_Colliders)
						{
							Physics.IgnoreCollision(colliderIgnoring, projectileCollider, false);
						}
					}
					m_CollidersIgnoring.Clear();
				}
			}
			
			transform.rotation = Quaternion.LookRotation(m_Rigidbody.linearVelocity);
		}

		protected virtual void Fire(Vector3 firingVector)
		{
			transform.rotation = Quaternion.LookRotation(firingVector);

			m_Rigidbody.linearVelocity = firingVector;

			m_Fired = true;
			
			m_CollidersIgnoring.Clear();

			if (fired != null)
			{
				fired();
			}
		}

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			if (Mathf.Abs(firingAngle) >= 90f)
			{
				firingAngle = Mathf.Sign(firingAngle) * 89.5f;
				Debug.LogWarning("Clamping angle to under +- 90 degrees to avoid errors.");
			}
		}
#endif
	}
}
