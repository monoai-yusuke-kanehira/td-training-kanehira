using ActionGameFramework.Health;
using Core.Health;
using Core.Utilities;
using TowerDefense.Nodes;
using UnityEngine;

namespace TowerDefense.Agents
{
	/// <summary>
	/// エージェントが本拠地に到達すると、本拠地を攻撃するコンポーネント 
	/// </summary>
	[RequireComponent(typeof(Agent))]
	public class HomeBaseAttacker : MonoBehaviour 
	{
		/// <summary>
		/// エージェントが攻撃するまでの充電時間
		/// ホームベース
		/// </summary>
		public float homeBaseAttackChargeTime = 0.5f;

		/// <summary>
		/// 本塁への攻撃を遅らせるために使用されるタイマー
		/// </summary>
		protected Timer m_HomeBaseAttackTimer;

		/// <summary>
		/// エージェントがプレイヤーのホームベースに到着し、攻撃を仕掛けている場合
		/// </summary>
		protected bool m_IsChargingHomeBaseAttack;
		
		/// <summary>
		/// ホームベースでのDamageableBehaviour 
		/// </summary>
		protected DamageableBehaviour m_FinalDestinationDamageableBehaviour;

		/// <summary>
		/// このゲームオブジェクトにアタッチされたエージェント コンポーネント
		/// </summary>
		public Agent agent { get; protected set; }

		/// <summary>
		/// 完了時に解雇される <see cref="m_HomeBaseAttackTimer"/>
		/// 本拠地にダメージを与える
		/// </summary>
		protected void AttackHomeBase()
		{
			m_IsChargingHomeBaseAttack = false;
			var damager = GetComponent<Damager>();
			if (damager != null)
			{
				m_FinalDestinationDamageableBehaviour.TakeDamage(damager.damage, transform.position, agent.configuration.alignmentProvider);
			}
			agent.Remove();
		}

		/// <summary>
		/// 攻撃タイマーを刻む
		/// </summary>
		protected virtual void Update () 
		{
			// ホームベースアタックタイマーを更新する
			if (m_IsChargingHomeBaseAttack)
			{
				m_HomeBaseAttackTimer.Tick(Time.deltaTime);
			}
		}

		/// <summary>
		/// 接続されたエージェントをキャッシュし、destinationReached イベントをサブスクライブします
		/// </summary>
		protected virtual void Awake()
		{
			agent = GetComponent<Agent>();
			agent.destinationReached += OnDestinationReached;
			agent.died += OnDied;
		}

		/// <summary>
		/// destinationReached イベントのサブスクライブを解除します
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (agent != null)
			{
				agent.destinationReached -= OnDestinationReached;
				agent.died -= OnDied;
			}
		}

		/// <summary>
		/// 本塁攻撃を阻止する
		/// </summary>
		void OnDied(DamageableBehaviour damageableBehaviour)
		{
			m_IsChargingHomeBaseAttack = false;
		}

		/// <summary>
		/// エージェントが最終ノードに到達したときに起動され、
		/// アタックタイマーを開始します
		/// </summary>
		/// <param name="homeBase"></param>
		void OnDestinationReached (Node homeBase)
		{
			m_FinalDestinationDamageableBehaviour = homeBase.GetComponent<DamageableBehaviour>();
			// タイマーを開始します 
			if (m_HomeBaseAttackTimer == null)
			{
				m_HomeBaseAttackTimer = new Timer(homeBaseAttackChargeTime, AttackHomeBase);
			}
			else
			{
				m_HomeBaseAttackTimer.Reset();
			}
			m_IsChargingHomeBaseAttack = true;
		}
	}
}
