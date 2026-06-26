using System;
using Core.Economy;
using TowerDefense.Level;
using TowerDefense.Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// タワーを生成するためのボタンコントローラー
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class TowerSpawnButton : MonoBehaviour, IDragHandler
	{
		/// <summary>
		/// ボタンにアタッチされているText
		/// </summary>
		public Text buttonText;

		public Image towerIcon;

		public Button buyButton;

		public Image energyIcon;

		public Color energyDefaultColor;
		
		public Color energyInvalidColor;

		/// <summary>
		/// ボタンがタップされたときに発火する
		/// </summary>
		public event Action<Tower> buttonTapped;

		/// <summary>
		/// ポインターがボタン範囲の外にあり、
		/// まだ押下中のときに発火する
		/// </summary>
		public event Action<Tower> draggedOff;
		
		/// <summary>
		/// ボタンの内容を定義するタワーコントローラー
		/// </summary>
		Tower m_Tower;

		/// <summary>
		/// レベルの通貨へのキャッシュ済み参照
		/// </summary>
		Currency m_Currency;

		/// <summary>
		/// アタッチされているRectTransform
		/// </summary>
		RectTransform m_RectTransform;

		/// <summary>
		/// ポインターが範囲外にあるか確認し、
		/// draggedOffイベントを発火する
		/// </summary>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!RectTransformUtility.RectangleContainsScreenPoint(m_RectTransform, eventData.position))
			{
				if (draggedOff != null)
				{
					draggedOff(m_Tower);
				}
			}
		}

		/// <summary>
		/// タワー用のボタン情報を定義する
		/// </summary>
		/// <param name="towerData">
		/// ボタンの初期化に使用するタワー
		/// </param>
		public void InitializeButton(Tower towerData)
		{
			m_Tower = towerData;

			if (towerData.levels.Length > 0)
			{
				TowerLevel firstTower = towerData.levels[0];
				buttonText.text = firstTower.cost.ToString();
				towerIcon.sprite = firstTower.levelData.icon;
			}
			else
			{
				Debug.LogWarning("[Tower Spawn Button] No level data for tower");
			}

			if (LevelManager.instanceExists)
			{
				m_Currency = LevelManager.instance.currency;
				m_Currency.currencyChanged += UpdateButton;
			}
			else
			{
				Debug.LogWarning("[Tower Spawn Button] No level manager to get currency object");
			}
			UpdateButton();
		}

		/// <summary>
		/// RectTransformをキャッシュする
		/// </summary>
		protected virtual void Awake()
		{
			m_RectTransform = (RectTransform) transform;
		}

		/// <summary>
		/// イベントの購読を解除する
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (m_Currency != null)
			{
				m_Currency.currencyChanged -= UpdateButton;
			}
		}

		/// <summary>
		/// ボタンがタップされたときのクリック処理
		/// </summary>
		public void OnClick()
		{
			if (buttonTapped != null)
			{
				buttonTapped(m_Tower);
			}
		}

		/// <summary>
		/// コストに基づいてボタンの状態を更新する
		/// </summary>
		void UpdateButton()
		{
			if (m_Currency == null)
			{
				return;
			}

			// ボタンを有効にする
			if (m_Currency.CanAfford(m_Tower.purchaseCost) && !buyButton.interactable)
			{
				buyButton.interactable = true;
				energyIcon.color = energyDefaultColor;
			}
			else if (!m_Currency.CanAfford(m_Tower.purchaseCost) && buyButton.interactable)
			{
				buyButton.interactable = false;
				energyIcon.color = energyInvalidColor;
			}
		}
	}
}
