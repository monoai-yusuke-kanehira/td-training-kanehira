using System.Collections.Generic;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Effects
{
	/// <summary>
	/// 有効化時にTrailとParticleをリセットし、さらに
	/// 再利用されるEmitterを停止してから開始するためのシンプルなエフェクト補助スクリプト（プールへ戻した後の移動中に放出されるのを防ぎます）
	/// </summary>
	public class PoolableEffect : Poolable
	{
		protected List<ParticleSystem> m_Systems;
		protected List<TrailRenderer> m_Trails;

		bool m_EffectsEnabled;
		
		/// <summary>
		/// すべてのParticleの放出を停止します
		/// </summary>
		public void StopAll()
		{
			foreach (var particleSystem in m_Systems)
			{
				particleSystem.Stop();
			}
		}
		
		/// <summary>
		/// 既知のすべてのシステムをオフにします
		/// </summary>
		public void TurnOffAllSystems()
		{
			if (!m_EffectsEnabled)
			{
				return;
			}
			
			// すべてのシステムとTrailをリセットします
			foreach (var particleSystem in m_Systems)
			{
				particleSystem.Clear();
				var emission = particleSystem.emission;
				emission.enabled = false;
			}

			foreach (var trailRenderer in m_Trails)
			{
				trailRenderer.Clear();
				trailRenderer.enabled = false;
			}

			m_EffectsEnabled = false;
		}

		/// <summary>
		/// 既知のすべてのシステムをオンにします
		/// </summary>
		public void TurnOnAllSystems()
		{
			if (m_EffectsEnabled)
			{
				return;
			}
			
			// すべてのシステムとTrailを再度有効化します
			foreach (var particleSystem in m_Systems)
			{
				particleSystem.Clear();
				var emission = particleSystem.emission;
				emission.enabled = true;
			}

			foreach (var trailRenderer in m_Trails)
			{
				trailRenderer.Clear();
				trailRenderer.enabled = true;
			}

			m_EffectsEnabled = true;
		}

		protected override void Repool()
		{
			base.Repool();
			TurnOffAllSystems();
		}

		protected virtual void Awake()
		{
			m_EffectsEnabled = true;
			
			// システムとTrailをキャッシュします。ただし、アクティブで放出中のものだけです
			m_Systems = new List<ParticleSystem>();
			m_Trails = new List<TrailRenderer>();

			foreach (var system in GetComponentsInChildren<ParticleSystem>())
			{
				if (system.emission.enabled && system.gameObject.activeSelf)
				{
					m_Systems.Add(system);
				}
			}
			
			foreach (var trail in GetComponentsInChildren<TrailRenderer>())
			{
				if (trail.enabled && trail.gameObject.activeSelf)
				{
					m_Trails.Add(trail);
				}
			}

			TurnOffAllSystems();
		}
	}
}