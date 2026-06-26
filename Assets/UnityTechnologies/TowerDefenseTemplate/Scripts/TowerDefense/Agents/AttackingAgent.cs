using Core.Health;
using TowerDefense.Affectors;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Agents
{
	/// <summary>
	/// 攻撃するエージェントの実装 
	/// その​​進路を妨げるあらゆる塔
	/// </summary>
	public class AttackingAgent : Agent
	{
		/// <summary>
		/// ターゲットへのタワー
		/// </summary>
		protected Tower m_TargetTower;

		/// <summary>
		/// 付属の攻撃アフェクター
		/// </summary>
		protected AttackAffector m_AttackAffector;
		
		/// <summary>
		/// このエージェントは現在タワーと交戦中ですか?
		/// </summary>
		protected bool m_IsAttacking;

		public override void Initialize()
		{
			base.Initialize();
			
			// 攻撃Affectorです
			m_AttackAffector.Initialize(configuration.alignmentProvider);
			
			// エージェントには、進路がブロックされるまでタワーを攻撃してほしくないのですが、 
			// したがって、必要になるまで m_ AttackAffector を無効にします
			m_AttackAffector.enabled = false;
		}

		/// <summary>
		/// 追跡されたタワーの削除イベントの登録を解除する
		/// そして、アタッチされた攻撃アフェクターを無効にします
		/// </summary>
		public override void Remove()
		{
			base.Remove();
			if (m_TargetTower != null)
			{
				m_TargetTower.removed -= OnTargetTowerDestroyed;
			}
			m_AttackAffector.enabled = false;
			m_TargetTower = null;
		}

		/// <summary>
		/// エージェントに最も近いタワーを取得します
		/// </summary>
		/// <returns>一番近い塔</returns>
		protected Tower GetClosestTower()
		{
			var towerController = m_AttackAffector.towerTargetter.GetTarget() as Tower;
			return towerController;
		}

		/// <summary>
		/// 必要に応じて攻撃アフェクターをキャッシュします
		/// </summary>
		protected override void LazyLoad()
		{
			base.LazyLoad();
			if (m_AttackAffector == null)
			{
				m_AttackAffector = GetComponent<AttackAffector>();
			}
		}
		
		/// <summary>
		/// 他のエージェントが攻撃している間にタワーが破壊された場合、タワーが無効になることを確認します
		/// </summary>
		/// <param name="tower">破壊された塔</param>
		protected virtual void OnTargetTowerDestroyed(DamageableBehaviour tower)
		{
			if (m_TargetTower == tower)
			{
				m_TargetTower.removed -= OnTargetTowerDestroyed;
				m_TargetTower = null;
			}
		}
		
		/// <summary>
		/// 状態に関連するパスの更新を実行します <see cref="Agent.State.OnCompletePath"/>, 
		/// <see cref="Agent.State.OnPartialPath"/> and <see cref="Agent.State.Attacking"/>
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
				case State.Attacking:
					AttackingUpdate();
					break;
			}
		}
		
		/// <summary>
		///に変更 <see cref="Agent.State.OnCompletePath" /> パスがブロックされなくなったとき、または
		/// <see cref="Agent.State.Attacking" /> エージェントが到着すると <see cref="AttackingAgent.m_TargetTower" />
		/// </summary>
		protected override void OnPartialPathUpdate()
		{
			if (!isPathBlocked)
			{
				state = State.OnCompletePath;
				return;
			}

			// 部分的なパスの終点で最も近いタワーを確認する
			m_AttackAffector.towerTargetter.transform.position = m_NavMeshAgent.pathEndPosition;
			Tower tower = GetClosestTower();
			if (tower != m_TargetTower)
			{
				// 現在のターゲットを置き換える場合は、削除されたイベントのサブスクライブを解除します
				if (m_TargetTower != null)
				{
					m_TargetTower.removed -= OnTargetTowerDestroyed;
				}
				
				// ターゲットを設定します。nullになる場合もあります
				m_TargetTower = tower;
				
				// 新しいターゲットが見つかった場合は、削除されたイベントをサブスクライブします
				if (m_TargetTower != null)
				{
					m_TargetTower.removed += OnTargetTowerDestroyed;
				}
			}
			if (m_TargetTower == null)
			{
				return;
			}
			float distanceToTower = Vector3.Distance(transform.position, m_TargetTower.transform.position);
			if (!(distanceToTower < m_AttackAffector.towerTargetter.effectRadius))
			{
				return;
			}
			if (!m_AttackAffector.enabled)
			{
				m_AttackAffector.towerTargetter.transform.position = transform.position;
				m_AttackAffector.enabled = true;
			}
			state = State.Attacking;
			m_NavMeshAgent.isStopped = true;
		}
		
		/// <summary>
		/// エージェントは、パスが再び利用可能になるまで、またはターゲットのタワーを破壊するまで攻撃します
		/// </summary>
		protected void AttackingUpdate()
		{
			if (m_TargetTower != null)
			{
				return;
			}
			MoveToNode();

			// ブロックが解除されたらパスを再開します
			m_IsAttacking = false;
			m_NavMeshAgent.isStopped = false;
			m_AttackAffector.enabled = false;
			state = isPathBlocked ? State.OnPartialPath : State.OnCompletePath;
			// ターゲッターをエージェントの位置に戻します
			m_AttackAffector.towerTargetter.transform.position = transform.position;
		}
	}
}
