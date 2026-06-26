using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Health
{
	/// <summary>
	/// 敵を識別するためのシンプルなクラス
	/// </summary>
	public class Targetable : DamageableBehaviour
	{
		/// <summary>
		/// ターゲットになる Transform
		/// </summary>
		public Transform targetTransform;

		/// <summary>
		/// オブジェクトの位置
		/// </summary>
		protected Vector3 m_CurrentPosition, m_PreviousPosition;

		/// <summary>
		/// Rigidbody の速度
		/// </summary>
		public virtual Vector3 velocity { get; protected set; }
		
		/// <summary>
		/// 他のオブジェクトが狙う Transform。未設定の場合はこのオブジェクトの Transform を使う
		/// </summary>
		public Transform targetableTransform
		{
			get
			{
				return targetTransform == null ? transform : targetTransform;
			}
		}

		/// <summary>
		/// この Targetable の Transform 位置を返す
		/// </summary>
		public override Vector3 position
		{
			get { return targetableTransform.position; }
		}

		/// <summary>
		/// DamageableBehaviour の処理を初期化する
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			ResetPositionData();
		}

		/// <summary>
		/// 速度を計算できるように位置データを設定する
		/// </summary>
		protected void ResetPositionData()
		{
			m_CurrentPosition = position;
			m_PreviousPosition = position;
		}

		/// <summary>
		/// 速度を計算し、位置を更新する
		/// </summary>
		void FixedUpdate()
		{
			m_CurrentPosition = position;
			velocity = (m_CurrentPosition - m_PreviousPosition) / Time.fixedDeltaTime;
			m_PreviousPosition = m_CurrentPosition;
		}
	}
}
