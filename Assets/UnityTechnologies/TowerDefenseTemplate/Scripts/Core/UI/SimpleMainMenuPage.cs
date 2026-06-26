using UnityEngine;

namespace Core.UI
{
	/// <summary>
	/// 単純にオンとオフを切り替えるメインメニューページの基本クラス
	/// </summary>
	public class SimpleMainMenuPage : MonoBehaviour, IMainMenuPage
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
		/// このページを表示する
		/// </summary>
		public virtual void Show()
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
	}
}
