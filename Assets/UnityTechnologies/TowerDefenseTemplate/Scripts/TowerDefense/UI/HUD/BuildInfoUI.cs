using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// ゴーストタワーのドラッグ中に情報表示を制御するクラス
	/// </summary>
	[RequireComponent(typeof(TowerUI))]
	public class BuildInfoUI : MonoBehaviour
	{
		/// <summary>
		/// UIアニメーションの状態を簡単に追跡するためのenum
		/// </summary>
		public enum AnimationState
		{
			/// <summary>
			/// UIが完全に非表示になっている
			/// </summary>
			Hidden,
			
			/// <summary>
			/// UIが表示されるアニメーション中
			/// </summary>
			Showing,
			
			/// <summary>
			/// UIが完全に表示されている
			/// </summary>
			Shown,
			
			/// <summary>
			/// UIが非表示になるアニメーション中
			/// </summary>
			Hiding
		}
		
		/// <summary>
		/// アタッチされているアニメーター
		/// </summary>
		public Animation anim;

		/// <summary>
		/// UIを表示するクリップ名
		/// </summary>
		public string showClipName = "Show";

		/// <summary>
		/// UIを非表示にするクリップ名
		/// </summary>
		public string hideClipName = "Hide";

		/// <summary>
		/// アタッチされている <see cref="TowerUI"/>
		/// </summary>
		protected TowerUI m_TowerUI;

		/// <summary>
		/// アタッチされているCanvas
		/// </summary>
		protected Canvas m_Canvas;

		/// <summary>
		/// UIのアニメーション状態を追跡する
		/// </summary>
		AnimationState m_State;

		/// <summary>
		/// 注: Showアニメーションクリップのイベントから再生される
		/// 表示アニメーションの終了時に呼ばれる
		/// <see cref="m_State"/> をShownに設定する
		/// </summary>
		public void ShowEnd()
		{
			m_State = AnimationState.Shown;
		}

		/// <summary>
		/// 注: Hideアニメーションクリップのイベントから再生される
		/// 非表示アニメーションの終了時に呼ばれる
		/// <see cref="m_State"/> をHiddenに設定する
		/// </summary>
		public void HideEnd()
		{
			m_State = AnimationState.Hidden;
		}

		/// <summary>
		/// 情報を表示する
		/// </summary>
		/// <param name="controller">
		/// 表示するタワー情報
		/// </param>
		public virtual void Show(Tower controller)
		{
			m_TowerUI.Show(controller);
			if (m_State == AnimationState.Shown)
			{
				return;
			}
			anim.Play(showClipName);
			if (m_State == AnimationState.Hiding)
			{
				anim[showClipName].normalizedTime = 1;
				m_State = AnimationState.Shown;
				return;
			}
			m_State = anim[showClipName].normalizedTime < 1 ? AnimationState.Showing : 
				AnimationState.Shown;
		}

		/// <summary>
		/// 情報を非表示にする
		/// </summary>
		public virtual void Hide()
		{
			if (m_State == AnimationState.Hidden)
			{
				return;
			}
			m_TowerUI.Hide();
			anim.Play(hideClipName);
			m_State = anim[hideClipName].normalizedTime < 1 ? AnimationState.Hiding : 
				AnimationState.Hidden;
		}

		/// <summary>
		/// アタッチされているCanvasとTowerControllerUIをキャッシュする
		/// </summary>
		protected virtual void Awake()
		{
			m_Canvas = GetComponent<Canvas>();
			m_TowerUI = GetComponent<TowerUI>();
		}
	}
}
