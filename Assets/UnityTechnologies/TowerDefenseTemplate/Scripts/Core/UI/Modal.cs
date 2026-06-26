using UnityEngine;

namespace Core.UI
{
	/// <summary>
	/// すべてのモーダルの抽象基底クラス
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class Modal : MonoBehaviour
	{
		/// <summary>
		/// アタッチされているCanvasGroup
		/// </summary>
		public CanvasGroup canvasGroup;

		/// <summary>
		/// モーダルを閉じる
		/// </summary>
		public virtual void CloseModal()
		{
			gameObject.SetActive(false);
			DisableInteractivity();
		}

		/// <summary>
		/// モーダルを表示する
		/// </summary>
		public virtual void Show()
		{
			LazyLoad();
			gameObject.SetActive(true);
			EnableInteractivity();
		}

		/// <summary>
		/// 操作できるようにする
		/// </summary>
		protected virtual void EnableInteractivity()
		{
			canvasGroup.interactable = true;
		}

		/// <summary>
		/// 操作できないようにする
		/// </summary>
		protected virtual void DisableInteractivity()
		{
			canvasGroup.interactable = false;
		}

		/// <summary>
		/// CanvasGroupを遅延取得してローカル変数に設定する
		/// </summary>
		protected virtual void LazyLoad()
		{
			if (canvasGroup != null)
			{
				canvasGroup = GetComponent<CanvasGroup>();
			}
		}
	}
}
