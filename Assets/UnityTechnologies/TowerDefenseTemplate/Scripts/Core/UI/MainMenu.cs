using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
	/// <summary>
	/// MainMenuの抽象基底クラス
	/// 具象クラスでは、OptionsMenuなど各ページ用のシリアライズ対象フィールドを公開する
	/// 具象クラスでは、内部でChangePage()を使うページ切り替え用メソッドを公開する。例: OpenOptionsMenu()
	/// </summary>
	public abstract class MainMenu : MonoBehaviour
	{
		/// <summary>
		/// 現在開いているMenuPage
		/// </summary>
		protected IMainMenuPage m_CurrentPage;

		/// <summary>
		/// 特定のページに到達するまでに通ったページを記録するスタック。戻る処理で使用する
		/// </summary>
		protected Stack<IMainMenuPage> m_PageStack = new Stack<IMainMenuPage>();

		/// <summary>
		/// ページを切り替える
		/// </summary>
		/// <param name="newPage">遷移先のページ</param>
		protected virtual void ChangePage(IMainMenuPage newPage)
		{
			DeactivateCurrentPage();
			ActivateCurrentPage(newPage);
		}

		/// <summary>
		/// 現在のページがあれば無効化する
		/// </summary>
		protected void DeactivateCurrentPage()
		{
			if (m_CurrentPage != null)
			{
				m_CurrentPage.Hide();
			}
		}

		/// <summary>
		/// 新しいページを有効化し、現在のページとして設定してスタックに追加する
		/// </summary>
		/// <param name="newPage">有効化するページ</param>
		protected void ActivateCurrentPage(IMainMenuPage newPage)
		{
			m_CurrentPage = newPage;
			m_CurrentPage.Show();
			m_PageStack.Push(m_CurrentPage);
		}

		/// <summary>
		/// 指定したページへ戻る
		/// </summary>
		/// <param name="backPage">戻り先のページ</param>
		protected void SafeBack(IMainMenuPage backPage)
		{
			DeactivateCurrentPage();
			ActivateCurrentPage(backPage);
		}

		/// <summary>
		/// 可能であれば1つ前のページへ戻る
		/// </summary>
		public virtual void Back()
		{
			if (m_PageStack.Count == 0)
			{
				return;
			}

			DeactivateCurrentPage();
			m_PageStack.Pop();
			ActivateCurrentPage(m_PageStack.Pop());
		}

		/// <summary>
		/// 可能であれば指定したページへ戻る
		/// </summary>
		/// <param name="backPage">戻り先のページ</param>
		public virtual void Back(IMainMenuPage backPage)
		{
			int count = m_PageStack.Count;
			if (count == 0)
			{
				SafeBack(backPage);
				return;
			}

			for (int i = count - 1; i >= 0; i--)
			{
				IMainMenuPage currentPage = m_PageStack.Pop();
				if (currentPage == backPage)
				{
					SafeBack(backPage);
					return;
				}
			}

			SafeBack(backPage);
		}
	}
}
