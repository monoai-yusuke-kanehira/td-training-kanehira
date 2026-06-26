using System;
using Core.Health;
using Core.Input;
using Core.Utilities;
using JetBrains.Annotations;
using TowerDefense.Level;
using TowerDefense.Towers;
using TowerDefense.Towers.Placement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// Raycast情報も持つポインター用のゲームUIラッパー
	/// </summary>
	public struct UIPointer
	{
		/// <summary>
		/// ポインター情報
		/// </summary>
		public PointerInfo pointer;

		/// <summary>
		/// このポインターのRay
		/// </summary>
		public Ray ray;

		/// <summary>
		/// 3DシーンへのRaycastでヒットしたオブジェクト
		/// </summary>
		public RaycastHit? raycast;

		/// <summary>
		/// このポインターがUI要素、またはイベントシステムが検出するものの上で開始された場合はtrue
		/// </summary>
		public bool overUI;
	}

	/// <summary>
	/// ゲーム内でのユーザー操作を管理するオブジェクト。主な役割は次のとおり
	/// <list type="bullet">
	///     <item>
	///         <description>タワーを建設する</description>
	///     </item>
	///     <item>
	///         <description>タワーとユニットを選択する</description>
	///     </item>
	/// </list>
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class GameUI : Singleton<GameUI>
	{
		/// <summary>
		/// UIが取り得る状態
		/// </summary>
		public enum State
		{
			/// <summary>
			/// ゲームの通常状態。この状態では、プレイヤーはカメラ移動、ユニットやタワーの選択ができる
			/// </summary>
			Normal,

			/// <summary>
			/// ゲームが「建設モード」の状態。この状態では、プレイヤーはカメラ移動、配置の確定またはキャンセルができる
			/// </summary>
			Building,

			/// <summary>
			/// ゲームが一時停止中の状態。この状態では、プレイヤーはレベルの再開始、またはメインメニューへの終了ができる
			/// </summary>
			Paused,

			/// <summary>
			/// ゲームが終了し、レベルが失敗または完了した状態
			/// </summary>
			GameOver,
			
			/// <summary>
			/// ゲームが「建設モード」で、プレイヤーがゴーストタワーをドラッグしている状態
			/// </summary>
			BuildingWithDrag
		}

		/// <summary>
		/// 現在のUI状態を取得する
		/// </summary>
		public State state { get; private set; }

		/// <summary>
		/// 現在選択中のタワー
		/// </summary>
		public LayerMask placementAreaMask;

		/// <summary>
		/// タワー選択用のレイヤー
		/// </summary>
		public LayerMask towerSelectionLayer;

		/// <summary>
		/// 配置が無効なときに、ゴーストをワールド上で動かすための物理レイヤー
		/// </summary>
		public LayerMask ghostWorldPlacementMask;

		/// <summary>
		/// ゴースト配置のチェックに使うSphereCastの半径
		/// </summary>
		public float sphereCastRadius = 1;

		/// <summary>
		/// ゴーストとタワーの射程表示を管理するコンポーネント
		/// </summary>
		public RadiusVisualizerController radiusVisualizerController;

		/// <summary>
		/// 個別のタワーデータを表示するUIコントローラー
		/// </summary>
		public TowerUI towerUI;

		/// <summary>
		/// 配置中にタワー情報を表示するUIコントローラー
		/// </summary>
		public BuildInfoUI buildInfoUI;

		/// <summary>
		/// <see cref="State"/> が変わったときに発火する
		/// TouchUIが使われているときだけ発火できるようにする
		/// </summary>
		public event Action<State, State> stateChanged;

		/// <summary>
		/// 所持通貨の変化により、以前は無効だったゴーストが有効になったときに発火する
		/// </summary>
		public event Action ghostBecameValid;

		/// <summary>
		/// タワーが選択または選択解除されたときに発火する
		/// </summary>
		public event Action<Tower> selectionChanged;

		/// <summary>
		/// ゴーストタワーが現在乗っている配置エリア
		/// </summary>
		IPlacementArea m_CurrentArea;

		/// <summary>
		/// ゴーストタワーが現在乗っているグリッド位置
		/// </summary>
		IntVector2 m_GridPosition;

		/// <summary>
		/// キャッシュ済みのCamera参照
		/// </summary>
		Camera m_Camera;

		/// <summary>
		/// 現在のタワーのプレースホルダー。<see cref="State.Building" /> 状態でない場合はnullになる
		/// </summary>
		TowerPlacementGhost m_CurrentTower;

		/// <summary>
		/// ゴーストが有効な位置にあり、プレイヤーが購入できるかどうかを管理する
		/// </summary>
		bool m_GhostPlacementPossible;

		/// <summary>
		/// 現在選択中のタワーを取得する
		/// </summary>
		public Tower currentSelectedTower { get; private set; }

		/// <summary>
		/// タワーが選択されているかどうかを取得する
		/// </summary>
		public bool isTowerSelected
		{
			get { return currentSelectedTower != null; }
		}

		/// <summary>
		/// 特定の建設操作が有効かどうかを取得する
		/// </summary>
		public bool isBuilding
		{
			get
			{
				return state == State.Building || state == State.BuildingWithDrag;
			}
		}

		/// <summary>
		/// ゴーストの配置をキャンセルする
		/// </summary>
		public void CancelGhostPlacement()
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Can't cancel out of ghost placement when not in the building state.");
			}

			if (buildInfoUI != null)
			{
				buildInfoUI.Hide();
			}
			Destroy(m_CurrentTower.gameObject);
			m_CurrentTower = null;
			SetState(State.Normal);
			DeselectTower();
		}

		/// <summary>
		/// 現在のタワーを使ってGameUIをドラッグモードに戻す
		/// </summary>
		/// /// <exception cref="InvalidOperationException">
		/// 建設モードでない場合に例外を投げる
		/// </exception>
		public void ChangeToDragMode()
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Trying to return to Build With Dragging Mode when not in Build Mode");
			}
			SetState(State.BuildingWithDrag);
		}

		/// <summary>
		/// 現在のタワーを使ってGameUIを建設モードに戻す
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// ドラッグモードでない場合に例外を投げる
		/// </exception>
		public void ReturnToBuildMode()
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Trying to return to Build Mode when not in Drag Mode");
			}
			SetState(State.Building);
		}

		
		/// <summary>
		/// 状態を変更し、<see cref="stateChanged"/> を発火する
		/// </summary>
		/// <param name="newState">変更先の状態</param>
		/// <exception cref="ArgumentOutOfRangeException">無効な状態の場合に投げられる</exception>
		void SetState(State newState)
		{
			if (state == newState)
			{
				return;
			}
			State oldState = state;
			if (oldState == State.Paused || oldState == State.GameOver)
			{
				Time.timeScale = 1f;
			}

			switch (newState)
			{
				case State.Normal:
					break;
				case State.Building:
					break;
				case State.BuildingWithDrag:
					break;
				case State.Paused:
				case State.GameOver:
					if (oldState == State.Building)
					{
						CancelGhostPlacement();
					}
					Time.timeScale = 0f;
					break;
				default:
					throw new ArgumentOutOfRangeException("newState", newState, null);
			}
			state = newState;
			if (stateChanged != null)
			{
				stateChanged(oldState, state);
			}
		}

		/// <summary>
		/// ゲーム終了時に呼び出される
		/// </summary>
		public void GameOver()
		{
			SetState(State.GameOver);
		}

		/// <summary>
		/// ゲームを一時停止し、ポーズメニューを表示する
		/// </summary>
		public void Pause()
		{
			SetState(State.Paused);
		}

		/// <summary>
		/// ゲームを再開し、ポーズメニューを閉じる
		/// </summary>
		public void Unpause()
		{
			SetState(State.Normal);
		}
		
		/// <summary>
		/// モードをドラッグに変更する
		/// </summary>
		/// <param name="towerToBuild">
		/// 建設するタワー
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// 通常モードでないときにドラッグモードへ変更しようとした場合に例外を投げる
		/// </exception>
		public void SetToDragMode([NotNull] Tower towerToBuild)
		{
			if (state != State.Normal)
			{
				throw new InvalidOperationException("Trying to enter drag mode when not in Normal mode");	
			}
			
			if (m_CurrentTower != null)
			{
				// 現在のゴーストを破棄する
				CancelGhostPlacement();
			}
			SetUpGhostTower(towerToBuild);
			SetState(State.BuildingWithDrag);
		}

		/// <summary>
		/// 指定されたタワー用にUIを建設状態にする
		/// </summary>
		/// <param name="towerToBuild">
		/// 建設するタワー
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// 通常モードでないときに建設モードへ入ろうとした場合に例外を投げる
		/// </exception>
		public void SetToBuildMode([NotNull] Tower towerToBuild)
		{
			if (state != State.Normal)
			{
				throw new InvalidOperationException("Trying to enter Build mode when not in Normal mode");
			}
			
			if (m_CurrentTower != null)
			{
				// 現在のゴーストを破棄する
				CancelGhostPlacement();
			}
			SetUpGhostTower(towerToBuild);
			SetState(State.Building);
		}

		/// <summary>
		/// 指定された位置へのタワー配置を試みる
		/// </summary>
		/// <param name="pointerInfo">タワーの配置に使うポインター</param>
		public void TryPlaceTower(PointerInfo pointerInfo)
		{
			UIPointer pointer = WrapPointer(pointerInfo);

			// UIの上にある場合は何もしない
			if (pointer.overUI)
			{
				return;
			}
			BuyTower(pointer);
		}

		/// <summary>
		/// 指定されたポインター位置にゴーストタワーを配置する
		/// </summary>
		/// <param name="pointerInfo">タワーの配置に使うポインター</param>
		/// <param name="hideWhenInvalid">無効な位置にあるときにゴーストを非表示にするかどうかを設定する任意パラメーター</param>
		public void TryMoveGhost(PointerInfo pointerInfo, bool hideWhenInvalid = true)
		{
			if (m_CurrentTower == null)
			{
				throw new InvalidOperationException("Trying to move the tower ghost when we don't have one");
			}

			UIPointer pointer = WrapPointer(pointerInfo);
			// UIの上にある場合は何もしない
			if (pointer.overUI && hideWhenInvalid)
			{
				m_CurrentTower.Hide();
				return;
			}
			MoveGhost(pointer, hideWhenInvalid);
		}

		/// <summary>
		/// タワーまたはゴーストタワー用の射程表示を設定する
		/// </summary>
		public void SetupRadiusVisualizer(Tower tower, Transform ghost = null)
		{
			radiusVisualizerController.SetupRadiusVisualizers(tower, ghost);
		}

		/// <summary>
		/// 射程表示を非表示にする
		/// </summary>
		public void HideRadiusVisualizer()
		{
			radiusVisualizerController.HideRadiusVisualizers();
		}

		/// <summary>
		/// 指定された情報でタワーコントローラーUIを有効にする
		/// </summary>
		/// <param name="tower">
		/// 使用するタワーコントローラー情報
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// <see cref="State" /> が <see cref="State.Normal" /> でないときにタワーを選択すると例外を投げる
		/// </exception>
		public void SelectTower(Tower tower)
		{
			if (state != State.Normal)
			{
				throw new InvalidOperationException("Trying to select whilst not in a normal state");
			}
			DeselectTower();
			currentSelectedTower = tower;
			if (currentSelectedTower != null)
			{
				currentSelectedTower.removed += OnTowerDied;
			}
			radiusVisualizerController.SetupRadiusVisualizers(tower);

			if (selectionChanged != null)
			{
				selectionChanged(tower);
			}
		}

		/// <summary>
		/// 可能であれば <see cref="currentSelectedTower" /> をアップグレードする
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// <see cref="State" /> が <see cref="State.Normal" /> でないとき、または
		/// <see cref="currentSelectedTower" /> がnullのときにタワーを選択すると例外を投げる
		/// </exception>
		public void UpgradeSelectedTower()
		{
			if (state != State.Normal)
			{
				throw new InvalidOperationException("Trying to upgrade whilst not in Normal state");
			}
			if (currentSelectedTower == null)
			{
				throw new InvalidOperationException("Selected Tower is null");
			}
			if (currentSelectedTower.isAtMaxLevel)
			{
				return;
			}
			int upgradeCost = currentSelectedTower.GetCostForNextLevel();
			bool successfulUpgrade = LevelManager.instance.currency.TryPurchase(upgradeCost);
			if (successfulUpgrade)
			{
				currentSelectedTower.UpgradeTower();
			}
			towerUI.Hide();
			DeselectTower();
		}

		/// <summary>
		/// 可能であれば <see cref="currentSelectedTower" /> を売却する
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// <see cref="State" /> が <see cref="State.Normal" /> でないとき、または
		/// <see cref="currentSelectedTower" /> がnullのときにタワーを選択すると例外を投げる
		/// </exception>
		public void SellSelectedTower()
		{
			if (state != State.Normal)
			{
				throw new InvalidOperationException("Trying to sell tower whilst not in Normal state");
			}
			if (currentSelectedTower == null)
			{
				throw new InvalidOperationException("Selected Tower is null");
			}
			int sellValue = currentSelectedTower.GetSellLevel();
			if (LevelManager.instanceExists && sellValue > 0)
			{
				LevelManager.instance.currency.AddCurrency(sellValue);
				currentSelectedTower.Sell();
			}
			DeselectTower();
		}

		/// <summary>
		/// タワーを購入し、現在の位置に配置する
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// 建設モードでないときにタワーを購入しようとすると例外を投げる
		/// </exception>
		public void BuyTower()
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Trying to buy towers when not in Build Mode");
			}
			if (m_CurrentTower == null || !IsGhostAtValidPosition())
			{
				return;
			}
			int cost = m_CurrentTower.controller.purchaseCost;
			bool successfulPurchase = LevelManager.instance.currency.TryPurchase(cost);
			if (successfulPurchase)
			{
				PlaceTower();
			}
		}

		/// <summary>
		/// 建設フェーズ中にタワーを購入するときに使う
		/// 所持通貨を確認し、<see cref="PlaceGhost" /> を呼び出す
		/// <exception cref="InvalidOperationException">
		/// 建設モードでない場合、またはタワーが有効な位置にない場合に例外を投げる
		/// </exception>
		/// </summary>
		public void BuyTower(UIPointer pointer)
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Trying to buy towers when not in a Build Mode");
			}
			if (m_CurrentTower == null || !IsGhostAtValidPosition())
			{
				return;
			}
			PlacementAreaRaycast(ref pointer);
			if (!pointer.raycast.HasValue || pointer.raycast.Value.collider == null)
			{
				CancelGhostPlacement();
				return;
			}
			int cost = m_CurrentTower.controller.purchaseCost;
			bool successfulPurchase = LevelManager.instance.currency.TryPurchase(cost);
			if (successfulPurchase)
			{
				PlaceGhost(pointer);
			}
		}

		/// <summary>
		/// 現在のタワーの選択を解除し、UIを非表示にする
		/// </summary>
		public void DeselectTower()
		{
			if (state != State.Normal)
			{
				throw new InvalidOperationException("Trying to deselect tower whilst not in Normal state");
			}
			if (currentSelectedTower != null)
			{
				currentSelectedTower.removed -= OnTowerDied;
			}

			currentSelectedTower = null;

			if (selectionChanged != null)
			{
				selectionChanged(null);
			}
		}

		/// <summary>
		/// <see cref="m_CurrentArea"/> 上にある
		/// <see cref="m_CurrentTower"/> の位置を確認する
		/// </summary>
		/// <returns>
		/// 配置が有効な場合はtrue
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// <see cref="State.Normal"/> 状態でチェックした場合に例外を投げる
		/// </exception>
		public bool IsGhostAtValidPosition()
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Trying to check ghost position when not in a build mode");
			}
			if (m_CurrentTower == null)
			{
				return false;
			}
			if (m_CurrentArea == null)
			{
				return false;
			}
			TowerFitStatus fits = m_CurrentArea.Fits(m_GridPosition, m_CurrentTower.controller.dimensions);
			return fits == TowerFitStatus.Fits;
		}

		/// <summary>
		/// ゴーストタワーを購入できるかどうかを確認する
		/// </summary>
		/// <returns>
		/// 購入できる場合はtrue
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// 建設モードまたはドラッグ中の建設モードでない場合に例外を投げる
		/// </exception>
		public bool IsValidPurchase()
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Trying to check ghost position when not in a build mode");
			}
			if (m_CurrentTower == null)
			{
				return false;
			}
			if (m_CurrentArea == null)
			{
				return false;
			}
			return LevelManager.instance.currency.CanAfford(m_CurrentTower.controller.purchaseCost);
		}

		/// <summary>
		/// ゴーストタワーがある場所にタワーを配置する
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// 建設状態でない場合、または <see cref="m_CurrentTower"/> が有効な位置にない場合に例外を投げる
		/// </exception>
		public void PlaceTower()
		{
			if ( !isBuilding )
			{
				throw new InvalidOperationException("Trying to place tower when not in a Build Mode");
			}
			if (!IsGhostAtValidPosition())
			{
				throw new InvalidOperationException("Trying to place tower on an invalid area");
			}
			if (m_CurrentArea == null)
			{
				return;
			}
			Tower createdTower = Instantiate(m_CurrentTower.controller);
			createdTower.Initialize(m_CurrentArea, m_GridPosition);

			CancelGhostPlacement();
		}

		/// <summary>
		/// 指定されたポインターが現在のタワーゴーストの上にあるかどうかを計算する
		/// </summary>
		/// <param name="pointerInfo">
		/// <see cref="m_CurrentTower"/> との判定に使う情報
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// 建設モードでない場合に例外を投げる
		/// </exception>
		public bool IsPointerOverGhost(PointerInfo pointerInfo)
		{
			if (state != State.Building)
			{
				throw new InvalidOperationException("Trying to tap on ghost tower when not in Build Mode");
			}
			UIPointer uiPointer = WrapPointer(pointerInfo);
			RaycastHit hit;
			return m_CurrentTower.ghostCollider.Raycast(uiPointer.ray, out hit, float.MaxValue);
		}

		/// <summary>
		/// 指定されたポインターの下にタワーがあれば選択する
		/// </summary>
		/// <param name="info">
		/// ポインターの選択判定に関するポインター情報
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// <see cref="State.Normal"/> でない場合に例外を投げる
		/// </exception>
		public void TrySelectTower(PointerInfo info)
		{
			if (state != State.Normal)
			{
				throw new InvalidOperationException("Trying to select towers outside of Normal state");
			}
			UIPointer uiPointer = WrapPointer(info);
			RaycastHit output;
			bool hasHit = Physics.Raycast(uiPointer.ray, out output, float.MaxValue, towerSelectionLayer);
			if (!hasHit || uiPointer.overUI)
			{
				return;
			}
			var controller = output.collider.GetComponent<Tower>();
			if (controller != null)
			{
				SelectTower(controller);
			}
		}

		/// <summary>
		/// ゴーストタワーのワールド座標を取得する
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// 建設モードでない場合、または
		/// ゴーストタワーが存在しない場合に例外を投げる
		/// </exception>
		public Vector3 GetGhostPosition()
		{
			if (!isBuilding)
			{
				throw new InvalidOperationException("Trying to get ghost position when not in a Build Mode");
			}
			if (m_CurrentTower == null)
			{
				throw new InvalidOperationException("Trying to get ghost position for an object that does not exist");
			}
			return m_CurrentTower.transform.position;
		}

		/// <summary>
		/// ゴーストを画面中央へ移動する
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// 建設モードでない場合に例外を投げる
		/// </exception>
		public void MoveGhostToCenter()
		{
			if (state != State.Building)
			{
				throw new InvalidOperationException("Trying to move ghost when not in Build Mode");
			}
			// 有効な配置場所を探す
			Ray ray = m_Camera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
			RaycastHit placementHit;

			if (Physics.SphereCast(ray, sphereCastRadius, out placementHit, float.MaxValue, placementAreaMask))
			{
				MoveGhostWithRaycastHit(placementHit);
			}
			else
			{
				MoveGhostOntoWorld(ray, false);
			}
		}

		/// <summary>
		/// 初期値を設定し、アタッチされたコンポーネントをキャッシュして、
		/// 操作設定を行う
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			state = State.Normal;
			m_Camera = GetComponent<Camera>();
		}

		/// <summary>
		/// ゲームが一時停止中の場合にTimeScaleをリセットする
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			Time.timeScale = 1f;
		}

		/// <summary>
		/// レベルマネージャーを購読する
		/// </summary>
		protected virtual void OnEnable()
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.currency.currencyChanged += OnCurrencyChanged;
			}
		}

		/// <summary>
		/// レベルマネージャーの購読を解除する
		/// </summary>
		protected virtual void OnDisable()
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.currency.currencyChanged -= OnCurrencyChanged;
			}
		}

		/// <summary>
		/// 指定されたポインター位置のデータを保持する新しいUIPointerを作成する
		/// </summary>
		protected UIPointer WrapPointer(PointerInfo pointerInfo)
		{
			return new UIPointer
			{
				overUI = IsOverUI(pointerInfo),
				pointer = pointerInfo,
				ray = m_Camera.ScreenPointToRay(pointerInfo.currentPosition)
			};
		}

		/// <summary>
		/// 指定されたポインターが何らかのUIの上にあるかどうかを確認する
		/// </summary>
		/// <param name="pointerInfo">判定するポインター</param>
		/// <returns>イベントシステムがこのポインターをUI上にあると判定した場合はtrue</returns>
		protected bool IsOverUI(PointerInfo pointerInfo)
		{
			int pointerId;
			EventSystem currentEventSystem = EventSystem.current;

			// ポインターIDはマウスでは負の値、タッチでは正の値になる
			var cursorInfo = pointerInfo as MouseCursorInfo;
			var mbInfo = pointerInfo as MouseButtonInfo;
			var touchInfo = pointerInfo as TouchInfo;

			if (cursorInfo != null)
			{
				pointerId = PointerInputModule.kMouseLeftId;
			}
			else if (mbInfo != null)
			{
				// 左マウスボタンは0だが、kMouseLeftIDは-1
				pointerId = -mbInfo.mouseButtonId - 1;
			}
			else if (touchInfo != null)
			{
				pointerId = touchInfo.touchId;
			}
			else
			{
				throw new ArgumentException("Passed pointerInfo is not a TouchInfo or MouseCursorInfo", "pointerInfo");
			}

			return currentEventSystem.IsPointerOverGameObject(pointerId);
		}

		/// <summary>
		/// ゴーストをポインターの位置へ移動する
		/// </summary>
		/// <param name="pointer">ゴーストを配置する位置を示すポインター</param>
		/// <param name="hideWhenInvalid">ゴーストを非表示にするかどうかの任意パラメーター</param>
		/// <exception cref="InvalidOperationException">正しい状態でない場合</exception>
		protected void MoveGhost(UIPointer pointer, bool hideWhenInvalid = true)
		{
			if (m_CurrentTower == null || !isBuilding)
			{
				throw new InvalidOperationException(
					"Trying to position a tower ghost while the UI is not currently in the building state.");
			}

			// 配置レイヤーにRaycastする
			PlacementAreaRaycast(ref pointer);

			if (pointer.raycast != null)
			{
				MoveGhostWithRaycastHit(pointer.raycast.Value);
			}
			else
			{
				MoveGhostOntoWorld(pointer.ray, hideWhenInvalid);
			}
		}


		/// <summary>
		/// m_PlacementAreaMaskへのRaycastHitが成功した位置へゴーストを移動する
		/// </summary>
		protected virtual void MoveGhostWithRaycastHit(RaycastHit raycast)
		{
			// 配置エリアのいずれかにヒットした
			// ヒットしたオブジェクトから配置エリアを取得してみる
			m_CurrentArea = raycast.collider.GetComponent<IPlacementArea>();

			if (m_CurrentArea == null)
			{
				Debug.LogError("There is not an IPlacementArea attached to the collider found on the m_PlacementAreaMask");
				return;
			}
			m_GridPosition = m_CurrentArea.WorldToGrid(raycast.point, m_CurrentTower.controller.dimensions);
			TowerFitStatus fits = m_CurrentArea.Fits(m_GridPosition, m_CurrentTower.controller.dimensions);

			m_CurrentTower.Show();
			m_GhostPlacementPossible = fits == TowerFitStatus.Fits && IsValidPurchase();
			m_CurrentTower.Move(m_CurrentArea.GridToWorld(m_GridPosition, m_CurrentTower.controller.dimensions),
								m_CurrentArea.transform.rotation,
								m_GhostPlacementPossible);
		}


		/// <summary>
		/// 指定されたRayを使ってゴーストを移動する
		/// </summary>
		protected virtual void MoveGhostOntoWorld(Ray ray, bool hideWhenInvalid)
		{
			m_CurrentArea = null;

			if (!hideWhenInvalid)
			{
				RaycastHit hit;
				// ゴーストを置けるすべてのレイヤーに対して確認する
				Physics.SphereCast(ray, sphereCastRadius, out hit, float.MaxValue, ghostWorldPlacementMask);
				if (hit.collider == null)
				{
					return;
				}
				m_CurrentTower.Show();
				m_CurrentTower.Move(hit.point, hit.collider.transform.rotation, false);
			}
			else
			{
				m_CurrentTower.Hide();
			}
		}

		/// <summary>
		/// ポインターの位置にゴーストを配置する
		/// </summary>
		/// <param name="pointer">ゴーストを配置する位置を示すポインター</param>
		/// <exception cref="InvalidOperationException">正しい状態でない場合</exception>
		protected void PlaceGhost(UIPointer pointer)
		{
			if (m_CurrentTower == null || !isBuilding)
			{
				throw new InvalidOperationException(
					"Trying to position a tower ghost while the UI is not currently in a building state.");
			}

			MoveGhost(pointer);

			if (m_CurrentArea != null)
			{
				TowerFitStatus fits = m_CurrentArea.Fits(m_GridPosition, m_CurrentTower.controller.dimensions);

				if (fits == TowerFitStatus.Fits)
				{
					// ゴーストを配置する
					Tower controller = m_CurrentTower.controller;

					Tower createdTower = Instantiate(controller);
					createdTower.Initialize(m_CurrentArea, m_GridPosition);

					CancelGhostPlacement();
				}
			}
		}

		/// <summary>
		/// タワー配置エリアにRaycastする
		/// </summary>
		/// <param name="pointer">判定するポインター</param>
		protected void PlacementAreaRaycast(ref UIPointer pointer)
		{
			pointer.raycast = null;

			if (pointer.overUI)
			{
				// ポインターがUIの上にあるため、有効な位置ではない
				return;
			}

			// 配置エリアレイヤーにRaycastする
			RaycastHit hit;
			if (Physics.Raycast(pointer.ray, out hit, float.MaxValue, placementAreaMask))
			{
				pointer.raycast = hit;
			}
		}

		/// <summary>
		/// 所持通貨が十分になったら、ゴーストタワーの有効表示を更新する
		/// </summary>
		protected virtual void OnCurrencyChanged()
		{
			if (!isBuilding || m_CurrentTower == null || m_CurrentArea == null)
			{
				return;
			}
			TowerFitStatus fits = m_CurrentArea.Fits(m_GridPosition, m_CurrentTower.controller.dimensions);
			bool valid = fits == TowerFitStatus.Fits && IsValidPurchase();
			m_CurrentTower.Move(m_CurrentArea.GridToWorld(m_GridPosition, m_CurrentTower.controller.dimensions),
			                    m_CurrentArea.transform.rotation,
			                    valid);
			if (valid && !m_GhostPlacementPossible && ghostBecameValid != null)
			{
				m_GhostPlacementPossible = true;
				ghostBecameValid();
			}
		}

		/// <summary>
		/// タワーが破壊されたときにTower UIを閉じる
		/// </summary>
		protected void OnTowerDied(DamageableBehaviour targetable)
		{
			towerUI.enabled = false;
			radiusVisualizerController.HideRadiusVisualizers();
			DeselectTower();
		}
		
		/// <summary>
		/// タワーを作成して非表示にし、buildInfoUIを表示する
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="towerToBuild"/> がnullの場合に例外を投げる
		/// </exception>
		void SetUpGhostTower([NotNull] Tower towerToBuild)
		{
			if (towerToBuild == null)
			{
				throw new ArgumentNullException("towerToBuild");
			}

			m_CurrentTower = Instantiate(towerToBuild.towerGhostPrefab);
			m_CurrentTower.Initialize(towerToBuild);
			m_CurrentTower.Hide();

			// 建設情報を有効にする
			if (buildInfoUI != null)
			{
				buildInfoUI.Show(towerToBuild);
			}
		}
	}
}
