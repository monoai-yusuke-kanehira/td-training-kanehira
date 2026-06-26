using Core.Input;
using TowerDefense.Level;
using TowerDefense.Towers;
using TowerDefense.UI.HUD;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System のために追加
using State = TowerDefense.UI.HUD.GameUI.State;

namespace TowerDefense.Input
{
    [RequireComponent(typeof(GameUI))]
    public class TowerDefenseKeyboardMouseInput : KeyboardMouseInput
    {
        GameUI m_GameUI;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            m_GameUI = GetComponent<GameUI>();

            if (InputController.instanceExists)
            {
                InputController controller = InputController.instance;

                controller.tapped += OnTap;
                controller.mouseMoved += OnMouseMoved;
            }
        }

        protected override void OnDisable()
        {
            if (!InputController.instanceExists)
            {
                return;
            }

            InputController controller = InputController.instance;

            controller.tapped -= OnTap;
            controller.mouseMoved -= OnMouseMoved;
        }

        protected override void Update()
        {
            base.Update();
            
            if (Keyboard.current == null) return;
            var kb = Keyboard.current;

            // New Input System で Escape キーを処理する
            if (kb.escapeKey.wasPressedThisFrame)
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
                    case State.BuildingWithDrag:
                    case State.Building:
                        m_GameUI.CancelGhostPlacement();
                        break;
                }
            }
            
            // タワー配置用のキーボードショートカット（1-9 と 0）
            if (LevelManager.instanceExists)
            {
                int towerLibraryCount = LevelManager.instance.towerLibrary.Count;
                
                // 数字キーの対応付け（Alpha1 がインデックス 0、Alpha2 が 1、以降同様）
                Key[] numKeys = { 
                    Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5, 
                    Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9 
                };

                int count = Mathf.Min(9, towerLibraryCount);

                for (int i = 0; i < count; i++)
                {
                    if (kb[numKeys[i]].wasPressedThisFrame)
                    {
                        TrySetBuildMode(i);
                        break;
                    }
                }

                // 0 キー用の特別処理（ライブラリのインデックス 9 に対応）
                if (towerLibraryCount > 9 && kb.digit0Key.wasPressedThisFrame)
                {
                    TrySetBuildMode(9);
                }
            }
        }

        /// <summary>
        /// 数字キーを押したときの処理の重複を避けるための補助メソッド
        /// </summary>
        void TrySetBuildMode(int libraryIndex)
        {
            Tower controller = LevelManager.instance.towerLibrary[libraryIndex];
            if (LevelManager.instance.currency.CanAfford(controller.purchaseCost))
            {
                if (m_GameUI.isBuilding)
                {
                    m_GameUI.CancelGhostPlacement();
                }
                GameUI.instance.SetToBuildMode(controller);
                GameUI.instance.TryMoveGhost(InputController.instance.basicMouseInfo);
            }
        }

        void OnMouseMoved(PointerInfo pointer)
        {
            var mouseInfo = pointer as MouseCursorInfo;
            if ((mouseInfo != null) && (m_GameUI.isBuilding))
            {
                m_GameUI.TryMoveGhost(pointer, false);
            }
        }

        void OnTap(PointerActionInfo pointer)
        {
            var mouseInfo = pointer as MouseButtonInfo;

            if (mouseInfo != null && !mouseInfo.startedOverUI)
            {
                if (m_GameUI.isBuilding)
                {
                    if (mouseInfo.mouseButtonId == 0) // 左クリックで確定
                    {
                        m_GameUI.TryPlaceTower(pointer);
                    }
                    else // 右クリックでキャンセル
                    {
                        m_GameUI.CancelGhostPlacement();
                    }
                }
                else
                {
                    if (mouseInfo.mouseButtonId == 0)
                    {
                        m_GameUI.TrySelectTower(pointer);
                    }
                }
            }
        }
    }
}
