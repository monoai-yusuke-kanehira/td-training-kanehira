using System.Collections.Generic;
using ActionGameFramework.Audio;
using Core.Health;
using TowerDefense.Agents;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// プレイヤーが守らなければならない本拠地を表すクラス
	/// </summary>
	public class PlayerHomeBase : DamageableBehaviour
	{
		/// <summary>
		///攻撃がチャージ中のパーティクルシステム
		/// </summary>
		public ParticleSystem chargePfx;

		/// <summary>
		/// チャージ効果開始時に鳴る音
		/// </summary>
		public RandomAudioSource chargeSound;
		
		/// <summary>
		/// 攻撃用のパーティクル システム
		/// </summary>
		public ParticleSystem attackPfx;
		
		/// <summary>
		/// 攻撃エフェクト開始時に鳴る音
		/// </summary>
		public RandomAudioSource attackSound;

		/// <summary>
		/// 本拠地攻撃ゾーン内の現在のエージェント
		/// </summary>
		protected List<Agent> m_CurrentAgentsInside = new List<Agent>();

		/// <summary>
		/// 破損したイベントをサブスクライブします
		/// </summary>
		protected virtual void Start()
		{
			configuration.damaged += OnDamaged;
		}

		/// <summary>
		/// 破損したイベントの登録を解除する
		/// </summary>
		protected virtual void OnDestroy()
		{
			configuration.damaged -= OnDamaged;
		}

		/// <summary>
		/// <see cref="attackPfx"/>が設定されていれば再生します
		/// </summary>
		protected virtual void OnDamaged(HealthChangeInfo obj)
		{
			if (attackPfx != null)
			{
				attackPfx.Play();
			}
			if (attackSound != null)
			{
				attackSound.PlayRandomClip();
			}
		}
		
		/// <summary>
		/// トリガーされたエージェントを追跡対象のエージェントに追加し、エージェントのサブスクライブします
		/// イベントを削除し、PFX を再生します
		/// </summary>
		/// <param name="other">Triggered collider</param>
		void OnTriggerEnter(Collider other)
		{
			var homeBaseAttacker = other.GetComponent<HomeBaseAttacker>();
			if (homeBaseAttacker == null)
			{
				return;
			}
			m_CurrentAgentsInside.Add(homeBaseAttacker.agent);
			homeBaseAttacker.agent.removed += OnAgentRemoved;
			if (chargePfx != null)
			{
				chargePfx.Play();
			}
			if (chargeSound != null)
			{
				chargeSound.PlayRandomClip();
			}
		}
		
		/// <summary>
		/// コライダーに入ったエンティティの場合
		/// が付いています <see cref="Agent"/> その上のコンポーネント
		/// </summary>
		void OnTriggerExit(Collider other)
		{
			var homeBaseAttacker = other.GetComponent<HomeBaseAttacker>();
			if (homeBaseAttacker == null)
			{
				return;
			}
			RemoveTarget(homeBaseAttacker.agent);
		}
		
		/// <summary>
		/// 追跡対象からエージェントを削除します <see cref="m_CurrentAgentsInside"/>
		/// </summary>
		void OnAgentRemoved(DamageableBehaviour targetable)
		{
			targetable.removed -= OnAgentRemoved;
			Agent attackingAgent = targetable as Agent;
			RemoveTarget(attackingAgent);
		}

		/// <summary>
		/// <paramref name="agent"/>を<see cref="m_CurrentAgentsInside"/>から削除し、pfxを停止します
		/// もうなくなったら <see cref="Agent"/>s
		/// </summary>
		/// <param name="agent">
		/// 削除するエージェント
		/// </param>
		void RemoveTarget(Agent agent)
		{
			if (agent == null)
			{
				return;
			}
			m_CurrentAgentsInside.Remove(agent);
			if (m_CurrentAgentsInside.Count == 0 && chargePfx != null)
			{
				chargePfx.Stop();
			}
		}
	}
}
