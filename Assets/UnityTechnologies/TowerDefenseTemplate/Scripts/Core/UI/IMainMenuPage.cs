namespace Core.UI
{
	/// <summary>
	/// メニューページの基底インターフェース
	/// </summary>
	public interface IMainMenuPage
	{
		/// <summary>
		/// このページを非表示にする
		/// </summary>
		void Hide();

		/// <summary>
		/// このページを表示する
		/// </summary>
		void Show();
	}
}
