using System;
using Core.Economy;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Economy
{
	/// <summary>
	/// 通貨獲得のためのクラス
	/// </summary>
	[Serializable]
	public class CurrencyGainer
	{
		/// <summary>
		/// ゲインレートで得られる金額
		/// </summary>
		public int constantCurrencyAddition;

		/// <summary>
		/// 通貨上昇の速度 (単位/秒)
		/// </summary>
		[Header("The Gain Rate in additions-per-second")]
		public float constantCurrencyGainRate;

		/// <summary>
		/// 通貨変更時のイベント
		/// </summary>
		public event Action<CurrencyChangeInfo> currencyChanged;

		/// <summary>
		/// 一定の通貨利益を得るタイマー
		/// </summary>
		protected RepeatingTimer m_GainTimer;

		/// <summary>
		/// この CurrencyGainer が変更する通貨を取得します
		/// </summary>
		public Currency currency { get; private set; }

		/// <summary>
		/// 新しいデータで通貨ゲインを初期化します
		/// </summary>
		/// <param name="currencyController">
		/// この通貨獲得者で変更する通貨コントローラ
		/// </param>
		/// <param name="gainAddition">
		/// 追加するたびに得られる通貨
		/// </param>
		/// <param name="gainRate">
		/// 増加率
		/// </param>
		public void Initialize(Currency currencyController, int gainAddition, float gainRate)
		{
			constantCurrencyAddition = gainAddition;
			constantCurrencyGainRate = gainRate;
			Initialize(currencyController);
		}

		/// <summary>
		/// 通貨ゲインを初期化します
		/// </summary>
		public void Initialize(Currency currencyController)
		{
			currency = currencyController;
			UpdateGainRate(constantCurrencyGainRate);
		}

		/// <summary>
		/// ゲインタイマー更新用
		/// </summary>
		/// <param name="deltaTime">
		/// タイマー更新時間の変更
		/// </param>
		public void Tick(float deltaTime)
		{
			if (m_GainTimer == null)
			{
				return;
			}
			m_GainTimer.Tick(Time.deltaTime);
		}

		/// <summary>
		/// 通貨のゲインレートを設定し、タイマーをアクティブにします
		/// </summary>
		/// <param name="currencyGainRate">
		/// コンスタントゲインレートを設定する量
		/// </param>
		public void UpdateGainRate(float currencyGainRate)
		{
			constantCurrencyGainRate = currencyGainRate;
			if (currencyGainRate < 0)
			{
				throw new ArgumentOutOfRangeException("currencyGainRate");
			}
			if (m_GainTimer == null)
			{
				m_GainTimer = new RepeatingTimer(1 / constantCurrencyGainRate, ConstantGain);
			}
			else
			{
				m_GainTimer.SetTime(1 / constantCurrencyGainRate);
			}
		}

		/// <summary>
		/// m_ConstantCurrencyAddition で通貨を増やします
		/// </summary>
		protected void ConstantGain()
		{
			int previousCurrency = currency.currentCurrency;
			currency.AddCurrency(constantCurrencyAddition);
			int currentCurrency = currency.currentCurrency;
			var info = new CurrencyChangeInfo(previousCurrency, currentCurrency);
			if (currencyChanged != null)
			{
				currencyChanged(info);
			}
		}
	}
}