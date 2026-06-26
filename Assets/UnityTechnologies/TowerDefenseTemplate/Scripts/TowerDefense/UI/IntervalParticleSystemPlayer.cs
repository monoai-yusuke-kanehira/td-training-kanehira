using System;
using UnityEngine;

namespace TowerDefense.UI
{
	/// <summary>
	/// 指定されたParticle Systemを一定間隔で再生するシンプルなコンポーネント
	/// </summary>
	public class IntervalParticleSystemPlayer : MonoBehaviour
	{
		public ParticleSystem particleSystemToPlay;

		public float interval;

		protected DateTime m_NextPlayTime;
	
		void Start ()
		{
			m_NextPlayTime = DateTime.Now.AddSeconds(interval);
		}

		void Update()
		{
			if (particleSystemToPlay != null && m_NextPlayTime <= DateTime.Now)
			{
				particleSystemToPlay.Play();
				m_NextPlayTime = DateTime.Now.AddSeconds(interval);
			}
		}
	}
}
