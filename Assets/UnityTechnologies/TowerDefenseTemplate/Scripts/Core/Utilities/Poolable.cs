using UnityEngine;

namespace Core.Utilities
{
	/// <summary>
	/// pool化されるクラス
	/// </summary>
	public class Poolable : MonoBehaviour
	{
		/// <summary>
		/// poolが初期化するPoolableの数
		/// </summary>
		public int initialPoolCapacity = 10;

		/// <summary>
		/// このPoolableが所属するpool
		/// </summary>
		public Pool<Poolable> pool;

		/// <summary>
		/// このインスタンスをpoolへ戻し、PoolManagerの配下へ移動する
		/// </summary>
		protected virtual void Repool()
		{
			transform.SetParent(PoolManager.instance.transform, false);
			pool.Return(this);
		}

		/// <summary>gameObject
		/// 可能ならオブジェクトをpoolへ戻し、できなければ破棄する
		/// </summary>
		/// <param name="gameObject">poolへ戻そうとしているGameObject</param>
		public static void TryPool(GameObject gameObject)
		{
			var poolable = gameObject.GetComponent<Poolable>();
			if (poolable != null && poolable.pool != null && PoolManager.instanceExists)
			{
				poolable.Repool();
			}
			else
			{
				Destroy(gameObject);
			}
		}

		/// <summary>
		/// prefabがpool化可能ならpoolされたオブジェクトを返し、そうでなければ新しいオブジェクトを生成する
		/// </summary>
		/// <param name="prefab">必要なオブジェクトのprefab</param>
		/// <typeparam name="T">コンポーネントの型</typeparam>
		/// <returns>poolから取得、または生成されたコンポーネント</returns>
		public static T TryGetPoolable<T>(GameObject prefab) where T : Component
		{
			var poolable = prefab.GetComponent<Poolable>();
			T instance = poolable != null && PoolManager.instanceExists ? 
				PoolManager.instance.GetPoolable(poolable).GetComponent<T>() : Instantiate(prefab).GetComponent<T>();
			return instance;
		}

		/// <summary>
		/// prefabがpool化可能ならpoolされたオブジェクトを返し、そうでなければ新しいオブジェクトを生成する
		/// </summary>
		/// <param name="prefab">必要なオブジェクトのprefab</param>
		/// <returns>poolから取得、または生成されたGameObject</returns>
		public static GameObject TryGetPoolable(GameObject prefab)
		{
			var poolable = prefab.GetComponent<Poolable>();
			GameObject instance = poolable != null && PoolManager.instanceExists ? 
				PoolManager.instance.GetPoolable(poolable).gameObject : Instantiate(prefab);
			return instance;
		}
	}
}