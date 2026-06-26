using UnityEngine;

namespace Core.Camera
{
	/// <summary>
	/// カメラの初期状態の値を設定するシンプルなクラス
	/// </summary>
	[RequireComponent(typeof(CameraRig))]
	public class CameraInitialState : MonoBehaviour
	{
		public enum StartZoomMode
		{
			NoChange,
			FurthestZoom,
			NearestZoom
		}

		/// <summary>
		/// カメラの開始時のズームレベルを決めます
		/// </summary>
		public StartZoomMode startZoomMode;
		
		/// <summary>
		/// 開始時にカメラが注視するオブジェクト
		/// </summary>
		public Transform initialLookAt;
		
		/// <summary>
		/// 開始時にカメラのパラメーターを設定します
		/// </summary>
		protected virtual void Start()
		{
			var rig = GetComponent<CameraRig>();

			switch (startZoomMode)
			{
				case StartZoomMode.FurthestZoom:
					rig.SetZoom(rig.furthestZoom);
					break;
				case StartZoomMode.NearestZoom:
					rig.SetZoom(rig.nearestZoom);
					break;
			}

			if (initialLookAt != null)
			{
				rig.PanTo(initialLookAt.transform.position);
			}
		}
	}
}