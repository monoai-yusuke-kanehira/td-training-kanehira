using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// イントロの基本実装: ディレイ
	/// </summary>
	public class TimedLevelIntro : LevelIntro
	{
		/// <summary>
		/// 遅延
		/// </summary>
		public float time = 5f;

		/// <summary>
		/// 遅延を追跡するために使用されるタイマー オブジェクト
		/// </summary>
		protected Timer m_Timer;

		/// <summary>
		/// タイマーを設定し、SafelyCallIntroCompleted イベントを発生させます
		/// </summary>
		protected void Awake()
		{
			m_Timer = new Timer(time, SafelyCallIntroCompleted);
		}

		/// <summary>
		/// タイマーにチェックを入れ、完了したら無効にします
		/// </summary>
		protected void Update()
		{
			if (m_Timer != null)
			{
				if (m_Timer.Tick(Time.deltaTime))
				{
					m_Timer = null;
				}
			}
		}
	}
}