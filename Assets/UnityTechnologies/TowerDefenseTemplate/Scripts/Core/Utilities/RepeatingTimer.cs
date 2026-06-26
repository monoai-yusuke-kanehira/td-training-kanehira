using System;

namespace Core.Utilities
{
	/// <summary>
	/// 停止されるまで繰り返すTimer。各繰り返しの終了時にコールバックを発火する
	/// </summary>
	public class RepeatingTimer : Timer
	{
		/// <summary>
		/// コンストラクター
		/// </summary>
		/// <param name="time">1サイクルの時間</param>
		/// <param name="onElapsed">各サイクルの終了時に発火するイベント</param>
		public RepeatingTimer(float time, Action onElapsed = null)
			: base(time, onElapsed)
		{
		}

		/// <summary>
		/// Tickを進め、経過後もオフにしない
		/// </summary>
		/// <param name="deltaTime">前回のTickからの経過時間</param>
		/// <returns>タイマーが自動削除されないように常にfalseを返す</returns>
		public override bool Tick(float deltaTime)
		{
			if (AssessTime(deltaTime))
			{
				Reset();
			}

			return false;
		}
	}
}