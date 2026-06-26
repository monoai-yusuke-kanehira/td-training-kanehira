using UnityEngine;

namespace ActionGameFramework.Spawning
{
	/// <summary>
	/// 重み付きリストからランダムに HitObject を選ぶ HitObjectSpawner の実装
	/// </summary>
	public class RandomHitObjectSpawner : HitObjectSpawner
	{
		public WeightedObjectList objectList;

		protected override GameObject GetGameObjectToInstantiate()
		{
			return objectList.WeightedSelection();
		}
	}
}
