using System;
using System.Threading;
using Cysharp.Threading.Tasks;

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

        private CancellationTokenSource m_Cts;

		protected void Awake()
		{
            m_Cts = new CancellationTokenSource();
            RunIntroAsync(m_Cts.Token).Forget();
		}

        async UniTaskVoid RunIntroAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cancellationToken);
            SafelyCallIntroCompleted();
        }

        protected void OnDestroy()
        {
            m_Cts?.Cancel();
            m_Cts?.Dispose();
        }
	}
}