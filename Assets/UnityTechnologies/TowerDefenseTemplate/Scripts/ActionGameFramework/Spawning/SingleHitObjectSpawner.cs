using UnityEngine;

namespace ActionGameFramework.Spawning
{
	/// <summary>
	/// 生成する GameObject を 1 つだけ提供する HitObjectSpawner の具象実装
	/// </summary>
	public class SingleHitObjectSpawner : HitObjectSpawner
	{
		public GameObject gameObjectToSpawn;

		protected override GameObject GetGameObjectToInstantiate()
		{
			return gameObjectToSpawn;
		}
	}
}
