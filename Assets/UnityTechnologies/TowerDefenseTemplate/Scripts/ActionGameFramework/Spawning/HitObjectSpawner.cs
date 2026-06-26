using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Spawning
{
	/// <summary>
	/// ヒット情報に基づいて GameObject を生成する public メソッドを提供する Spawner
	/// 生成された GameObject には、ヒット情報を受け取る HitObject コンポーネントが付いている場合がある
	/// </summary>
	public abstract class HitObjectSpawner : MonoBehaviour
	{
		/// <summary>
		/// 生成する GameObject を取得する
		/// 生成対象の GameObject を選ぶ仕組みを差し替えられるようにするために必要
		/// </summary>
		/// <returns>生成する GameObject。</returns>
		protected abstract GameObject GetGameObjectToInstantiate();

		/// <summary>
		/// HitObject を生成する public メソッド。DamageableListener 側のメソッドから呼び出せる
		/// </summary>
		/// <param name="hitInfo">ヒット情報。</param>
		public virtual void InstantiateHitObject(HitInfo hitInfo)
		{
			GameObject gameObjectToInstantiate = GetGameObjectToInstantiate();
			GameObject gameObjectInstance = Instantiate(gameObjectToInstantiate, hitInfo.damagePoint, Quaternion.identity);
			HitObject[] hitObjects = gameObjectInstance.GetComponentsInChildren<HitObject>();
			int length = hitObjects.Length;
			for (int i = 0; i < length; i++)
			{
				HitObject hitObject = hitObjects[i];
				hitObject.SetHitInfo(hitInfo);
			}
		}
	}
}
