namespace Core.Input
{
	/// <summary>
	/// ピンチ操作に関する情報
	/// </summary>
	public struct PinchInfo
	{
		/// <summary>
		/// ピンチ操作に使われている1つ目のタッチ
		/// </summary>
		public TouchInfo touch1;

		/// <summary>
		/// ピンチ操作に使われている2つ目のタッチ
		/// </summary>
		public TouchInfo touch2;
	}
}
