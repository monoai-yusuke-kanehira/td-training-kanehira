using Core.Input;
using TowerDefense.UI;
using TowerDefense.UI.HUD;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using State = TowerDefense.UI.HUD.GameUI.State;

namespace TowerDefense.Input
{
	[RequireComponent(typeof(GameUI))]
	public class TowerDefenseTouchInput : TouchInput
	{
		/// <summary>
		/// ドラッグ中にパンが発生する画面の割合
		/// </summary>
		[Range(0, 0.5f)]
		public float panAreaScreenPercentage = 0.2f;

		/// <summary>
		/// 確認ボタンを保持するオブジェクト
		/// </summary>
		public MovingCanvas confirmationButtons;

		/// <summary>
		/// 無効な選択を保持するオブジェクト
		/// </summary>
		public MovingCanvas invalidButtons;

		/// <summary>
		/// 添付されたゲーム UI オブジェクト
		/// </summary>
		GameUI m_GameUI;

		/// <summary>
		/// ゴーストタワーが選択されているかどうかを追跡します
		/// </summary>
		bool m_IsGhostSelected;

		/// <summary>
		/// 画面端のポインタ
		/// </summary>
		TouchInfo m_DragPointer;

		/// <summary>
		/// UI の確認ボタンによって呼び出されます
		/// </summary>
		public void OnTowerPlacementConfirmation()
		{
			confirmationButtons.canvasEnabled = false;
			if (!m_GameUI.IsGhostAtValidPosition())
			{
				return;
			}
			m_GameUI.BuyTower();
		}

		/// <summary>
		/// UI の閉じるボタンによって呼び出されます
		/// </summary>
		public void Cancel()
		{
			GameUI.instance.CancelGhostPlacement();
			confirmationButtons.canvasEnabled = false;
			invalidButtons.canvasEnabled = false;
		}

		/// <summary>
		/// 入力イベントの登録
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			
			m_GameUI = GetComponent<GameUI>();
			
			m_GameUI.stateChanged += OnStateChanged;
			m_GameUI.ghostBecameValid += OnGhostBecameValid;

			// タップイベントを登録する
			if (InputController.instanceExists)
			{
				InputController.instance.tapped += OnTap;
				InputController.instance.startedDrag += OnStartDrag;
			}

			// ポップアップを無効にします
			confirmationButtons.canvasEnabled = false;
			invalidButtons.canvasEnabled = false;
		
		}

		/// <summary>
		/// 入力イベントの登録を解除します
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			
			if (confirmationButtons != null)
			{
				confirmationButtons.canvasEnabled = false;
			}
			if (invalidButtons != null)
			{
				invalidButtons.canvasEnabled = false;
			}
			if (InputController.instanceExists)
			{
				InputController.instance.tapped -= OnTap;
				InputController.instance.startedDrag -= OnStartDrag;
			}
			if (m_GameUI != null)
			{
				m_GameUI.stateChanged -= OnStateChanged;
				m_GameUI.ghostBecameValid -= OnGhostBecameValid;
			}
		}

		/// <summary>
		/// UIを非表示にします
		/// </summary>
		protected virtual void Awake()
		{
			if (confirmationButtons != null)
			{
				confirmationButtons.canvasEnabled = false;
			}
			if (invalidButtons != null)
			{
				invalidButtons.canvasEnabled = false;
			}
		}

		/// <summary>
		/// フリックの勢いを減衰させます
		/// </summary>
		protected override void Update()
		{
			base.Update();

			// 画面端でパンします
			if (m_DragPointer != null)
			{
				EdgePan();
			}

			if (UnityInput.GetKeyDown(KeyCode.Escape))
			{
				switch (m_GameUI.state)
				{
					case State.Normal:
						if (m_GameUI.isTowerSelected)
						{
							m_GameUI.DeselectTower();
						}
						else
						{
							m_GameUI.Pause();
						}
						break;
					case State.Building:
						m_GameUI.CancelGhostPlacement();
						break;
				}
			}
		}

		/// <summary>
		/// 入力を押すと呼び出されます
		/// </summary>
		protected override void OnPress(PointerActionInfo pointer)
		{
			base.OnPress(pointer);
			var touchInfo = pointer as TouchInfo;
			// 幽霊にプレスが始まる？それなら拾ってみよう
			if (touchInfo != null)
			{
				if (m_GameUI.state == State.Building)
				{
					m_IsGhostSelected = m_GameUI.IsPointerOverGhost(pointer);
					if (m_IsGhostSelected)
					{
						m_DragPointer = touchInfo;
					}
				}				
			}
		}

		/// <summary>
		/// フリックの場合、入力リリース時に呼び出されます
		/// </summary>
		protected override void OnRelease(PointerActionInfo pointer)
		{
			// 通常の動作をオーバーライドします。ゴーストが選択されていない場合にのみフリックを実行したい
			// このため、意図的にbaseを呼び出しません
			var touchInfo = pointer as TouchInfo;

			if (touchInfo != null)
			{
				// リリース時に UI を表示する
				if (m_GameUI.isBuilding)
				{
					Vector2 screenPoint = cameraRig.cachedCamera.WorldToScreenPoint(m_GameUI.GetGhostPosition());
					if (m_GameUI.IsGhostAtValidPosition() && m_GameUI.IsValidPurchase())
					{
						confirmationButtons.canvasEnabled = true;
						invalidButtons.canvasEnabled = false;
						confirmationButtons.TryMove(screenPoint);
					}
					else
					{
						invalidButtons.canvasEnabled = true;
						confirmationButtons.canvasEnabled = false;
						confirmationButtons.TryMove(screenPoint);
					}
					if (m_IsGhostSelected)
					{
						m_GameUI.ReturnToBuildMode();
					}
				}
				if (!m_IsGhostSelected && cameraRig != null)
				{
					// ここで通常の基本動作を実行します
					DoReleaseFlick(pointer);
				}
				
				m_IsGhostSelected = false;

				// 解放された場合は m_DragPointer をリセット
				if (m_DragPointer != null && m_DragPointer.touchId == touchInfo.touchId)
				{
					m_DragPointer = null;
				}
			}
		}

		/// <summary>
		/// タップで呼び出され、
		/// タワーの配置の確認を呼び出します
		/// </summary>
		protected virtual void OnTap(PointerActionInfo pointerActionInfo)
		{
			var touchInfo = pointerActionInfo as TouchInfo;
			if (touchInfo != null)
			{
				if (m_GameUI.state == State.Normal && !touchInfo.startedOverUI)
				{
					m_GameUI.TrySelectTower(touchInfo);
				}
				else if (m_GameUI.state == State.Building && !touchInfo.startedOverUI)
				{
					m_GameUI.TryMoveGhost(touchInfo, false);
					if (m_GameUI.IsGhostAtValidPosition() && m_GameUI.IsValidPurchase())
					{
						confirmationButtons.canvasEnabled = true;
						invalidButtons.canvasEnabled = false;
						confirmationButtons.TryMove(touchInfo.currentPosition);
					}
					else
					{
						invalidButtons.canvasEnabled = true;
						invalidButtons.TryMove(touchInfo.currentPosition);
						confirmationButtons.canvasEnabled = false;
					}
				}
			}
		}

		/// <summary>
		/// ドラッグ ポインターを割り当て、UI をドラッグ モードに設定します
		/// </summary>
		/// <param name="pointer"></param>
		protected virtual void OnStartDrag(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;
			if (touchInfo != null)
			{
				if (m_IsGhostSelected)
				{
					m_GameUI.ChangeToDragMode();
					m_DragPointer = touchInfo;
				}
			}
		}
		

		/// <summary>
		/// ドラッグすると呼び出されます
		/// </summary>
		protected override void OnDrag(PointerActionInfo pointer)
		{
			// 通常の動作をオーバーライドします。ゴーストが選択されていない場合にのみパンしたい
			// このため、意図的にbaseを呼び出しません
			var touchInfo = pointer as TouchInfo;
			if (touchInfo != null)
			{
				// タワーが引きずり落とされた場合は拾ってみてください
				if (m_IsGhostSelected)
				{
					m_GameUI.TryMoveGhost(pointer, false);
				}
				
				if (m_GameUI.state == State.BuildingWithDrag)
				{
					DragGhost(touchInfo);
				}
				else
				{
					// ゴーストが選択されていない場合にのみ、通常の基本動作を実行します
					if (cameraRig != null)
					{
						DoDragPan(pointer);

						if (invalidButtons.canvasEnabled)
						{
							invalidButtons.TryMove(cameraRig.cachedCamera.WorldToScreenPoint(m_GameUI.GetGhostPosition()));
						}
						if (confirmationButtons.canvasEnabled)
						{
							confirmationButtons.TryMove(cameraRig.cachedCamera.WorldToScreenPoint(m_GameUI.GetGhostPosition()));
						}
					}
				}
			}
		}

		/// <summary>
		/// 幽霊を引きずる
		/// </summary>
		void DragGhost(TouchInfo touchInfo)
		{
			if (touchInfo.touchId == m_DragPointer.touchId)
			{
				m_GameUI.TryMoveGhost(touchInfo, false);

				if (invalidButtons.canvasEnabled)
				{
					invalidButtons.canvasEnabled = false;
				}
				if (confirmationButtons.canvasEnabled)
				{
					confirmationButtons.canvasEnabled = false;
				}
			}
		}

		/// <summary>
		/// 画面の端でパンします
		/// </summary>
		void EdgePan()
		{
			float edgeWidth = panAreaScreenPercentage * Screen.width;
			PanWithScreenCoordinates(m_DragPointer.currentPosition, edgeWidth, panSpeed);
		}
		

		/// <summary>
		/// 新しい状態が次の場合 <see cref="GameUI.State.Building"/> 次にゴーストを画面の中央に移動します
		/// </summary>
		/// <param name="previousState">
		/// 以前の GameUI は次のとおりです
		/// </param>
		/// <param name="currentState">
		/// GameUI の新しい状態
		/// </param>
		void OnStateChanged(State previousState, State currentState)
		{
			// 2 つの理由により早期復帰
			// 1. ビルドモードには移行しません
			// 2.実際には触れていない
			if (UnityInput.touchCount == 0)
			{
				return;
			}
			if (currentState == State.Building && previousState != State.BuildingWithDrag)
			{
				m_GameUI.MoveGhostToCenter();
				confirmationButtons.canvasEnabled = false;
				invalidButtons.canvasEnabled = false;
			}
			if (currentState == State.BuildingWithDrag)
			{
				m_IsGhostSelected = true;
			}
		}

		/// <summary>
		/// タワーが有効になったときに正しい確認ボタンを表示します
		/// </summary>
		void OnGhostBecameValid()
		{
			// これは、無効なボタンがすでに画面上にある場合にのみ実行する必要があります
			if (!invalidButtons.canvasEnabled)
			{
				return;
			}
			Vector2 screenPoint = cameraRig.cachedCamera.WorldToScreenPoint(m_GameUI.GetGhostPosition());
			if (!confirmationButtons.canvasEnabled)
			{
				confirmationButtons.canvasEnabled = true;
				invalidButtons.canvasEnabled = false;
				confirmationButtons.TryMove(screenPoint);
			}
		}
	}
}
