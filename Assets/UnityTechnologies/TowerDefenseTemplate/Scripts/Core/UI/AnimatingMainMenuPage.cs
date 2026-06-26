using UnityEngine;

namespace Core.UI
{
	/// <summary>
	/// 有効化と無効化の処理をアニメーションさせるメニューページの抽象基底クラス
	/// ページの有効化と無効化を扱う
	/// </summary>
	public abstract class AnimatingMainMenuPage : MonoBehaviour, IMainMenuPage
	{
		/// <summary>
		/// 無効化するCanvas。このオブジェクトが設定されている場合は、GameObjectではなくCanvasを無効化する
		/// </summary>
		public Canvas canvas;
		
		/// <summary>
		/// このページを非表示にする
		/// </summary>
		public virtual void Hide()
		{
			BeginDeactivatingPage();
		}

		/// <summary>
		/// このページを表示する
		/// </summary>
		public virtual void Show()
		{
			BeginActivatingPage();
		}

		/// <summary>
		/// 無効化処理を開始する。例: ページのフェードアウトを始める。完了したらFinishedDeactivatingPageを呼ぶ
		/// </summary>
		protected abstract void BeginDeactivatingPage();

		/// <summary>
		/// 無効化処理を終了し、関連するGameObjectまたはCanvasをオフにする
		/// </summary>
		protected virtual void FinishedDeactivatingPage()
		{
			if (canvas != null)
			{
				canvas.enabled = false;
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 関連するGameObjectまたはCanvasをオンにして有効化処理を開始する。完了したらFinishedActivatingPageを呼ぶ
		/// </summary>
		protected virtual void BeginActivatingPage()
		{
			if (canvas != null)
			{
				canvas.enabled = true;
			}
			else
			{
				gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// 有効化処理を完了する。例: 入力を有効にする
		/// </summary>
		protected abstract void FinishedActivatingPage();
	}
}
