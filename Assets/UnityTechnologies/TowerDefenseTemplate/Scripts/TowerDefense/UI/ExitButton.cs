using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// ゲームを終了するためのボタン
	/// </summary>
	public class ExitButton : Button
	{
		/// <summary>
		/// このボタンがクリックされたらゲームを終了する
		/// </summary>
		public override void OnPointerClick(PointerEventData eventData)
		{
			Application.Quit();
		}

		/// <summary>
		/// モバイルプラットフォームではこのボタンを無効にする
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

#if UNITY_ANDROID || UNITY_IOS
			if (Application.isPlaying)
			{
				gameObject.SetActive(false);
			}
#endif
		}
	}
}
