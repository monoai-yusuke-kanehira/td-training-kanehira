using UnityEngine;

namespace ActionGameFramework.Projectiles
{
	/// <summary>
	/// 上向きに発射された後、追尾を開始する Projectile
	/// </summary>
	public class WobblingHomingProjectile : HomingLinearProjectile
	{
		protected enum State
		{
			Wobbling,
			Turning,
			Targeting
		}

		/// <summary>
		/// Projectile が上向きに揺れながら進む時間を、この範囲からランダムに決める
		/// </summary>
		public Vector2 wobbleTimeRange = new Vector2(1, 2);

		/// <summary>
		/// 1 秒あたりに揺れ方向が変わる回数
		/// </summary>
		public float wobbleDirectionChangeSpeed = 4;

		/// <summary>
		/// 揺れの強さ
		/// </summary>
		public float wobbleMagnitude = 7;

		/// <summary>
		/// Projectile が旋回して追尾に入るまでの時間
		/// </summary>
		public float turningTime = 0.5f;

		/// <summary>
		/// Projectile の状態
		/// </summary>
		State m_State;

		/// <summary>
		/// 揺れている経過秒数
		/// </summary>
		protected float m_CurrentWobbleTime;

		/// <summary>
		/// 揺れる合計時間
		/// </summary>
		protected float m_WobbleDuration;

		/// <summary>
		/// 追尾目標の方向へ旋回している経過秒数
		/// </summary>
		protected float m_CurrentTurnTime;

		/// <summary>
		/// 現在の旋回にかかった秒数
		/// </summary>
		protected float m_WobbleChangeTime;

		protected Vector3 m_WobbleVector,
		                  m_TargetWobbleVector;

		protected override void Update()
		{
			// 通常の HomingLinearProjectile の挙動。追尾目標が null の場合も処理する
			if (m_HomingTarget == null || m_State == State.Targeting)
			{
				base.Update();
				return;
			}
			switch (m_State)
			{
				// Projectile を揺らす
				case State.Wobbling:
					m_CurrentWobbleTime += Time.deltaTime;
					if (m_CurrentWobbleTime >= m_WobbleDuration)
					{
						m_State = State.Turning;
						m_CurrentTurnTime = 0;
					}

					m_WobbleChangeTime += Time.deltaTime * wobbleDirectionChangeSpeed;
					if (m_WobbleChangeTime >= 1)
					{
						m_WobbleChangeTime = 0;
						m_TargetWobbleVector = new Vector3(Random.Range(-wobbleMagnitude, wobbleMagnitude),
						                                   Random.Range(-wobbleMagnitude, wobbleMagnitude), 0);
						m_WobbleVector = Vector3.zero;
					}
					m_WobbleVector = Vector3.Lerp(m_WobbleVector, m_TargetWobbleVector, m_WobbleChangeTime);
					m_Rigidbody.linearVelocity = Quaternion.Euler(m_WobbleVector) * m_Rigidbody.linearVelocity;

					m_Rigidbody.rotation = Quaternion.LookRotation(m_Rigidbody.linearVelocity);
					break;
				// 追尾目標の方向を向くように Projectile を旋回させる
				case State.Turning:
					m_CurrentTurnTime += Time.deltaTime;
					Quaternion aimDirection = Quaternion.LookRotation(GetHeading());

					m_Rigidbody.rotation = Quaternion.Lerp(m_Rigidbody.rotation, aimDirection, m_CurrentTurnTime / turningTime);
					m_Rigidbody.linearVelocity = transform.forward * m_Rigidbody.linearVelocity.magnitude;

					if (m_CurrentTurnTime >= turningTime)
					{
						m_State = State.Targeting;
					}
					break;
			}
		}

		// 最初の揺れベクトルを選び、揺れ状態に設定する
		protected override void Fire(Vector3 firingVector)
		{
			m_TargetWobbleVector = new Vector3(Random.Range(-wobbleMagnitude, wobbleMagnitude),
			                                   Random.Range(-wobbleMagnitude, wobbleMagnitude), 0);
			m_WobbleDuration = Random.Range(wobbleTimeRange.x, wobbleTimeRange.y);
			base.Fire(firingVector);
			m_State = State.Wobbling;
			m_CurrentWobbleTime = 0.0f;
		}
	}
}
