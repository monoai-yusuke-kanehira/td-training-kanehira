using TowerDefense.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// Unity UIを使ってタワーの情報を表示するために使用する
	/// </summary>
	public class TowerInfoDisplay : MonoBehaviour
	{
		/// <summary>
		/// 名前用のTextコンポーネント
		/// </summary>
		public Text towerName;

		/// <summary>
		/// 説明用のTextコンポーネント
		/// </summary>
		public Text description;

		/// <summary>
		/// DPS用のTextコンポーネント
		/// </summary>
		public Text dps;

		/// <summary>
		/// レベル用のTextコンポーネント
		/// </summary>
		public Text level;

		/// <summary>
		/// 体力用のTextコンポーネント
		/// </summary>
		public Text health;

		/// <summary>
		/// サイズ用のTextコンポーネント
		/// </summary>
		public Text dimensions;

		/// <summary>
		/// アップグレード費用用のTextコンポーネント
		/// </summary>
		public Text upgradeCost;

		/// <summary>
		/// 売却価格用のTextコンポーネント
		/// </summary>
		public Text sellPrice;

		/// <summary>
		/// 関連するTextコンポーネントが設定されている場合、タワーデータをCanvasに描画する
		/// </summary>
		/// <param name="tower">
		/// 情報を取得するタワー
		/// </param>
		public void Show(Tower tower)
		{
			int levelOfTower = tower.currentLevel;
			Show(tower, levelOfTower);
		}

		/// <summary>
		/// 関連するTextコンポーネントが設定されている場合、タワーデータをCanvasに描画する
		/// </summary>
		/// <param name="tower">情報を取得するタワー</param>
		/// <param name="levelOfTower">タワーのレベル</param>
		public void Show(Tower tower, int levelOfTower)
		{
			if (levelOfTower >= tower.levels.Length)
			{
				return;
			}
			TowerLevel towerLevel = tower.levels[levelOfTower];
			DisplayText(towerName, tower.towerName);
			DisplayText(description, towerLevel.description);
			DisplayText(dps, towerLevel.GetTowerDps().ToString("f2"));
			DisplayText(health, string.Format("{0}/{1}", tower.configuration.currentHealth, towerLevel.maxHealth));
			DisplayText(level, (levelOfTower + 1).ToString());
			DisplayText(dimensions, string.Format("{0}, {1}", tower.dimensions.x, tower.dimensions.y));
			if (levelOfTower + 1 < tower.levels.Length)
			{
				DisplayText(upgradeCost, tower.levels[levelOfTower + 1].cost.ToString());
			}

			int sellValue = tower.GetSellLevel(levelOfTower);
			DisplayText(sellPrice, sellValue.ToString());
		}

		/// <summary>
		/// Textコンポーネントが設定されている場合、テキストを描画する
		/// </summary>
		/// <param name="textBox"></param>
		/// <param name="text"></param>
		static void DisplayText(Text textBox, string text)
		{
			if (textBox != null)
			{
				textBox.text = text;
			}
		}
	}
}
