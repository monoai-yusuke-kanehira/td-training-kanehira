using System;
using ActionGameFramework.Helpers;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
	/// <summary>
	/// 必要に応じて m_Acceleration を使いながら直線飛行する Projectile 向けのシンプルな IProjectile 実装
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class LinearProjectile : MonoBehaviour, IProjectile
	{
		public float acceleration;

		public float startSpeed;

		protected bool m_Fired;

		protected Rigidbody m_Rigidbody;

		public event Action fired;

		/// <summary>
		/// 指定した開始位置から指定したワールド座標へ向けてこの Projectile を発射する
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="targetPoint">飛行先の目標位置。</param>
		public virtual void FireAtPoint(Vector3 startPoint, Vector3 targetPoint)
		{
			transform.position = startPoint;

			Fire(Ballistics.CalculateLinearFireVector(startPoint, targetPoint, startSpeed));
		}

		/// <summary>
		/// 指定した方向へこの Projectile を発射する
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="fireVector">飛行方向を表す Vector。</param>
		public virtual void FireInDirection(Vector3 startPoint, Vector3 fireVector)
		{
			transform.position = startPoint;

			// 初速がない場合は、発射ベクトルに基準となる大きさを持たせるため小さな値を設定する
			if (Math.Abs(startSpeed) < float.Epsilon)
			{
				startSpeed = 0.001f;
			}

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

		protected virtual void Awake()
		{
			m_Rigidbody = GetComponent<Rigidbody>();
		}

		protected virtual void Update()
		{
			if (!m_Fired)
			{
				return;
			}

			if (Math.Abs(acceleration) >= float.Epsilon)
			{
				m_Rigidbody.linearVelocity += transform.forward * acceleration * Time.deltaTime;
			}
		}

		protected virtual void Fire(Vector3 firingVector)
		{
			m_Fired = true;

			transform.rotation = Quaternion.LookRotation(firingVector);

			m_Rigidbody.linearVelocity = firingVector;

			if (fired != null)
			{
				fired();
			}
		}
	}
}
