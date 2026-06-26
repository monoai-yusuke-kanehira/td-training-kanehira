namespace TowerDefense.Towers.Placement
{
	/// <summary>
	/// Towerが配置エリアに収まる状態を表すEnum
	/// </summary>
	public enum TowerFitStatus
	{
		/// <summary>
		/// この位置にTowerを配置できます
		/// </summary>
		Fits,

		/// <summary>
		/// 配置エリア内の別のTowerと重なっています
		/// </summary>
		Overlaps,

		/// <summary>
		/// Towerが配置エリアの範囲を超えています
		/// </summary>
		OutOfBounds
	}
}