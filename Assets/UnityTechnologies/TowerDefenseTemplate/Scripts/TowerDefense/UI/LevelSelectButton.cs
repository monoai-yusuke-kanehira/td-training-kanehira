using Core.Game;
using TowerDefense.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// レベルを選択するためのボタン
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class LevelSelectButton : MonoBehaviour, ISelectHandler
	{
		/// <summary>
		/// 必要なButtonコンポーネントへの参照
		/// </summary>
		protected Button m_Button;

		/// <summary>
		/// レベル名を表示するUIのText要素
		/// </summary>
		public Text titleDisplay;
		
		public Text description;

		public Sprite starAchieved;

		public Image[] stars;

		protected MouseScroll m_MouseScroll;

		/// <summary>
		/// このボタンが表示するレベルに関するデータ
		/// </summary>
		protected LevelItem m_Item;

		/// <summary>
		/// ユーザーがボタンをクリックしたらシーンを変更する
		/// </summary>
		public void ButtonClicked()
		{
			ChangeScenes();
		}

		/// <summary>
		/// itemのデータをボタンに割り当てるメソッド
		/// </summary>
		/// <param name="item">
		/// レベルに関する情報を持つデータ
		/// </param>
		public void Initialize(LevelItem item, MouseScroll mouseScroll)
		{
			LazyLoad();
			if (titleDisplay == null)
			{
				return;
			}
			m_Item = item;
			titleDisplay.text = item.name;
			description.text = item.description;
			HasPlayedState();
			m_MouseScroll = mouseScroll;
		}

		/// <summary>
		/// プレイヤーがプレイ済みかどうかに関するフィードバックを設定する
		/// </summary>
		protected void HasPlayedState()
		{
			GameManager gameManager = GameManager.instance;
			if (gameManager == null)
			{
				return;
			}
			int starsForLevel = gameManager.GetStarsForLevel(m_Item.id);
			for (int i = 0; i < starsForLevel; i++)
			{
				stars[i].sprite = starAchieved;
			}
		}

		/// <summary>
		/// m_Itemで指定されたシーン名へシーンを変更する
		/// </summary>
		protected void ChangeScenes()
		{
			SceneManager.LoadScene(m_Item.sceneName);
		}

		/// <summary>
		/// <see cref="m_Button"/> がnullでないことを保証する
		/// </summary>
		protected void LazyLoad()
		{
			if (m_Button == null)
			{
				m_Button = GetComponent<Button>();
			}
		}

		/// <summary>
		/// 破棄前にボタンのすべてのリスナーを削除する
		/// </summary>
		protected void OnDestroy()
		{
			if (m_Button != null)
			{
				m_Button.onClick.RemoveAllListeners();
			}
		}

		/// <summary>
		/// ISelectHandlerの実装
		/// </summary>
		/// <param name="eventData">選択イベントのデータ</param>
		public void OnSelect(BaseEventData eventData)
		{
			m_MouseScroll.SelectChild(this);
		}
	}
}
