using UnityEngine;
using UnityEngine.AI;

namespace TowerDefense.Agents
{
	/// <summary>
	/// 道を遮る塔を「越えて」通過できるエージェント
	/// </summary>
	public class FlyingAgent : Agent
	{
		/// <summary>
		/// ナビメッシュの障害物をクリアするまでの待ち時間
		/// </summary>
		protected float m_WaitTime = 0.5f;

		/// <summary>
		/// エージェントの移動を通常どおり再開できるようになるまでの現在の待機時間
		/// </summary>
		protected float m_CurrentWaitTime;

		/// <summary>
		/// 飛行エージェントがブロックされていても、障害物を通過して移動できるはずです
		/// </summary>
		protected override void OnPartialPathUpdate()
		{
			if (!isPathBlocked)
			{
				state = State.OnCompletePath;
				return;
			}
			if (!isAtDestination)
			{
				return;
			}
			m_NavMeshAgent.enabled = false;
			m_CurrentWaitTime = m_WaitTime;
			state = State.PushingThrough;
		}
		
		/// <summary>
		/// 状態に基づいて動作を制御します <see cref="Agent.State.OnCompletePath"/>, <see cref="Agent.State.OnPartialPath"/> 
		/// <see cref="Agent.State.PushingThrough"/>の状態に応じて動作を制御します
		/// </summary>
		protected override void PathUpdate()
		{
			switch (state)
			{
				case State.OnCompletePath:
					OnCompletePathUpdate();
					break;
				case State.OnPartialPath:
					OnPartialPathUpdate();
					break;
				case State.PushingThrough:
					PushingThrough();
					break;
			}
		}

		/// <summary>
		/// 飛行エージェントが押し込んでいる場合は、隙間を空けてエージェントを攻撃するために少し時間を与えます
		/// 時間が経過すると
		/// </summary>
		protected void PushingThrough()
		{
			m_CurrentWaitTime -= Time.deltaTime;

			// 目的地に到着するまでエージェントを移動し、NavMeshAgent をオーバーライドします
			transform.LookAt(m_Destination, Vector3.up);
			transform.position += transform.forward * m_NavMeshAgent.speed * Time.deltaTime;
			if (m_CurrentWaitTime > 0)
			{
				return;
			}
			// エージェントの下に navmesh があるかどうかを確認し、ない場合はタイマーをリセットします
			NavMeshHit hit;
			if (!NavMesh.Raycast(transform.position + Vector3.up, Vector3.down, out hit, navMeshMask))
			{
				m_CurrentWaitTime = m_WaitTime;
			}
			else
			{
				// 時間が経過し、その下に NavMesh がある場合は、通常どおりエージェントの移動を再開します
				m_NavMeshAgent.enabled = true;
				NavigateTo(m_Destination);
				state = isPathBlocked ? State.OnPartialPath : State.OnCompletePath;
			}
		}
	}
}
