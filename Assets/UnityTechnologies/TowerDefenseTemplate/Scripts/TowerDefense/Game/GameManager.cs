using Core.Data;
using Core.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Game
{
	/// <summary>
	/// ゲームマネージャー。永続化やレベルリストなどを処理する常駐オブジェクト
	/// ゲーム起動時に初期化する必要があります
	/// </summary>
	public class GameManager : GameManagerBase<GameManager, GameDataStore>
	{
		/// <summary>
		/// レベルのリストのスクリプト可能なオブジェクト
		/// </summary>
		public LevelList levelList;

		/// <summary>
		/// スリープしないようにスリープ タイムアウトを設定します
		/// </summary>
		protected override void Awake()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			base.Awake();
		}

		/// <summary>
		/// レベルを完了するために使用される方法
		/// </summary>
		/// <param name="levelId">完了としてマークする levelId</param>
		/// <param name="starsEarned"></param>
		public void CompleteLevel(string levelId, int starsEarned)
		{
			if (!levelList.ContainsKey(levelId))
			{
				Debug.LogWarningFormat("[GAME] Cannot complete level with id = {0}. Not in level list", levelId);
				return;
			}

			m_DataStore.CompleteLevel(levelId, starsEarned);
			SaveData();
		}

		/// <summary>
		/// 現在のレベルの ID を取得します
		/// </summary>
		public LevelItem GetLevelForCurrentScene()
		{
			string sceneName = SceneManager.GetActiveScene().name;

			return levelList.GetLevelByScene(sceneName);
		}

		/// <summary>
		/// 特定のレベルが完了したかどうかを判断します
		/// </summary>
		/// <param name="levelId">確認するレベルID</param>
		/// <returns>レベルが完了している場合は true</returns>
		public bool IsLevelCompleted(string levelId)
		{
			if (!levelList.ContainsKey(levelId))
			{
				Debug.LogWarningFormat("[GAME] Cannot check if level with id = {0} is completed. Not in level list", levelId);
				return false;
			}

			return m_DataStore.IsLevelCompleted(levelId);
		}

		/// <summary>
		/// 指定されたレベルで獲得したスターを取得します
		/// </summary>
		/// <param name="levelId"></param>
		/// <returns></returns>
		public int GetStarsForLevel(string levelId)
		{
			if (!levelList.ContainsKey(levelId))
			{
				Debug.LogWarningFormat("[GAME] Cannot check if level with id = {0} is completed. Not in level list", levelId);
				return 0;
			}

			return m_DataStore.GetNumberOfStarForLevel(levelId);
		}
	}
}
