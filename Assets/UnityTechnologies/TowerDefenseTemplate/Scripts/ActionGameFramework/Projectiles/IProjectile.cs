using System;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
	/// <summary>
	/// 汎用的な Projectile を定義するためのインターフェース
	/// </summary>
	public interface IProjectile
	{
		/// <summary>
		/// この Projectile が発射されたときに発火するイベント
		/// </summary>
		event Action fired;
		
		/// <summary>
		/// 指定した開始位置から指定したワールド座標へ向けてこの Projectile を発射する
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="targetPoint">飛行先の目標位置。</param>
		void FireAtPoint(Vector3 startPoint, Vector3 targetPoint);

		/// <summary>
		/// 指定した方向へこの Projectile を発射する
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="fireVector">飛行方向を表す Vector。</param>
		void FireInDirection(Vector3 startPoint, Vector3 fireVector);

		/// <summary>
		/// 指定した初速でこの Projectile を発射し、既存の開始速度を上書きする
		/// </summary>
		/// <param name="startPoint">飛行の開始位置。</param>
		/// <param name="fireVelocity">発射速度を表す Vector3。</param>
		void FireAtVelocity(Vector3 startPoint, Vector3 fireVelocity);
	}
}
