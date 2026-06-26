using Core.Input;
using TowerDefense.UI.HUD;
using State = TowerDefense.UI.HUD.GameUI.State;

namespace TowerDefense.UI
{
	/// <summary>
	/// Tower Defense専用の入力切替。ゲームのポーズ中は操作も無効にする
	/// </summary>
	public class TowerDefenseInputSchemeSwitcher : InputSchemeSwitcher
	{
		/// <summary>
		/// ゲームがポーズ状態かどうかを取得する
		/// </summary>
		public bool isPaused
		{
			get { return GameUI.instance.state == State.Paused; }
		}

		/// <summary>
		/// GameUIのstateChangedイベントを登録する
		/// </summary>
		protected virtual void Start()
		{
			if (GameUI.instanceExists)
			{
				GameUI.instance.stateChanged += OnUIStateChanged;
			}
		}

		/// <summary>
		/// ゲームがポーズ中の場合は何もしない
		/// </summary>
		protected override void Update()
		{
			if (isPaused)
			{
				return;
			}
			
			base.Update();
		}

		/// <summary>
		/// GameUIのstateChangedイベントの登録を解除する
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (GameUI.instanceExists)
			{
				GameUI.instance.stateChanged -= OnUIStateChanged;
			}
		}

		/// <summary>
		/// ゲームがポーズ/ポーズ解除されたとき、現在の入力スキームを有効または無効にする
		/// </summary>
		void OnUIStateChanged(State oldState, State newState)
		{
			if (m_CurrentScheme == null)
			{
				return;
			}
			if (newState == State.Paused)
			{
				m_CurrentScheme.Deactivate(null);
			}
			else
			{
				m_CurrentScheme.Activate(null);
			}
		}
	}
}
