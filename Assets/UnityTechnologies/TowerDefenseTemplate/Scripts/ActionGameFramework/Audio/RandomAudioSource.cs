using UnityEngine;

namespace ActionGameFramework.Audio
{
	/// <summary>
	/// ランダムな AudioClip を再生するためのヘルパー
	/// ランダム性は均等ではなく、重みに基づく
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class RandomAudioSource : MonoBehaviour
	{
		/// <summary>
		/// AudioClip の重み付きリスト
		/// </summary>
		public WeightedAudioList clips;

		/// <summary>
		/// OnEnable 時にランダムでサウンドを再生するための設定
		/// </summary>
		public bool playOnEnabled;

		/// <summary>
		/// アタッチされている AudioSource
		/// </summary>
		protected AudioSource m_Source;

		/// <summary>
		/// AudioSource をキャッシュし、必要なら再生する
		/// </summary>
		protected virtual void OnEnable()
		{
			if (m_Source == null)
			{
				m_Source = GetComponent<AudioSource>();
			}
			if (playOnEnabled)
			{
				PlayRandomClip();
			}
		}

		/// <summary>
		/// アタッチされている AudioSource を使ってランダムなクリップを再生する
		/// </summary>
		public virtual void PlayRandomClip()
		{
			if (m_Source == null)
			{
				m_Source = GetComponent<AudioSource>();
			}
			PlayRandomClip(m_Source);
		}

		/// <summary>
		/// 指定した AudioSource を使ってランダムなクリップを再生する
		/// </summary>
		/// <param name="source">使用する AudioSource</param>
		public virtual void PlayRandomClip(AudioSource source)
		{
			if (source == null)
			{
				Debug.LogError("[RANDOM AUDIO SOURCE] Missing audio source");
				return;
			}

			AudioClip clip = clips.WeightedSelection();
			if (clip == null)
			{
				Debug.LogError("[RANDOM AUDIO SOURCE] Missing audio clips");
				return;
			}

			source.clip = clip;
			source.Play();
		}
	}
}
