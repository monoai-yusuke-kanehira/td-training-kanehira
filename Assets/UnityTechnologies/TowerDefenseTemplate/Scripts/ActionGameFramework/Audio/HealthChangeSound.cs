using System;
using UnityEngine;

namespace ActionGameFramework.Audio
{
	/// <summary>
	/// 体力変化量と AudioClip を対応付けるサウンド設定
	/// </summary>
	[Serializable]
	public class HealthChangeSound
	{
		[Tooltip("Health Change should be in ascending order")]
		public float healthChange;

		public AudioClip sound;
	}
}
