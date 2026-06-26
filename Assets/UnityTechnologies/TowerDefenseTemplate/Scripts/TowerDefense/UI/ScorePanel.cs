using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// 最終スコアを表示するUIオブジェクト
	/// </summary>
	public class ScorePanel : MonoBehaviour
	{
		/// <summary>
		/// 星を表すオブジェクト
		/// </summary>
		public Image[] starImages;

		public Sprite achievedStarSprite;

		/// <summary>
		/// スコアに応じた正しい数の星を表示する
		/// </summary>
		/// <param name="score">最終スコア</param>
		public void SetStars(int score)
		{
			if (score <= 0)
			{
				return;
			}
			score = Mathf.Clamp(score, 0, starImages.Length);
			for (int i = 0; i < score; i++)
			{
				starImages[i].sprite = achievedStarSprite;
			}
		}
	}
}
