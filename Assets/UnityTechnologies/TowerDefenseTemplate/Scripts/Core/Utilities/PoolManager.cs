using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
	/// <summary>
	/// poolのDictionaryを管理し、取得と返却を扱う
	/// </summary>
	public class PoolManager : Singleton<PoolManager>
	{
		/// <summary>
		/// 対応するpoolの初期化に使うPoolableのリスト
		/// </summary>
		public List<Poolable> poolables;

		/// <summary>
		/// poolのDictionary。キーはprefab
		/// </summary>
		protected Dictionary<Poolable, AutoComponentPrefabPool<Poolable>> m_Pools;

		/// <summary>
		/// 対応するpoolからPoolableコンポーネントを取得する
		/// </summary>
		/// <param name="poolablePrefab"></param>
		/// <returns></returns>
		public Poolable GetPoolable(Poolable poolablePrefab)
		{
			if (!m_Pools.ContainsKey(poolablePrefab))
			{
				m_Pools.Add(poolablePrefab, new AutoComponentPrefabPool<Poolable>(poolablePrefab, Initialize, null,
				                                                                  poolablePrefab.initialPoolCapacity));
			}

			AutoComponentPrefabPool<Poolable> pool = m_Pools[poolablePrefab];
			Poolable spawnedInstance = pool.Get();

			spawnedInstance.pool = pool;
			return spawnedInstance;
		}

		/// <summary>
		/// Poolableコンポーネントを所属するコンポーネントpoolへ返す
		/// </summary>
		/// <param name="poolable"></param>
		public void ReturnPoolable(Poolable poolable)
		{
			poolable.pool.Return(poolable);
		}

		/// <summary>
		/// poolのDictionaryを初期化する
		/// </summary>
		protected void Start()
		{
			m_Pools = new Dictionary<Poolable, AutoComponentPrefabPool<Poolable>>();

			foreach (var poolable in poolables)
			{
				if (poolable == null)
				{
					continue;
				}
				m_Pools.Add(poolable, new AutoComponentPrefabPool<Poolable>(poolable, Initialize, null,
				                                                            poolable.initialPoolCapacity));
			}
		}

		void Initialize(Component poolable)
		{
			poolable.transform.SetParent(transform, false);
		}
	}
}