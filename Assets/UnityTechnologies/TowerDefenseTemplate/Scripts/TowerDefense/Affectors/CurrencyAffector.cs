using TowerDefense.Economy;
using TowerDefense.Level;
using UnityEngine;

namespace TowerDefense.Affectors
{
	/// <summary>
	/// 通貨を生成するタワー効果
	/// </summary>
	public class CurrencyAffector : Affector
	{
		/// <summary>
		/// 通貨獲得を制御するコントローラー
		/// </summary>
		public CurrencyGainer currencyGainer;

		/// <summary>
		/// このAffectorのプロパティ表示に使うフォーマット
		/// </summary>
		public string descriptionFormat = "<b>Produces</b> {1} at {2} units per second";

		/// <summary>
		/// アタッチされているAudioSource
		/// </summary>
		public AudioSource audioSource;

		/// <summary>
		/// アタッチされているParticleSystem
		/// </summary>
		public ParticleSystem currencyParticleSystem;


		/// <summary>
		/// 通貨獲得を初期化する
		/// </summary>
		protected virtual void Start()
		{
			currencyGainer.Initialize(LevelManager.instance.currency);
		}

		/// <summary>
		/// 通貨獲得を更新する
		/// </summary>
		protected virtual void Update()
		{
			currencyGainer.Tick(Time.deltaTime);
		}

		/// <summary>
		/// 通貨獲得イベントを購読する
		/// </summary>
		protected virtual void OnEnable()
		{
			currencyGainer.currencyChanged += OnCurrencyChanged;
		}

		/// <summary>
		/// 通貨獲得イベントの購読を解除する
		/// </summary>
		protected virtual void OnDisable()
		{
			currencyGainer.currencyChanged -= OnCurrencyChanged;
		}

		/// <summary>
		/// <see cref="currencyGainer"/>で通貨が変化したときに発火する
		/// </summary>
		/// <param name="info">
		/// CurrencyGainer用の情報
		/// </param>
		protected void OnCurrencyChanged(CurrencyChangeInfo info)
		{
			if (audioSource != null)
			{
				audioSource.Play();
			}
			if (currencyParticleSystem != null)
			{
				currencyParticleSystem.Play();
			}
		}
	}
}
