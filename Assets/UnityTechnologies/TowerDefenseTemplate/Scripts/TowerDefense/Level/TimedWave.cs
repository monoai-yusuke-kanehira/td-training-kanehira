using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// 一定の時間が経過した後に waveCompleted イベントをトリガーする wave 実装
	/// </summary>
	public class TimedWave : Wave
	{
		/// <summary>
		/// 次のウェーブが開始されるまでの時間
		/// </summary>
		[Tooltip("The time until the next wave is started")]
		public float timeToNextWave = 10f;

		/// <summary>
		/// 次のウェーブを開始するために使用されるタイマー
		/// </summary>
		protected Timer m_WaveTimer;

		public override float progress
		{
			get { return m_WaveTimer == null ? 0 : m_WaveTimer.normalizedProgress; }
		}

		/// <summary>
		/// ウェーブを初期化します
		/// </summary>
		public override void Init()
		{
			base.Init();

			if (spawnInstructions.Count > 0)
			{
				m_WaveTimer = new Timer(timeToNextWave, SafelyBroadcastWaveCompletedEvent);
				StartTimer(m_WaveTimer);
			}
		}

		/// <summary>
		/// 現在のエージェントの生成を処理し、次のエージェントの生成をセットアップします
		/// </summary>
		protected override void SpawnCurrent()
		{
			Spawn();
			if (!TrySetupNextSpawn())
			{
				StopTimer(m_SpawnTimer);
			}
		}
	}
}