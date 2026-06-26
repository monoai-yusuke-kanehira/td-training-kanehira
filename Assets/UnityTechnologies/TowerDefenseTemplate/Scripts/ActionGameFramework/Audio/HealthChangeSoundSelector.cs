using System;
using System.Collections.Generic;
using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Audio
{
	/// <summary>
	/// 体力変化用のサウンドセレクター
	/// </summary>
	[Serializable]
	public class HealthChangeSoundSelector
	{
		/// <summary>
		/// 体力変化サウンドの配列
		/// この配列は体力差の昇順に並べる必要がある
		/// </summary>
		[Tooltip("Health change should be in ascending order")]
		public List<HealthChangeSound> healthChangeSounds;

		/// <summary>
		/// この <see cref="ActionGameFramework.Audio.HealthChangeSoundSelector" /> が設定済みかどうかを取得する
		/// </summary>
		/// <value>体力変化サウンドがある場合は <c>true</c>、ない場合は <c>false</c>。</value>
		public bool isSetUp
		{
			get { return healthChangeSounds.Count > 0; }
		}

		/// <summary>
		/// 体力変化情報からクリップを取得する
		/// </summary>
		/// <returns>体力変化情報に対応するクリップ。</returns>
		/// <param name="info">HealthChangeInfo</param>
		public virtual AudioClip GetClipFromHealthChangeInfo(HealthChangeInfo info)
		{
			int count = healthChangeSounds.Count;

			for (int i = 0; i < count; i++)
			{
				HealthChangeSound sound = healthChangeSounds[i];

				// 体力変化の絶対値がサウンド側の体力変化量以下なら
				// このサウンドクリップを使用する
				if (info.absHealthDifference <= sound.healthChange)
				{
					return sound.sound;
				}
			}

			Debug.LogFormat("Could not find sound for healthChange of {0}", info.absHealthDifference);
			return null;
		}
	}
}
