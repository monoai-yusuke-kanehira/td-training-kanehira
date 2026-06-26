
using Core.Game;
using Core.Health;
using TowerDefense.Game;
using TowerDefense.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// ゲームオーバー画面を表示するUI
	/// </summary>
	public class EndGameScreen : MonoBehaviour
	{
		/// <summary>
		/// 勝利時に再生するAudioClip
		/// </summary>
		public AudioClip victorySound;

		/// <summary>
		/// 敗北時に再生するAudioClip
		/// </summary>
		public AudioClip defeatSound;

		/// <summary>
		/// サウンドを再生するAudioSource
		/// </summary>
		public AudioSource audioSource;

		/// <summary>
		/// 終了画面UIを含むパネル
		/// </summary>
		public Canvas endGameCanvas;

		/// <summary>
		/// 結果メッセージを表示するTextオブジェクトへの参照
		/// </summary>
		public Text endGameMessageText;

		/// <summary>
		/// 最終的な星評価を表示するパネル
		/// </summary>
		public ScorePanel scorePanel;

		/// <summary>
		/// レベル選択画面の名前
		/// </summary>
		public string menuSceneName = "MainMenu";

		/// <summary>
		/// ポップアップに表示するテキスト
		/// </summary>
		public string levelCompleteText = "{0} COMPLETE!";
		
		public string levelFailedText = "{0} FAILED!";

		/// <summary>
		/// 背景画像
		/// </summary>
		public Image background;

		/// <summary>
		/// 背景に設定する色
		/// </summary>
		public Color winBackgroundColor;
		
		public Color loseBackgroundColor;

		/// <summary>
		/// プレイヤーがレベルをクリアしたときに、
		/// 次のレベルへ進むボタンを保持するCanvas
		/// </summary>
		public Canvas nextLevelButton;

		/// <summary>
		/// <see cref="LevelManager" /> への参照
		/// </summary>
		protected LevelManager m_LevelManager;

		/// <summary>
		/// <see cref="LevelManager" /> のイベント購読を安全に解除する。
		/// メインメニューシーンへ戻る
		/// </summary>
		public void GoToMainMenu()
		{
			SafelyUnsubscribe();
			SceneManager.LoadScene(menuSceneName);
		}

		/// <summary>
		/// <see cref="LevelManager" /> のイベント購読を安全に解除する。
		/// アクティブなシーンを再読み込みする
		/// </summary>
		public void RestartLevel()
		{
			SafelyUnsubscribe();
			string currentSceneName = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene(currentSceneName);
		}

		/// <summary>
		/// <see cref="LevelManager" /> のイベント購読を安全に解除する。
		/// 有効な場合は次のシーンへ進む
		/// </summary>
		public void GoToNextLevel()
		{
			SafelyUnsubscribe();
			if (!GameManager.instanceExists)
			{
				return;
			}
			GameManager gm = GameManager.instance;
			LevelItem item = gm.GetLevelForCurrentScene();
			LevelList list = gm.levelList;
			int levelCount = list.Count;
			int index = -1;
			for (int i = 0; i < levelCount; i++)
			{
				if (item == list[i])
				{
					index = i + 1;
					break;
				}
			}
			if (index < 0 || index >= levelCount)
			{
				return;
			}
			LevelItem nextLevel = gm.levelList[index];
			SceneManager.LoadScene(nextLevel.sceneName);
		}

		/// <summary>
		/// 開始時にパネルがアクティブなら非表示にする。
		/// <see cref="LevelManager" /> の完了/失敗イベントを購読する。
		/// </summary>
		protected void Start()
		{
			LazyLoad();
			endGameCanvas.enabled = false;
			nextLevelButton.enabled = false;
			nextLevelButton.gameObject.SetActive(false);

			m_LevelManager.levelCompleted += Victory;
			m_LevelManager.levelFailed += Defeat;
		}

		/// <summary>
		/// 終了画面を表示する
		/// </summary>
		protected void OpenEndGameScreen(string endResultText)
		{
			LevelItem level = GameManager.instance.GetLevelForCurrentScene();
			endGameCanvas.enabled = true;

			int score = CalculateFinalScore();
			scorePanel.SetStars(score);
			if (level != null) 
			{
				endGameMessageText.text = string.Format (endResultText, level.name.ToUpper ());
				GameManager.instance.CompleteLevel (level.id, score);
			} 
			else 
			{
				// レベルがLevelListにない場合は、シーン名をそのまま使用する。この場合、レベルのスコアは保存されない。
				string levelName = SceneManager.GetActiveScene ().name;
				endGameMessageText.text = string.Format (endResultText, levelName.ToUpper ());
			}


			if (!HUD.GameUI.instanceExists)
			{
				return;
			}
			if (HUD.GameUI.instance.state == HUD.GameUI.State.Building)
			{
				HUD.GameUI.instance.CancelGhostPlacement();
			}
			HUD.GameUI.instance.GameOver();
		}

		/// <summary>
		/// レベルを正常にクリアしたときに発生する
		/// </summary>
		protected void Victory()
		{
			OpenEndGameScreen(levelCompleteText);
			if ((victorySound != null) && (audioSource != null))
			{
				audioSource.PlayOneShot(victorySound);
			}
			background.color = winBackgroundColor;

			// まず、このレベルの後にさらにレベルがあるか確認する
			if (nextLevelButton == null || !GameManager.instanceExists)
			{
				return;
			}
			GameManager gm = GameManager.instance;
			LevelItem item = gm.GetLevelForCurrentScene();
			LevelList list = gm.levelList;
			int levelCount = list.Count;
			int index = -1;
			for (int i = 0; i < levelCount; i++)
			{
				if (item == list[i])
				{
					index = i;
					break;
				}
			}
			// レベルが存在しない、またはこれが最後のレベルの場合は
			// 次のレベルボタンを非表示にする
			if (index < 0 || index == levelCount - 1)
			{
				nextLevelButton.enabled = false;
				nextLevelButton.gameObject.SetActive(false);
				return;
			}
			nextLevelButton.enabled = true;
			nextLevelButton.gameObject.SetActive(true);
		}

		/// <summary>
		/// レベルに失敗したときに発生する
		/// </summary>
		protected void Defeat()
		{
			OpenEndGameScreen(levelFailedText);
			if (nextLevelButton != null)
			{
				nextLevelButton.enabled = false;
				nextLevelButton.gameObject.SetActive(false);
			}
			if ((defeatSound != null) && (audioSource != null))
			{
				audioSource.PlayOneShot(defeatSound);
			}
			background.color = loseBackgroundColor;
		}

		/// <summary>
		/// <see cref="LevelManager" /> のイベント購読を安全に解除する。
		/// </summary>
		protected void OnDestroy()
		{
			SafelyUnsubscribe();
			if (HUD.GameUI.instanceExists)
			{
				HUD.GameUI.instance.Unpause();
			}
		}

		/// <summary>
		/// 必要なときに <see cref="LevelManager" /> のイベント購読が解除されるようにする
		/// </summary>
		protected void SafelyUnsubscribe()
		{
			LazyLoad();
			m_LevelManager.levelCompleted -= Victory;
			m_LevelManager.levelFailed -= Defeat;
		}

		/// <summary>
		/// <see cref="m_LevelManager" /> がnullでないことを保証する
		/// </summary>
		protected void LazyLoad()
		{
			if ((m_LevelManager == null) && LevelManager.instanceExists)
			{
				m_LevelManager = LevelManager.instance;
			}
		}

		/// <summary>
		/// すべてのHome Baseの体力を合計し、スコアを返す
		/// </summary>
		/// <returns>最終スコア</returns>
		protected int CalculateFinalScore()
		{
			int homeBaseCount = m_LevelManager.numberOfHomeBases;
			PlayerHomeBase[] homeBases = m_LevelManager.playerHomeBases;

			float totalRemainingHealth = 0f;
			float totalBaseHealth = 0f;
			for (int i = 0; i < homeBaseCount; i++)
			{
				Damageable config = homeBases[i].configuration;
				totalRemainingHealth += config.currentHealth;
				totalBaseHealth += config.maxHealth;
			}
			int score = CalculateScore(totalRemainingHealth, totalBaseHealth);
			return score;
		}

		/// <summary>
		/// すべての拠点の最終残り体力をもとに評価する
		/// </summary>
		/// <param name="remainingHealth">すべてのHome Baseの合計残り体力</param>
		/// <param name="maxHealth">すべてのHome Baseの合計最大体力</param>
		/// <returns>残り体力に応じた0から3の値</returns>
		protected int CalculateScore(float remainingHealth, float maxHealth)
		{
			float normalizedHealth = remainingHealth / maxHealth;
			if (Mathf.Approximately(normalizedHealth, 1f))
			{
				return 3;
			}
			if ((normalizedHealth <= 0.9f) && (normalizedHealth >= 0.5f))
			{
				return 2;
			}
			if ((normalizedHealth < 0.5f) && (normalizedHealth > 0f))
			{
				return 1;
			}
			return 0;
		}
	}
}
