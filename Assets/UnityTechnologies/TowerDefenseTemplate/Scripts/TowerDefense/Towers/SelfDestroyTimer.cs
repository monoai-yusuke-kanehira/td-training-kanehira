using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Core.Utilities;

namespace TowerDefense.Towers
{
	public class SelfDestroyTimer : MonoBehaviour
	{
		public float time = 5;
		public UnityEvent death;
        CancellationTokenSource m_DestroyCts;

        /// <summary>
        /// 非同期タイマーを開始する処理
        /// </summary>
		protected virtual void OnEnable()
        {
            m_DestroyCts?.Cancel();
            m_DestroyCts?.Dispose();
            m_DestroyCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            RunDestroyTimerAsync(m_DestroyCts.Token).Forget();
        }

        /// <summary>
        /// 非同期で待つ処理の実装
        /// </summary>
        async UniTaskVoid RunDestroyTimerAsync(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cancellationToken);
                OnTimeEnd();
            }
            catch(OperationCanceledException)
            {
            }
        }

        protected virtual void OnDisable()
        {
            m_DestroyCts?.Cancel();
            m_DestroyCts?.Dispose();
            m_DestroyCts = null;
        }

		protected virtual void OnTimeEnd()
		{
			death.Invoke();
			Poolable.TryPool(gameObject);
		}
	}
}