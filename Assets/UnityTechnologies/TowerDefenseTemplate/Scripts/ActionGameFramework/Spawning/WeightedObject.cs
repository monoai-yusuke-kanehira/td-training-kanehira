using System;
using UnityEngine;

namespace ActionGameFramework.Spawning
{
	/// <summary>
	/// 重み付き HitObject。
	/// 個別のオブジェクトが選ばれやすくなるように確率を上げるために使う
	/// </summary>
	[Serializable]
	public class WeightedObject
	{
		/// <summary>
		/// 対象の GameObject
		/// </summary>
		public GameObject gameObject;

		/// <summary>
		/// 重み。個別のオブジェクトが選ばれる確率を高くするために使う
		/// </summary>
		public int weight = 1;
	}
}
