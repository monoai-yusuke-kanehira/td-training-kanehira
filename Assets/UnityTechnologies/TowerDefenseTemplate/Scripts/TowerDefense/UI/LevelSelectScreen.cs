using System.Collections.Generic;
using Core.Game;
using Core.UI;
using TowerDefense.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// レベル選択ユーザーインターフェースのマネージャー
	/// </summary>
	public class LevelSelectScreen : SimpleMainMenuPage
	{
		/// <summary>
		/// レベル選択ボタンとして
		/// インスタンス化するボタン
		/// </summary>
		public LevelSelectButton selectionPrefab;

		/// <summary>
		/// ボタンをインスタンス化する先のLayoutGroup
		/// </summary>
		public LayoutGroup layout;

		/// <summary>
		/// レベルパネル用のバッファ
		/// </summary>
		public Transform rightBuffer;

		public Button backButton;

		public MouseScroll mouseScroll;

		public Animation cameraAnimator;

		public string enterCameraAnim;
		
		public string exitCameraAnim;

		/// <summary>
		/// 表示するレベル一覧への参照
		/// </summary>
		protected LevelList m_LevelList;
		
		protected List<Button> m_Buttons = new List<Button>();

		/// <summary>
		/// ボタンをインスタンス化する
		/// </summary>
		protected virtual void Start()
		{
			if (GameManager.instance == null)
			{
				return;
			}

			m_LevelList = GameManager.instance.levelList;
			if (layout == null || selectionPrefab == null || m_LevelList == null)
			{
				return;
			}

			int amount = m_LevelList.Count;
			for (int i = 0; i < amount; i++)
			{
				LevelSelectButton button = CreateButton(m_LevelList[i]);
				button.transform.SetParent(layout.transform);
				button.transform.localScale = Vector3.one;
				m_Buttons.Add(button.GetComponent<Button>());
			}
			if (rightBuffer != null)
			{
				rightBuffer.SetAsLastSibling();
			}

			for (int index = 1; index < m_Buttons.Count - 1; index++)
			{
				Button button = m_Buttons[index];
				SetUpNavigation(button, m_Buttons[index - 1], m_Buttons[index + 1]);
			}
			

			SetUpNavigation(m_Buttons[0], backButton, m_Buttons[1]);
			SetUpNavigation(m_Buttons[m_Buttons.Count - 1], m_Buttons[m_Buttons.Count - 2], null);
			
			mouseScroll.SetHasRightBuffer(rightBuffer != null);
		}

		/// <summary>
		/// itemに基づいてレベル選択ボタンを作成し、初期化する
		/// </summary>
		/// <param name="item">
		/// レベルデータ
		/// </param>
		/// <returns>
		/// 初期化済みのボタン
		/// </returns>
		protected LevelSelectButton CreateButton(LevelItem item)
		{
			LevelSelectButton button = Instantiate(selectionPrefab);
			button.Initialize(item, mouseScroll);
			return button;
		}

		/// <summary>
		/// カメラアニメーションを再生する
		/// </summary>
		public override void Show()
		{
			base.Show();

			if (cameraAnimator != null && enterCameraAnim != null)
			{
				cameraAnimator.Play(enterCameraAnim);
			}
		}

		/// <summary>
		/// カメラを通常位置に戻す
		/// </summary>
		public override void Hide()
		{
			base.Hide();

			if (cameraAnimator != null && exitCameraAnim != null)
			{
				cameraAnimator.Play(exitCameraAnim);
			}
		}

		/// <summary>
		/// Selectableのナビゲーションを設定する
		/// </summary>
		/// <param name="selectable">設定対象のSelectable</param>
		/// <param name="left">左側で選択する対象</param>
		/// <param name="right">右側で選択する対象</param>
		void SetUpNavigation(Selectable selectable, Selectable left, Selectable right)
		{
			Navigation navigation = selectable.navigation;
			navigation.selectOnLeft = left;
			navigation.selectOnRight = right;
			selectable.navigation = navigation;
		}
		
	}
}
