using Core.Economy;
using TowerDefense.Level;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// 通貨表示を制御するクラス
	/// </summary>
	public class CurrencyUI : MonoBehaviour
	{
		/// <summary>
		/// 情報を表示するText要素
		/// </summary>
		public Text display;

		/// <summary>
		/// 金額の横に表示する通貨の接頭辞
		/// </summary>
		public string currencySymbol = "$";

		protected Currency m_Currency;

		/// <summary>
		/// 正しい通貨値を割り当てる
		/// </summary>
		protected virtual void Start()
		{
			if (LevelManager.instance != null)
			{
				m_Currency = LevelManager.instance.currency;

				UpdateDisplay();
				m_Currency.currencyChanged += UpdateDisplay;
			}
			else
			{
				Debug.LogError("[UI] No level manager to get currency from");
			}
		}

		/// <summary>
		/// イベントの購読を解除する
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (m_Currency != null)
			{
				m_Currency.currencyChanged -= UpdateDisplay;
			}
		}

		/// <summary>
		/// 現在の通貨に基づいて表示を更新するメソッド
		/// </summary>
		protected void UpdateDisplay()
		{
			int current = m_Currency.currentCurrency;
			display.text = current.ToString();
		}
	}
}
