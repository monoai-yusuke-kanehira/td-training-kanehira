using System.Collections.Generic;
using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Audio
{
	/// <summary>
	/// 体力変化時にサウンドを再生するためのヘルパー
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class HealthChangeAudioSource : MonoBehaviour
	{
		/// <summary>
		/// サウンドセレクター。HealthChangeInfo に基づいて再生するサウンドを選ぶ仕組み
		/// </summary>
		public HealthChangeSoundSelector soundSelector;

		/// <summary>
		/// サウンド再生に使う AudioSource
		/// </summary>
		protected AudioSource m_Source;

		/// <summary>
		/// 実行時に必要な AudioSource 参照を割り当てる
		/// </summary>
		protected virtual void Awake()
		{
			m_Source = GetComponent<AudioSource>();
		}

		/// <summary>
		/// AudioSource を再生する
		/// </summary>
		public virtual void PlaySound()
		{
			m_Source.Play();
		}

		/// <summary>
		/// 指定された体力変化の条件を満たしたときにクリップを再生する
		/// </summary>
		/// <param name="info">再生するクリップを決めるために使う <see cref="HealthChangeInfo"/></param>
		public virtual void PlayHealthChangeSound(HealthChangeInfo info)
		{
			if (soundSelector != null && soundSelector.isSetUp)
			{
				AudioClip newClip = soundSelector.GetClipFromHealthChangeInfo(info);
				if (newClip != null)
				{
					m_Source.clip = newClip;
				}
			}

			m_Source.Play();
		}

		/// <summary>
		/// <see cref="soundSelector"/> のサウンドリストを並べ替える
		/// </summary>
		public void Sort()
		{
			if (soundSelector.healthChangeSounds == null || soundSelector.healthChangeSounds.Count <= 0)
			{
				return;
			}
			soundSelector.healthChangeSounds.Sort(new HealthChangeSoundComparer());
		}
	}

	/// <summary>
	/// 2 つの <see cref="HealthChangeSound"/> を比較する方法を提供する
	/// </summary>
	public class HealthChangeSoundComparer : IComparer<HealthChangeSound>
	{
		/// <summary>
		/// 2 つの <see cref="HealthChangeSound"/> を比較する
		/// </summary>
		public int Compare(HealthChangeSound first, HealthChangeSound second)
		{
			if (first.healthChange == second.healthChange)
			{
				return 0;
			}
			if (first.healthChange < second.healthChange)
			{
				return -1;
			}

			return 1;
		}
	}
}
