using UnityEngine;

namespace TowerDefense.UI
{
	/// <summary>
	/// 条件に応じたCanvasの移動を制御するクラス
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class MovingCanvas : MonoBehaviour
	{
		/// <summary>
		/// 画面範囲との照合に使用するRectTransform
		/// </summary>
		public RectTransform content;

		/// <summary>
		/// Canvasを配置する位置をオフセットするための値
		/// </summary>
		public Vector2 offset;

		/// <summary>
		/// アタッチされているCanvas
		/// </summary>
		Canvas m_Canvas;

		/// <summary>
		/// アタッチされているCanvasの有効/無効を切り替えるプロパティ
		/// </summary>
		public bool canvasEnabled
		{
			get
			{
				if (m_Canvas == null)
				{
					m_Canvas = GetComponent<Canvas>();
				}
				return m_Canvas.enabled;
			}
			set
			{
				if (m_Canvas == null)
				{
					m_Canvas = GetComponent<Canvas>();
				}
				m_Canvas.enabled = value;
			}
		}

		/// <summary>
		/// <see cref="content"/> のrectに基づいてCanvasの移動を試みる
		/// </summary>
		/// <param name="position">
		/// 移動先の位置
		/// </param>
		public void TryMove(Vector3 position)
		{
			Rect rect = content.rect;
			position += (Vector3) offset;
			rect.position = position;

			if (rect.xMin < rect.width * 0.5f)
			{
				position.x = rect.width * 0.5f;
			}
			if (rect.xMax > Screen.width - rect.width * 0.5f)
			{
				position.x = Screen.width - rect.width * 0.5f;
			}
			if (rect.yMin < rect.height * 0.5f)
			{
				position.y = rect.height * 0.5f;
			}
			if (rect.yMax > Screen.height - rect.height * 0.5f)
			{
				position.y = Screen.height - rect.height * 0.5f;
			}
			transform.position = position;
		}

		/// <summary>
		/// アタッチされているCanvasをキャッシュする
		/// </summary>
		protected virtual void Awake()
		{
			canvasEnabled = false;
		}
	}
}
