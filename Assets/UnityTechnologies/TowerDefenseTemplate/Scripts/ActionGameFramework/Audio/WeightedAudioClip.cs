using System;
using UnityEngine;

namespace ActionGameFramework.Audio
{
	/// <summary>
	/// 重み付き AudioClip。
	/// 個別のクリップが選ばれやすくなるように確率を上げるために使う
	/// </summary>
	[Serializable]
	public class WeightedAudioClip
	{
		/// <summary>
		/// 再生対象の AudioClip
		/// </summary>
		public AudioClip clip;

		/// <summary>
		/// 重み。個別のクリップが選ばれる確率を高くするために使う
		/// </summary>
		public int weight = 1;
	}
}
