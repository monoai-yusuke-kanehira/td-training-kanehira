using UnityEngine;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// 配置タイルが取り得る状態
	/// </summary>
	public enum PlacementTileState
	{
		Filled,
		Empty
	}
	
	/// <summary>
	/// タイルの配置場所を示すためのシンプルなクラス
	/// </summary>
	public class PlacementTile : MonoBehaviour
	{
		/// <summary>
		/// このタイルが空のときに使用するMaterial
		/// </summary>
		public Material emptyMaterial;
		/// <summary>
		/// このタイルが埋まっているときに使用するMaterial
		/// </summary>
		public Material filledMaterial;
		/// <summary>
		/// Materialを変更するRenderer
		/// </summary>
		public Renderer tileRenderer;

		/// <summary>
		/// この配置タイルの状態を更新する
		/// </summary>
		public void SetState(PlacementTileState newState)
		{
			switch (newState)
			{
				case PlacementTileState.Filled:
					if (tileRenderer != null && filledMaterial != null)
					{
						tileRenderer.sharedMaterial = filledMaterial;
					}
					break;
				case PlacementTileState.Empty:
					if (tileRenderer != null && emptyMaterial != null)
					{
						tileRenderer.sharedMaterial = emptyMaterial;
					}
					break;
			}
		}
	}
}
