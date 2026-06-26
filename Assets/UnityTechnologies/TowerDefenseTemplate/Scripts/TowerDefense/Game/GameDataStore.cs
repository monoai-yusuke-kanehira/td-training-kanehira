using System.Collections.Generic;
using Core.Data;
using UnityEngine;

namespace TowerDefense.Game
{
	/// <summary>
	/// TD のデータ ストア
	/// </summary>
	public sealed class GameDataStore : GameDataStoreBase
	{
		/// <summary>
		/// 完了したレベルのレベル ID のリスト
		/// </summary>
		public List<LevelSaveData> completedLevels = new List<LevelSaveData>();

		/// <summary>
		/// デバッグする出力
		/// </summary>
		public override void PreSave()
		{
			Debug.Log("[GAME] Saving Game");
		}

		/// <summary>
		/// デバッグする出力
		/// </summary>
		public override void PostLoad()
		{
			Debug.Log("[GAME] Loaded Game");
		}

		/// <summary>
		/// レベルを完了としてマークします
		/// </summary>
		/// <param name="levelId">完了としてマークする levelId</param>
		/// <param name="starsEarned">Stars earned</param>
		public void CompleteLevel(string levelId, int starsEarned)
		{
			foreach (LevelSaveData level in completedLevels)
			{
				if (level.id == levelId)
				{
					level.numberOfStars = Mathf.Max(level.numberOfStars, starsEarned);
					return;
				}
			}
			completedLevels.Add(new LevelSaveData(levelId, starsEarned));
		}

		/// <summary>
		/// 特定のレベルが完了したかどうかを判断します
		/// </summary>
		/// <param name="levelId">確認するレベルID</param>
		/// <returns>レベルが完了している場合は true</returns>
		public bool IsLevelCompleted(string levelId)
		{
			foreach (LevelSaveData level in completedLevels)
			{
				if (level.id == levelId)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 指定されたレベルのスター数を取得します
		/// </summary>
		public int GetNumberOfStarForLevel(string levelId)
		{
			foreach (LevelSaveData level in completedLevels)
			{
				if (level.id == levelId)
				{
					return level.numberOfStars;
				}
			}
			return 0;
		}
	}
}