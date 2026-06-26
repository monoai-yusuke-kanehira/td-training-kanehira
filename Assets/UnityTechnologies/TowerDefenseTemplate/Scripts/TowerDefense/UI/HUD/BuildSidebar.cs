using TowerDefense.Level;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// このレベルで建設できるタワーを表示するUIコンポーネント。
	/// </summary>
	public class BuildSidebar : MonoBehaviour
	{
		/// <summary>
		/// 各ボタンとして生成されるPrefab
		/// </summary>
		public TowerSpawnButton towerSpawnButton;

		/// <summary>
		/// タワー生成ボタンを初期化する
		/// </summary>
		protected virtual void Start()
		{
			if (!LevelManager.instanceExists)
			{
				Debug.LogError("[UI] No level manager for tower list");
			}
			foreach (Tower tower in LevelManager.instance.towerLibrary)
			{
				TowerSpawnButton button = Instantiate(towerSpawnButton, transform);
				button.InitializeButton(tower);
				button.buttonTapped += OnButtonTapped;
				button.draggedOff += OnButtonDraggedOff;
			}
		}

		/// <summary>
		/// <see cref="towerData"/> を使ってGameUIをビルドモードに設定する
		/// </summary>
		/// <param name="towerData"></param>
		void OnButtonTapped(Tower towerData)
		{
			var gameUI = GameUI.instance;
			if (gameUI.isBuilding)
			{
				gameUI.CancelGhostPlacement();
			}
			gameUI.SetToBuildMode(towerData);
		}

		/// <summary>
		/// <see cref="towerData"/> を使ってGameUIをビルドモードに設定する
		/// </summary>
		/// <param name="towerData"></param>
		void OnButtonDraggedOff(Tower towerData)
		{
			if (!GameUI.instance.isBuilding)
			{
				GameUI.instance.SetToDragMode(towerData);
			}
		}

		/// <summary>
		/// すべてのタワー生成ボタンから購読を解除する
		/// </summary>
		void OnDestroy()
		{
			TowerSpawnButton[] childButtons = GetComponentsInChildren<TowerSpawnButton>();

			foreach (TowerSpawnButton towerButton in childButtons)
			{
				towerButton.buttonTapped -= OnButtonTapped;
				towerButton.draggedOff -= OnButtonDraggedOff;
			}
		}

		/// <summary>
		/// シーン内のウェーブ開始ボタンから呼ばれる
		/// </summary>
		public void StartWaveButtonPressed()
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.BuildingCompleted();
			}
		}

		/// <summary>
		/// 通貨を追加するデバッグ用ボタン
		/// </summary>
		/// <param name="amount">追加する量</param>
		public void AddCurrency(int amount)
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.currency.AddCurrency(amount);
			}
		}
	}
}
