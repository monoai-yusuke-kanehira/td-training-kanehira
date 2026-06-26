using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
	/// <summary>
	/// 指定した目標を迎撃できるよう、飛行中に進路を調整する LinearProjectile の基本的な派生クラス
	/// </summary>
	public class HomingLinearProjectile : LinearProjectile
	{
		public int leadingPrecision = 2;

		public bool leadTarget;

		protected Targetable m_HomingTarget;

		Vector3 m_TargetVelocity;

		/// <summary>
		/// 発射後に追尾する目標 Transform を設定する
		/// </summary>
		/// <param name="target">追尾する目標の Transform。</param>
		public void SetHomingTarget(Targetable target)
		{
			m_HomingTarget = target;
		}

		protected virtual void FixedUpdate()
		{
			if (m_HomingTarget == null)
			{
				return;
			}

			m_TargetVelocity = m_HomingTarget.velocity;
		}

		protected override void Update()
		{
			if (!m_Fired)
			{
				return;
			}

			if (m_HomingTarget == null)
			{
				m_Rigidbody.rotation = Quaternion.LookRotation(m_Rigidbody.linearVelocity);
				return;
			}

			Quaternion aimDirection = Quaternion.LookRotation(GetHeading());

			m_Rigidbody.rotation = aimDirection;
			m_Rigidbody.linearVelocity = transform.forward * m_Rigidbody.linearVelocity.magnitude;

			base.Update();
		}

		protected Vector3 GetHeading()
		{
			if (m_HomingTarget == null)
			{
				return Vector3.zero;
			}
			Vector3 heading;
			if (leadTarget)
			{
				heading = Ballistics.CalculateLinearLeadingTargetPoint(transform.position, m_HomingTarget.position,
				                                                       m_TargetVelocity, m_Rigidbody.linearVelocity.magnitude,
				                                                       acceleration,
				                                                       leadingPrecision) - transform.position;
			}
			else
			{
				heading = m_HomingTarget.position - transform.position;
			}

			return heading.normalized;
		}

		protected override void Fire(Vector3 firingVector)
		{
			if (m_HomingTarget == null)
			{
				Debug.LogError("Homing target has not been specified. Aborting fire.");
				return;
			}
			m_HomingTarget.removed += OnTargetDied;

			base.Fire(firingVector);
		}

		void OnTargetDied(DamageableBehaviour targetable)
		{
			targetable.removed -= OnTargetDied;
			m_HomingTarget = null;
		}
	}
}
