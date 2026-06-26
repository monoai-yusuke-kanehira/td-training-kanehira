using System;

namespace Core.Economy
{
	/// <summary>
	/// ゲーム内通貨の基本モデル
	/// </summary>
	public class Currency
	{
		/// <summary>
		/// 現在所持している通貨量
		/// </summary>
		public int currentCurrency { get; private set; }

		/// <summary>
		/// 通貨が変更されたときに発生します。
		/// </summary>
		public event Action currencyChanged;

		/// <summary>
		/// <see cref="Core.Economy.Currency" /> クラスの新しいインスタンスを初期化します。
		/// </summary>
		public Currency(int startingCurrency)
		{
			ChangeCurrency(startingCurrency);
		}

		/// <summary>
		/// 通貨を追加します。
		/// </summary>
		/// <param name="increment">通貨の増減量</param>
		public void AddCurrency(int increment)
		{
			ChangeCurrency(increment);
		}

		/// <summary>
		/// 購入を試みるメソッドです。資金が足りない場合はfalseを返します
		/// </summary>
		/// <returns>通貨が足りていて購入できた場合は <c>true</c>、それ以外の場合は <c>false</c></returns>
		public bool TryPurchase(int cost)
		{
			// このアイテムを購入できません
			if (!CanAfford(cost))
			{
				return false;
			}
			ChangeCurrency(-cost);
			return true;
		}

		/// <summary>
		/// 指定したコストを支払えるかどうかを判定します。
		/// </summary>
		/// <returns>このコストを支払える場合は <c>true</c>、それ以外の場合は <c>false</c></returns>
		public bool CanAfford(int cost)
		{
			return currentCurrency >= cost;
		}

		/// <summary>
		/// 通貨を変更します。
		/// </summary>
		/// <param name="increment">通貨の増減量</param>
		protected void ChangeCurrency(int increment)
		{
			if (increment != 0)
			{
				currentCurrency += increment;
				if (currencyChanged != null)
				{
					currencyChanged();
				}
			}
		}
	}
}
