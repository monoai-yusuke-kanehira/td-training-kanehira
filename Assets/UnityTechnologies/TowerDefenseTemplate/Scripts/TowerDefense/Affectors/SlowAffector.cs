using ActionGameFramework.Health;
using TowerDefense.Agents;
using UnityEngine;

namespace TowerDefense.Affectors
{
	/// <summary>
	/// トリガーを使って、エージェントに<see cref="AgentSlower" />コンポーネントを追加・削除します
	/// </summary>
	public class SlowAffector : PassiveAffector
	{
		/// <summary>
		/// エージェントを遅くする割合を表す正規化された値です
		/// </summary>
		[Range(0, 1)]
		public float slowFactor;

		/// <summary>
		/// UIに表示するためのスロー倍率です
		/// </summary>
		public string slowFactorFormat = "<b>Slow Factor:</b> {0}";

		/// <summary>
		/// エンティティが球に入ったときに再生されるパーティクル システム
		/// </summary>
		public ParticleSystem enterParticleSystem;

		public GameObject slowFxPrefab;

		/// <summary>
		/// エンティティが球体に入ったときに再生されるオーディオ ソース
		/// </summary>
		public AudioSource audioSource;

		/// <summary>
		/// 関連するターゲッターイベントをサブスクライブします
		/// </summary>
		protected void Awake()
		{
			towerTargetter.targetEntersRange += OnTargetEntersRange;
			towerTargetter.targetExitsRange += OnTargetExitsRange;
		}

		/// <summary>
		/// 関連するターゲッターイベントのサブスクライブを解除します
		/// </summary>
		void OnDestroy()
		{
			towerTargetter.targetEntersRange -= OnTargetEntersRange;
			towerTargetter.targetExitsRange -= OnTargetExitsRange;
		}

		/// <summary>
		/// エージェントに<see cref="AgentSlower" />を追加します
		/// </summary>
		/// <param name="target">Slowerを追加する対象のエージェントです</param>
		protected void AttachSlowComponent(Agent target)
		{
			var slower = target.GetComponent<AgentSlower>();
			if (slower == null)
			{
				slower = target.gameObject.AddComponent<AgentSlower>();
			}
			slower.Initialize(slowFactor, slowFxPrefab, target.appliedEffectOffset, target.appliedEffectScale);

			if (enterParticleSystem != null)
			{
				enterParticleSystem.Play();
			}
			if (audioSource != null)
			{
				audioSource.Play();
			}
		}

		/// <summary>
		/// エージェントが範囲外に出たら、<see cref="AgentSlower" />を削除します
		/// </summary>
		/// <param name="target">Slowerを削除する対象のエージェントです</param>
		protected void RemoveSlowComponent(Agent target)
		{
			if (target == null)
			{
				return;
			}
			var slowComponent = target.gameObject.GetComponent<AgentSlower>();
			if (slowComponent != null)
			{
				slowComponent.RemoveSlow(slowFactor);
			}
		}

		/// <summary>
		/// ターゲッターが新しいターゲット可能オブジェクトを取得したときに発生します
		/// </summary>
		protected void OnTargetEntersRange(Targetable other)
		{
			var agent = other as Agent;
			if (agent == null)
			{
				return;
			}
			AttachSlowComponent(agent);
		}

		/// <summary>
		/// ターゲット設定者がターゲット可能オブジェクトを取得したときに発生します
		/// </summary>
		protected void OnTargetExitsRange(Targetable other)
		{
			var searchable = other as Agent;
			if (searchable == null)
			{
				return;
			}
			RemoveSlowComponent(searchable);
		}
	}
}
