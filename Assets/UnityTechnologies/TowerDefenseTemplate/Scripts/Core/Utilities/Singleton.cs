using UnityEngine;

namespace Core.Utilities
{
	/// <summary>
	/// Singletonクラス
	/// </summary>
	/// <typeparam name="T">Singletonの型</typeparam>
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		/// <summary>
		/// インスタンスへのstatic参照
		/// </summary>
		public static T instance { get; protected set; }

		/// <summary>
		/// このSingletonのインスタンスが存在するかを取得する
		/// </summary>
		public static bool instanceExists
		{
			get { return instance != null; }
		}

		/// <summary>
		/// Singletonとインスタンスを関連付けるAwakeメソッド
		/// </summary>
		protected virtual void Awake()
		{
			if (instanceExists)
			{
				Destroy(gameObject);
			}
			else
			{
				instance = (T) this;
			}
		}

		/// <summary>
		/// Singletonとの関連付けを解除するOnDestroyメソッド
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}
	}
}