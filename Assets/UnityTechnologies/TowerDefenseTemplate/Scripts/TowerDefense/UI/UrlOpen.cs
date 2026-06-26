using UnityEngine;

namespace TowerDefense.UI
{
	/// <summary>
	/// URLを開くシンプルなスクリプト
	/// </summary>
	public class UrlOpen : MonoBehaviour
	{
		/// <summary>
		/// 指定されたURLを開く
		/// </summary>
		public void OpenUrl(string url)
		{
			Application.OpenURL(url);
		}
	}
}
