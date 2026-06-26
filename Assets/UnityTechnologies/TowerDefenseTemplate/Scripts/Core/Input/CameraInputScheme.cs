using Core.Camera;
using UnityEngine;

namespace Core.Input
{
	/// <summary>
	/// CameraRigを制御する入力スキーム用の抽象基底クラス
	/// </summary>
	public abstract class CameraInputScheme : InputScheme
	{
		/// <summary>
		/// 制御対象のCameraRig
		/// </summary>
		public CameraRig cameraRig;

		/// <summary>
		/// 最大までズームインしたときのパン速度係数
		/// </summary>
		public float nearZoomPanSpeedModifier = 0.2f;

		/// <summary>
		/// 指定したズームレベルでのパン速度倍率を取得します
		/// </summary>
		/// <returns></returns>
		protected float GetPanSpeedForZoomLevel()
		{
			return cameraRig != null ? 
				Mathf.Lerp(nearZoomPanSpeedModifier, 1, cameraRig.CalculateZoomRatio()) : 
				1.0f;
		}

		/// <summary>
		/// 指定したスクリーン座標を使って画面端パンを行います
		/// </summary>
		/// <param name="screenPosition">カメラをパンするカーソルのスクリーン座標</param>
		/// <param name="screenEdgeThreshold">画面端判定のしきい値（ピクセル）</param>
		/// <param name="panSpeed">パン速度</param>
		protected void PanWithScreenCoordinates(Vector2 screenPosition, float screenEdgeThreshold, float panSpeed)
		{
			// ズーム比率を計算します
			float zoomRatio = GetPanSpeedForZoomLevel();

			// 左
			if ((screenPosition.x < screenEdgeThreshold))
			{
				float panAmount = (screenEdgeThreshold - screenPosition.x) / screenEdgeThreshold;
				panAmount = Mathf.Clamp01(Mathf.Log(panAmount) + 1);

				if (cameraRig.trackingObject == null)
				{
					cameraRig.PanCamera(Vector3.left * Time.deltaTime * panSpeed * panAmount * zoomRatio);

					cameraRig.StopTracking();
				}
			}

			// 右
			if ((screenPosition.x > Screen.width - screenEdgeThreshold))
			{
				float panAmount = ((screenEdgeThreshold - Screen.width) + screenPosition.x) / screenEdgeThreshold;
				panAmount = Mathf.Clamp01(Mathf.Log(panAmount) + 1);

				if (cameraRig.trackingObject == null)
				{
					cameraRig.PanCamera(Vector3.right * Time.deltaTime * panSpeed * panAmount * zoomRatio);
				}
				cameraRig.StopTracking();
			}

			// 下
			if ((screenPosition.y < screenEdgeThreshold))
			{
				float panAmount = (screenEdgeThreshold - screenPosition.y) / screenEdgeThreshold;
				panAmount = Mathf.Clamp01(Mathf.Log(panAmount) + 1);

				if (cameraRig.trackingObject == null)
				{
					cameraRig.PanCamera(Vector3.back * Time.deltaTime * panSpeed * panAmount * zoomRatio);

					cameraRig.StopTracking();
				}
			}

			// 上
			if ((screenPosition.y > Screen.height - screenEdgeThreshold))
			{
				float panAmount = ((screenEdgeThreshold - Screen.height) + screenPosition.y) / screenEdgeThreshold;
				panAmount = Mathf.Clamp01(Mathf.Log(panAmount) + 1);

				if (cameraRig.trackingObject == null)
				{
					cameraRig.PanCamera(Vector3.forward * Time.deltaTime * panSpeed * panAmount * zoomRatio);

					cameraRig.StopTracking();
				}
			}
		}
	}
}