using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Towers.Placement
{
	/// <summary>
	/// Towerを配置できるエリア用のInterface
	/// </summary>
	public interface IPlacementArea
	{
		/// <summary>
		/// このオブジェクトのTransformを取得します
		/// </summary>
		Transform transform { get; }

		/// <summary>
		/// 指定したワールド位置から、特定サイズのオブジェクトが中央に来るようオフセットしたグリッド位置を計算します
		/// </summary>
		IntVector2 WorldToGrid(Vector3 worldPosition, IntVector2 sizeOffset);

		/// <summary>
		/// 指定したグリッド位置からスナップ後のワールド位置を計算します
		/// </summary>
		Vector3 GridToWorld(IntVector2 gridPosition, IntVector2 sizeOffset);

		/// <summary>
		/// 指定サイズのオブジェクトが、このグリッドの指定位置に収まるかを取得します
		/// </summary>
		/// <param name="gridPos">グリッド上の位置</param>
		/// <param name="size">アイテムのサイズ</param>
		/// <returns><paramref name="gridPos"/>にアイテムが収まる場合はtrue</returns>
		TowerFitStatus Fits(IntVector2 gridPos, IntVector2 size);

		/// <summary>
		/// この配置エリア上の指定範囲を占有します
		/// </summary>
		/// <param name="gridPos">グリッド上の位置</param>
		/// <param name="size">アイテムのサイズ</param>
		void Occupy(IntVector2 gridPos, IntVector2 size);

		/// <summary>
		/// この配置エリア上の指定範囲をクリアします
		/// </summary>
		/// <param name="gridPos">グリッド上の位置</param>
		/// <param name="size">アイテムのサイズ</param>
		void Clear(IntVector2 gridPos, IntVector2 size);
	}

	public static class PlacementAreaExtensions
	{
		/// <summary>
		/// 指定されたワールド位置をこのグリッドにスナップします
		/// </summary>
		public static Vector3 Snap(this IPlacementArea placementArea, Vector3 worldPosition, IntVector2 sizeOffset)
		{
			// 最も近いグリッド位置を計算し、それをワールド空間に戻します
			return placementArea.GridToWorld(placementArea.WorldToGrid(worldPosition, sizeOffset), sizeOffset);
		}
	}
}