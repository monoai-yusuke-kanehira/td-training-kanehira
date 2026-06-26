using System;

namespace TowerDefense.Game
{
	/// <summary>
	/// レベルデータを保存するためのコール
	/// </summary>
	[Serializable]
	public class LevelSaveData
	{
		public string id;
		public int numberOfStars;

		public LevelSaveData(string levelId, int numberOfStarsEarned)
		{
			id = levelId;
			numberOfStars = numberOfStarsEarned;
		}
	}
}