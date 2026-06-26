using UnityEngine;

namespace Core.Input
{
	/// <summary>
	/// 待機中のポインター入力の情報を管理するクラス
	/// </summary>
	public abstract class PointerInfo
	{
		/// <summary>
		/// 現在のポインター位置
		/// </summary>
		public Vector2 currentPosition;

		/// <summary>
		/// 前フレームのポインター位置
		/// </summary>
		public Vector2 previousPosition;

		/// <summary>
		/// このフレームでの移動量
		/// </summary>
		public Vector2 delta;

		/// <summary>
		/// このポインター入力がUI上で始まったかを記録する
		/// </summary>
		public bool startedOverUI;
	}
}
