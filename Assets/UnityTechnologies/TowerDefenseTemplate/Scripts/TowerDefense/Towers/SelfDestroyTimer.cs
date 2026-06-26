using Core.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense.Towers
{
	/// <summary>
	/// 自壊処理用の補助コンポーネント
	/// </summary>
	public class SelfDestroyTimer : MonoBehaviour
	{
		/// <summary>
		/// 破棄までの時間
		/// </summary>
		public float time = 5;

		/// <summary>
		/// 制御用タイマー
		/// </summary>
		public Timer timer;
		
		/// <summary>
		/// 公開されている死亡時コールバック
		/// </summary>
		public UnityEvent death;

		/// <summary>
		/// 必要に応じて時間を初期化します
		/// </summary>
		protected virtual void OnEnable()
		{
			if (timer == null)
			{
				timer = new Timer(time, OnTimeEnd);
			}
			else
			{
				timer.Reset();
			}
		}

		/// <summary>
		/// タイマーを更新します
		/// </summary>
		protected virtual void Update()
		{
			if (timer == null)
			{
				return;
			}
			timer.Tick(Time.deltaTime);
		}

		/// <summary>
		/// タイマー終了時に発火します
		/// </summary>
		protected virtual void OnTimeEnd()
		{
			death.Invoke();
			Poolable.TryPool(gameObject);
			timer.Reset();
		}
	}
}