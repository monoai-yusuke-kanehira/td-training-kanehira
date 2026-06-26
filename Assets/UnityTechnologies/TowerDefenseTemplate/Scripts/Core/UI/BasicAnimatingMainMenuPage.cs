namespace Core.UI
{
	/// <summary>
	/// 最もシンプルなMainMenuPage。ページの有効化と無効化を即座に行う
	/// </summary>
	public class BasicAnimatingMainMenuPage : AnimatingMainMenuPage
	{
		/// <summary>
		/// BeginDeactivatingPageからすぐにFinishedDeactivatingPageを呼ぶ
		/// </summary>
		protected override void BeginDeactivatingPage()
		{
			FinishedDeactivatingPage();
		}

		/// <summary>
		/// ここでは何もしなくてよい
		/// </summary>
		protected override void FinishedActivatingPage()
		{
		}
	}
}
