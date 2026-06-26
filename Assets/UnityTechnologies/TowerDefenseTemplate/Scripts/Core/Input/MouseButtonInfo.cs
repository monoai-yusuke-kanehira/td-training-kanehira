namespace Core.Input
{
	/// <summary>
	/// マウス情報
	/// </summary>
	public class MouseButtonInfo : PointerActionInfo
	{
		/// <summary>
		/// このマウスボタンが押されているかどうか
		/// </summary>
		public bool isDown;

		/// <summary>
		/// このマウスボタンのID
		/// </summary>
		public int mouseButtonId;
	}
}