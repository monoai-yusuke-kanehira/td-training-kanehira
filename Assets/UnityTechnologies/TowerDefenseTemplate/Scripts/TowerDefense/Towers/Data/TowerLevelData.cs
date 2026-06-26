using UnityEngine;

namespace TowerDefense.Towers.Data
{
	/// <summary>
	/// Towerレベルごとの設定を保持するデータコンテナ
	/// </summary>
	[CreateAssetMenu(fileName = "TowerData.asset", menuName = "TowerDefense/Tower Configuration", order = 1)]
	public class TowerLevelData : ScriptableObject
	{
		/// <summary>
		/// UIに表示するTowerの説明
		/// </summary>
		public string description;

		/// <summary>
		/// UIに表示するTowerの説明
		/// </summary>
		public string upgradeDescription;

		/// <summary>
		/// このレベルへアップグレードするコスト
		/// </summary>
		public int cost;

		/// <summary>
		/// Towerの売却コスト
		/// </summary>
		public int sell;

		/// <summary>
		/// 最大体力
		/// </summary>
		public int maxHealth;

		/// <summary>
		/// 開始時の体力
		/// </summary>
		public int startingHealth;

		/// <summary>
		/// Towerのアイコン
		/// </summary>
		public Sprite icon;
	}
}