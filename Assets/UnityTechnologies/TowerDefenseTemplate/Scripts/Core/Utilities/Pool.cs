using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Utilities
{
	/// <summary>
	/// オブジェクトのpoolを管理する
	/// </summary>
	public class Pool<T>
	{
		/// <summary>
		/// このpoolの生成関数
		/// </summary>
		protected Func<T> m_Factory;

		/// <summary>
		/// このpoolのリセット関数
		/// </summary>
		protected readonly Action<T> m_Reset;

		/// <summary>
		/// 利用可能な全アイテムのリスト
		/// </summary>
		protected readonly List<T> m_Available;

		/// <summary>
		/// poolが管理する全アイテムのリスト
		/// </summary>
		protected readonly List<T> m_All;

		/// <summary>
		/// 指定された数の初期要素を持つ新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		/// <param name="reset">poolから取得するときにアイテムをリセットするための関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public Pool(Func<T> factory, Action<T> reset, int initialCapacity)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}

			m_Available = new List<T>();
			m_All = new List<T>();
			m_Factory = factory;
			m_Reset = reset;

			if (initialCapacity > 0)
			{
				Grow(initialCapacity);
			}
		}

		/// <summary>
		/// 空の新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		public Pool(Func<T> factory)
			: this(factory, null, 0)
		{
		}

		/// <summary>
		/// 指定された数の初期要素を持つ新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public Pool(Func<T> factory, int initialCapacity)
			: this(factory, null, initialCapacity)
		{
		}

		/// <summary>
		/// poolからアイテムを取得し、必要ならpoolを拡張する
		/// </summary>
		/// <returns></returns>
		public virtual T Get()
		{
			return Get(m_Reset);
		}

		/// <summary>
		/// 指定したリセット関数を使い、必要ならpoolを拡張してアイテムを取得する
		/// </summary>
		/// <param name="resetOverride">指定したオブジェクトのリセットに使う関数</param>
		public virtual T Get(Action<T> resetOverride)
		{
			if (m_Available.Count == 0)
			{
				Grow(1);
			}
			if (m_Available.Count == 0)
			{
				throw new InvalidOperationException("Failed to grow pool");
			}

			int itemIndex = m_Available.Count - 1;
			T item = m_Available[itemIndex];
			m_Available.RemoveAt(itemIndex);

			if (resetOverride != null)
			{
				resetOverride(item);
			}

			return item;
		}

		/// <summary>
		/// このpoolが指定したアイテムを含んでいるかを取得する
		/// </summary>
		public virtual bool Contains(T pooledItem)
		{
			return m_All.Contains(pooledItem);
		}

		/// <summary>
		/// アイテムをpoolへ返す
		/// </summary>
		public virtual void Return(T pooledItem)
		{
			if (m_All.Contains(pooledItem) &&
			    !m_Available.Contains(pooledItem))
			{
				ReturnToPoolInternal(pooledItem);
			}
			else
			{
				throw new InvalidOperationException("Trying to return an item to a pool that does not contain it: " + pooledItem +
				                                    ", " + this);
			}
		}

		/// <summary>
		/// すべてのアイテムをpoolへ返す
		/// </summary>
		public virtual void ReturnAll()
		{
			ReturnAll(null);
		}

		/// <summary>
		/// すべてのアイテムをpoolへ返し、それぞれにdelegateを呼び出す
		/// </summary>
		public virtual void ReturnAll(Action<T> preReturn)
		{
			for (int i = 0; i < m_All.Count; ++i)
			{
				T item = m_All[i];
				if (!m_Available.Contains(item))
				{
					if (preReturn != null)
					{
						preReturn(item);
					}
					ReturnToPoolInternal(item);
				}
			}
		}

		/// <summary>
		/// 指定した数だけpoolの要素を増やす
		/// </summary>
		public void Grow(int amount)
		{
			for (int i = 0; i < amount; ++i)
			{
				AddNewElement();
			}
		}

		/// <summary>
		/// オブジェクトをm_Availableリストへ返す。整合性チェックは行わない
		/// </summary>
		protected virtual void ReturnToPoolInternal(T element)
		{
			m_Available.Add(element);
		}

		/// <summary>
		/// poolに新しい要素を追加する
		/// </summary>
		protected virtual T AddNewElement()
		{
			T newElement = m_Factory();
			m_All.Add(newElement);
			m_Available.Add(newElement);

			return newElement;
		}

		/// <summary>
		/// Tのデフォルト値を返すダミーの生成関数
		/// </summary>		
		protected static T DummyFactory()
		{
			return default(T);
		}
	}

	/// <summary>
	/// Unityコンポーネントを扱うpoolの派生版。必要に応じて自動的に有効化、無効化する
	/// </summary>
	public class UnityComponentPool<T> : Pool<T>
		where T : Component
	{
		/// <summary>
		/// 指定された数の初期要素を持つ新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		/// <param name="reset">poolから取得するときにアイテムをリセットするための関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public UnityComponentPool(Func<T> factory, Action<T> reset, int initialCapacity)
			: base(factory, reset, initialCapacity)
		{
		}

		/// <summary>
		/// 空の新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		public UnityComponentPool(Func<T> factory)
			: base(factory)
		{
		}

		/// <summary>
		/// 指定された数の初期要素を持つ新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public UnityComponentPool(Func<T> factory, int initialCapacity)
			: base(factory, initialCapacity)
		{
		}

		/// <summary>
		/// 有効化された要素をpoolから取得する
		/// </summary>
		public override T Get(Action<T> resetOverride)
		{
			T element = base.Get(resetOverride);

			element.gameObject.SetActive(true);

			return element;
		}

		/// <summary>
		/// 返却されたオブジェクトを自動的に無効化する
		/// </summary>
		protected override void ReturnToPoolInternal(T element)
		{
			element.gameObject.SetActive(false);

			base.ReturnToPoolInternal(element);
		}

		/// <summary>
		/// 新しく作成したオブジェクトを無効化したままにする
		/// </summary>
		protected override T AddNewElement()
		{
			T newElement = base.AddNewElement();

			newElement.gameObject.SetActive(false);

			return newElement;
		}
	}

	/// <summary>
	/// UnityのGameObjectを扱うpoolの派生版。必要に応じて自動的に有効化、無効化する
	/// </summary>
	public class GameObjectPool : Pool<GameObject>
	{
		/// <summary>
		/// 指定された数の初期要素を持つ新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		/// <param name="reset">poolから取得するときにアイテムをリセットするための関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public GameObjectPool(Func<GameObject> factory, Action<GameObject> reset, int initialCapacity)
			: base(factory, reset, initialCapacity)
		{
		}

		/// <summary>
		/// 空の新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		public GameObjectPool(Func<GameObject> factory)
			: base(factory)
		{
		}

		/// <summary>
		/// 指定された数の初期要素を持つ新しいpoolを作成する
		/// </summary>
		/// <param name="factory">pool用オブジェクトを作成する関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public GameObjectPool(Func<GameObject> factory, int initialCapacity)
			: base(factory, initialCapacity)
		{
		}

		/// <summary>
		/// 有効化された要素をpoolから取得する
		/// </summary>
		public override GameObject Get(Action<GameObject> resetOverride)
		{
			GameObject element = base.Get(resetOverride);

			element.SetActive(true);

			return element;
		}

		/// <summary>
		/// 返却されたオブジェクトを自動的に無効化する
		/// </summary>
		protected override void ReturnToPoolInternal(GameObject element)
		{
			element.SetActive(false);

			base.ReturnToPoolInternal(element);
		}

		/// <summary>
		/// 新しく作成したオブジェクトを無効化したままにする
		/// </summary>
		protected override GameObject AddNewElement()
		{
			GameObject newElement = base.AddNewElement();

			newElement.SetActive(false);

			return newElement;
		}
	}

	/// <summary>
	/// 指定されたUnity GameObject prefabからオブジェクトを自動生成するpoolの派生版
	/// </summary>
	public class AutoGameObjectPrefabPool : GameObjectPool
	{
		/// <summary>
		/// 新しいprefabアイテムの複製を作成する
		/// </summary>
		GameObject PrefabFactory()
		{
			GameObject newElement = Object.Instantiate(m_Prefab);
			if (m_Initialize != null)
			{
				m_Initialize(newElement);
			}

			return newElement;
		}

		/// <summary>
		/// 元になるprefab
		/// </summary>
		protected readonly GameObject m_Prefab;

		/// <summary>
		/// オブジェクトの初期化メソッド
		/// </summary>
		protected readonly Action<GameObject> m_Initialize;

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		public AutoGameObjectPrefabPool(GameObject prefab)
			: this(prefab, null, null, 0)
		{
		}

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialize">prefab作成後に呼び出す初期化関数</param>
		public AutoGameObjectPrefabPool(GameObject prefab, Action<GameObject> initialize)
			: this(prefab, initialize, null, 0)
		{
		}

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialize">prefab作成後に呼び出す初期化関数</param>
		/// <param name="reset">poolから取得するときにアイテムをリセットするための関数</param>
		public AutoGameObjectPrefabPool(GameObject prefab, Action<GameObject> initialize, Action<GameObject> reset)
			: this(prefab, initialize, reset, 0)
		{
		}

		/// <summary>
		/// 指定された数の初期要素を持つ、指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public AutoGameObjectPrefabPool(GameObject prefab, int initialCapacity)
			: this(prefab, null, null, initialCapacity)
		{
		}

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialize">prefab作成後に呼び出す初期化関数</param>
		/// <param name="reset">poolから取得するときにアイテムをリセットするための関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public AutoGameObjectPrefabPool(GameObject prefab, Action<GameObject> initialize, Action<GameObject> reset,
		                                int initialCapacity)
			: base(DummyFactory, reset, 0)
		{
			// 先に自分自身を設定する必要があるため、初期容量には0を渡す
			// その後、自分でGrowをもう一度呼び出す
			m_Initialize = initialize;
			m_Prefab = prefab;
			m_Factory = PrefabFactory;
			if (initialCapacity > 0)
			{
				Grow(initialCapacity);
			}
		}
	}

	/// <summary>
	/// 指定されたUnityコンポーネントprefabからオブジェクトを自動生成するpoolの派生版
	/// </summary>
	public class AutoComponentPrefabPool<T> : UnityComponentPool<T>
		where T : Component
	{
		/// <summary>
		/// 新しいprefabアイテムの複製を作成する
		/// </summary>
		T PrefabFactory()
		{
			T newElement = Object.Instantiate(m_Prefab);
			if (m_Initialize != null)
			{
				m_Initialize(newElement);
			}

			return newElement;
		}

		/// <summary>
		/// 元になるprefab
		/// </summary>
		protected readonly T m_Prefab;

		/// <summary>
		/// オブジェクトの初期化メソッド
		/// </summary>
		protected readonly Action<T> m_Initialize;

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		public AutoComponentPrefabPool(T prefab)
			: this(prefab, null, null, 0)
		{
		}

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialize">prefab作成後に呼び出す初期化関数</param>
		public AutoComponentPrefabPool(T prefab, Action<T> initialize)
			: this(prefab, initialize, null, 0)
		{
		}

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialize">prefab作成後に呼び出す初期化関数</param>
		/// <param name="reset">poolから取得するときにアイテムをリセットするための関数</param>
		public AutoComponentPrefabPool(T prefab, Action<T> initialize, Action<T> reset)
			: this(prefab, initialize, reset, 0)
		{
		}

		/// <summary>
		/// 指定された数の初期要素を持つ、指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public AutoComponentPrefabPool(T prefab, int initialCapacity)
			: this(prefab, null, null, initialCapacity)
		{
		}

		/// <summary>
		/// 指定されたUnity prefab用の新しいpoolを作成する
		/// </summary>
		/// <param name="prefab">複製元のprefab</param>
		/// <param name="initialize">prefab作成後に呼び出す初期化関数</param>
		/// <param name="reset">poolから取得するときにアイテムをリセットするための関数</param>
		/// <param name="initialCapacity">poolにあらかじめ用意する要素数</param>
		public AutoComponentPrefabPool(T prefab, Action<T> initialize, Action<T> reset, int initialCapacity)
			: base(DummyFactory, reset, 0)
		{
			// 先に自分自身を設定する必要があるため、初期容量には0を渡す
			// その後、自分でGrowをもう一度呼び出す
			m_Initialize = initialize;
			m_Prefab = prefab;
			m_Factory = PrefabFactory;
			if (initialCapacity > 0)
			{
				Grow(initialCapacity);
			}
		}
	}
}
