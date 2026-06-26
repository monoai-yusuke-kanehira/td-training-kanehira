using System.Collections.Generic;
using Core.Health;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Agents
{
	/// <summary>
	/// この効果は、SlowAffector の半径の範囲内にあるエージェントに付加されます
	/// </summary>
	public class AgentSlower : AgentEffect
	{
		protected GameObject m_SlowFx;

		protected List<float> m_CurrentEffects = new List<float>();

		/// <summary>
		/// SlowAffector で設定されたパラメータを使用してスローを初期化します
		/// </summary>
		/// <param name="slowFactor">エージェントに適用される速度低下の割合を表す正規化された浮動小数点数</param>
		/// <param name="slowfxPrefab">スローエフェクトを視覚化するためにインスタンス化されたオブジェクト</param>
		/// <param name="position"></param>
		/// <param name="scale"></param>
		public void Initialize(float slowFactor, GameObject slowfxPrefab = null, 
		                       Vector3 position = default(Vector3),
		                       float scale = 1)
		{
			LazyLoad();
			m_CurrentEffects.Add(slowFactor);

			// 最も強いスロー効果を探します
			float min = slowFactor;
			foreach (float item in m_CurrentEffects)
			{
				min = Mathf.Min(min, item);
			}
			
			float originalSpeed = m_Agent.originalMovementSpeed;
			float newSpeed = originalSpeed * min;
			m_Agent.navMeshNavMeshAgent.speed = newSpeed;

			if (m_SlowFx == null && slowfxPrefab != null)
			{
				m_SlowFx = Poolable.TryGetPoolable(slowfxPrefab);
				m_SlowFx.transform.parent = transform;
				m_SlowFx.transform.localPosition = position;
				m_SlowFx.transform.localScale *= scale;
			}
			m_Agent.removed += OnRemoved;
		}

		/// <summary>
		/// エージェントの速度をリセットします 
		/// </summary>
		public void RemoveSlow(float slowFactor)
		{
			m_Agent.removed -= OnRemoved;
			
			m_CurrentEffects.Remove(slowFactor);
			if (m_CurrentEffects.Count != 0)
			{
				return;
			}
			
			// スロー効果が残っていません
			ResetAgent();
		}

		/// <summary>
		/// エージェントが死亡しました。影響を削除します
		/// </summary>
		void OnRemoved(DamageableBehaviour targetable)
		{
			m_Agent.removed -= OnRemoved;
			ResetAgent();
		}

		void ResetAgent()
		{
			if (m_Agent != null)
			{
				m_Agent.navMeshNavMeshAgent.speed = m_Agent.originalMovementSpeed;
			}
			if (m_SlowFx != null)
			{
				Poolable.TryPool(m_SlowFx);
				m_SlowFx.transform.localScale = Vector3.one;
				m_SlowFx = null;
			}
			Destroy(this);
		}
	}
}
