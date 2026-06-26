using UnityEngine;

namespace TowerDefense.Economy
{
	/// <summary>
	/// 為替データを保持するための構造体
	/// </summary>
	public struct CurrencyChangeInfo
	{
		/// <summary>
		/// 通貨の以前の価値
		/// </summary>
		public readonly int previousCurrency;

		/// <summary>
		/// 通貨の新しい価値
		/// </summary>
		public readonly int currentCurrency;

		/// <summary>
		/// 金額の差額
		/// </summary>
		public readonly int difference;

		/// <summary>
		/// 金額の差の絶対値を取得します
		/// </summary>
		public readonly int absoluteDifference;

		/// <summary>
		/// CurrencyChangeInfoを初期化します
		/// </summary>
		/// <param name="previous">
		/// 通貨の以前の価値
		/// </param>
		/// <param name="current">
		/// 通貨の現在の価値
		/// </param>
		public CurrencyChangeInfo(int previous, int current)
		{
			previousCurrency = previous;
			currentCurrency = current;
			difference = currentCurrency - previousCurrency;
			absoluteDifference = Mathf.Abs(difference);
		}
	}
}