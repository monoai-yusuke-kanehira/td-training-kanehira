using TowerDefense.Level;
using TowerDefense.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// タワーデータを描画するUIオブジェクトを制御する
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class TowerUI : MonoBehaviour
	{
		/// <summary>
		/// 名前用のTextオブジェクト
		/// </summary>
		public Text towerName;

		/// <summary>
		/// 説明用のTextオブジェクト
		/// </summary>
		public Text description;
		
		public Text upgradeDescription;

		/// <summary>
		/// アタッチされている売却ボタン
		/// </summary>
		public Button sellButton;

		/// <summary>
		/// アタッチされているアップグレードボタン
		/// </summary>
		public Button upgradeButton;

		/// <summary>
		/// タワーの関連情報を表示するコンポーネント
		/// </summary>
		public TowerInfoDisplay towerInfoDisplay;

		public RectTransform panelRectTransform;

		public GameObject[] confirmationButtons;

		/// <summary>
		/// メインのゲームカメラ
		/// </summary>
		protected Camera m_GameCamera;

		/// <summary>
		/// 現在描画するタワー
		/// </summary>
		protected Tower m_Tower;

		/// <summary>
		/// gameObjectにアタッチされているCanvas
		/// </summary>
		protected Canvas m_Canvas;

		/// <summary>
		/// タワーデータをCanvasに描画する
		/// </summary>
		/// <param name="towerToShow">
		/// 情報を取得するタワー
		/// </param>
		public virtual void Show(Tower towerToShow)
		{
			if (towerToShow == null)
			{
				return;
			}
			m_Tower = towerToShow;
			AdjustPosition();

			m_Canvas.enabled = true;

			int sellValue = m_Tower.GetSellLevel();
			if (sellButton != null)
			{
				sellButton.gameObject.SetActive(sellValue > 0);
			}
			if (upgradeButton != null)
			{
				upgradeButton.interactable = 
					LevelManager.instance.currency.CanAfford(m_Tower.GetCostForNextLevel());
				bool maxLevel = m_Tower.isAtMaxLevel;
				upgradeButton.gameObject.SetActive(!maxLevel);
				if (!maxLevel)
				{
					upgradeDescription.text =
						m_Tower.levels[m_Tower.currentLevel + 1].upgradeDescription.ToUpper();
				}
			}
			LevelManager.instance.currency.currencyChanged += OnCurrencyChanged;
			towerInfoDisplay.Show(towerToShow);
			foreach (var button in confirmationButtons)
			{
				button.SetActive(false);
			}
		}

		/// <summary>
		/// タワー情報UIと範囲ビジュアライザーを非表示にする
		/// </summary>
		public virtual void Hide()
		{
			m_Tower = null;
			if (GameUI.instanceExists)
			{
				GameUI.instance.HideRadiusVisualizer();
			}
			m_Canvas.enabled = false;
			LevelManager.instance.currency.currencyChanged -= OnCurrencyChanged;
		}

		/// <summary>
		/// <see cref="GameUI"/> を通してタワーをアップグレードする
		/// </summary>
		public void UpgradeButtonClick()
		{
			GameUI.instance.UpgradeSelectedTower();
		}

		/// <summary>
		/// <see cref="GameUI"/> を通してタワーを売却する
		/// </summary>
		public void SellButtonClick()
		{
			GameUI.instance.SellSelectedTower();
		}

		/// <summary>
		/// ボタンにアタッチされているTextを取得する
		/// </summary>
		protected virtual void Awake()
		{
			m_Canvas = GetComponent<Canvas>();
		}

		/// <summary>
		/// タワーが選択または選択解除されたときに発火する
		/// </summary>
		/// <param name="newTower"></param>
		protected virtual void OnUISelectionChanged(Tower newTower)
		{
			if (newTower != null)
			{
				Show(newTower);
			}
			else
			{
				Hide();
			}
		}

		/// <summary>
		/// マウスボタン操作を購読する
		/// </summary>
		protected virtual void Start()
		{
			m_GameCamera = Camera.main;
			m_Canvas.enabled = false;
			if (GameUI.instanceExists)
			{
				GameUI.instance.selectionChanged += OnUISelectionChanged;
				GameUI.instance.stateChanged += OnGameUIStateChanged;
			}
		}

		/// <summary>
		/// カメラが移動したときに位置を調整する
		/// </summary>
		protected virtual void Update()
		{
			AdjustPosition();
		}

		/// <summary>
		/// currencyChangedの購読を解除する
		/// </summary>
		protected virtual void OnDisable()
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.currency.currencyChanged -= OnCurrencyChanged;
			}
		}

		/// <summary>
		/// UIの位置を調整する
		/// </summary>
		protected void AdjustPosition()
		{
			if (m_Tower == null)
			{
				return;
			}
			Vector3 point = m_GameCamera.WorldToScreenPoint(m_Tower.position);
			point.z = 0;
			panelRectTransform.transform.position = point;
		}

		/// <summary>
		/// <see cref="GameUI"/> の状態が変化したときに発火する
		/// 新しい状態が <see cref="GameUI.State.GameOver"/> の場合は <see cref="TowerUI"/> を非表示にする必要がある
		/// </summary>
		/// <param name="oldState">前の状態</param>
		/// <param name="newState">遷移先の状態</param>
		protected void OnGameUIStateChanged(GameUI.State oldState, GameUI.State newState)
		{
			if (newState == GameUI.State.GameOver)
			{
				Hide();
			}
		}

		/// <summary>
		/// 通貨が変化したとき、プレイヤーがアップグレード費用を支払えるか確認する
		/// </summary>
		void OnCurrencyChanged()
		{
			if (m_Tower != null && upgradeButton != null)
			{
				upgradeButton.interactable = 
					LevelManager.instance.currency.CanAfford(m_Tower.GetCostForNextLevel());
			}
		}

		/// <summary>
		/// GameUIのselectionChangedとstateChangedの購読を解除する
		/// </summary>
		void OnDestroy()
		{
			if (GameUI.instanceExists)
			{
				GameUI.instance.selectionChanged -= OnUISelectionChanged;
				GameUI.instance.stateChanged -= OnGameUIStateChanged;
			}
		}
	}
}
