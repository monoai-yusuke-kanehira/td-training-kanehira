namespace ActionGameFramework.Projectiles
{
	/// <summary>
	/// 弾道の放物線計算で使う優先度と設定
	/// </summary>
	public enum BallisticArcHeight
	{
		/// <summary>
		/// 高い「下手投げ」軌道
		/// </summary>
		UseHigh,

		/// <summary>
		/// 低い「上手投げ」軌道
		/// </summary>
		UseLow,

		/// <summary>
		/// 高い軌道が有効なら使い、可能なら低い軌道にフォールバックする
		/// </summary>
		PreferHigh,

		/// <summary>
		/// 低い軌道が有効なら使い、可能なら高い軌道にフォールバックする
		/// </summary>
		PreferLow
	}
}
