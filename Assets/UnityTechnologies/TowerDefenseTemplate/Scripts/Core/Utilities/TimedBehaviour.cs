using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
	/// <summary>
	/// MonoBehaviourでタイミング処理を扱いやすくする抽象基底クラス
	/// </summary>
	public abstract class TimedBehaviour : MonoBehaviour
	{
		/// <summary>
		/// アクティブなタイマーのリスト
		/// </summary>
		readonly List<Timer> m_ActiveTimers = new List<Timer>();

		/// <summary>
		/// タイマーをアクティブなタイマーのリストへ追加する
		/// </summary>
		/// <param name="newTimer">アクティブなタイマーのリストへ追加するタイマー</param>
		protected void StartTimer(Timer newTimer)
		{
			if (m_ActiveTimers.Contains(newTimer))
			{
				Debug.LogWarning("Timer already exists!");
			}
			else
			{
				m_ActiveTimers.Add(newTimer);
			}
		}

		/// <summary>
		/// アクティブなタイマーのリストからタイマーを削除する
		/// </summary>
		/// <param name="timer">アクティブなタイマーのリストから削除するタイマー</param>
		protected void PauseTimer(Timer timer)
		{
			if (m_ActiveTimers.Contains(timer))
			{
				m_ActiveTimers.Remove(timer);
			}
		}

		/// <summary>
		/// タイマーをリセットして削除する
		/// </summary>
		/// <param name="timer">停止するタイマー</param>
		protected void StopTimer(Timer timer)
		{
			timer.Reset();
			PauseTimer(timer);
		}

		/// <summary>
		/// アクティブなタイマーのリストを走査してTickを進める
		/// </summary>
		protected virtual void Update()
		{
			for (int i = m_ActiveTimers.Count - 1; i >= 0; i--)
			{
				if (m_ActiveTimers[i].Tick(Time.deltaTime))
				{
					StopTimer(m_ActiveTimers[i]);
				}
			}
		}
	}
}