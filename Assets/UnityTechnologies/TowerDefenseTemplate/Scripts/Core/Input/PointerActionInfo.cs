using UnityEngine;

namespace Core.Input
{
	/// <summary>
	/// アクティブなポインター入力の情報を管理するクラス
	/// </summary>
	public class PointerActionInfo : PointerInfo
	{
		/// <summary>
		/// 入力が始まった位置
		/// </summary>
		public Vector2 startPosition;

		/// <summary>
		/// フリック速度は移動量の移動平均
		/// </summary>
		public Vector2 flickVelocity;

		/// <summary>
		/// 押されてからの、このポインターの合計移動量
		/// </summary>
		public float totalMovement;

		/// <summary>
		/// ホールドが始まった時刻
		/// </summary>
		public float startTime;

		/// <summary>
		/// この入力がドラッグされたか
		/// </summary>
		public bool isDrag;

		/// <summary>
		/// この入力がホールド中か
		/// </summary>
		public bool isHold;

		/// <summary>
		/// この入力がホールド後にドラッグされたか
		/// </summary>
		public bool wasHold;
	}
}
