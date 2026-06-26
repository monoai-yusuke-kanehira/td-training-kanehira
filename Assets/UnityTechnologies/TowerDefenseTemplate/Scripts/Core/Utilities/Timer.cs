using System;
using UnityEngine;

namespace Core.Utilities
{
	/// <summary>
	/// タイマーのデータモデル。TimedBehaviourによって使用、処理される
	/// </summary>
	public class Timer
	{
		/// <summary>
		/// 時間が経過したときに発火するイベント
		/// </summary>
		readonly Action m_Callback;

		/// <summary>
		/// 時間
		/// </summary>
		float m_Time, m_CurrentTime;

		/// <summary>
		/// タイマーの正規化された進行度
		/// </summary>
		public float normalizedProgress
		{
			get { return Mathf.Clamp(m_CurrentTime / m_Time, 0f, 1f); }
		}

		/// <summary>
		/// Timerのコンストラクター
		/// </summary>
		/// <param name="newTime">タイマーが計測する時間</param>
		/// <param name="onElapsed">タイマーの終了時に発火するイベント</param>
		public Timer(float newTime, Action onElapsed = null)
		{
			SetTime(newTime);

			m_CurrentTime = 0f;
			m_Callback += onElapsed;
		}

		/// <summary>
		/// AssessTimeの結果を返す
		/// </summary>
		/// <param name="deltaTime">Tick間の経過時間</param>
		/// <returns>タイマーが終了していればtrue、それ以外はfalse</returns>
		public virtual bool Tick(float deltaTime)
		{
			return AssessTime(deltaTime);
		}

		/// <summary>
		/// 時間が経過したかを確認し、Tickイベントを発火する
		/// </summary>
		/// <param name="deltaTime">評価間の経過時間</param>
		/// <returns>タイマーが終了していればtrue、それ以外はfalse</returns>
		protected bool AssessTime(float deltaTime)
		{
			m_CurrentTime += deltaTime;
			if (m_CurrentTime >= m_Time)
			{
				FireEvent();
				return true;
			}

			return false;
		}

		/// <summary>
		/// 現在時間を0にリセットする
		/// </summary>
		public void Reset()
		{
			m_CurrentTime = 0;
		}

		/// <summary>
		/// 関連付けられたタイマーイベントを発火する
		/// </summary>
		public void FireEvent()
		{
			m_Callback.Invoke();
		}

		/// <summary>
		/// 経過時間を設定する
		/// </summary>
		/// <param name="newTime">新しく設定する時間</param>
		public void SetTime(float newTime)
		{
			m_Time = newTime;

			if (newTime <= 0)
			{
				m_Time = 0.1f;
			}
		}
	}
}