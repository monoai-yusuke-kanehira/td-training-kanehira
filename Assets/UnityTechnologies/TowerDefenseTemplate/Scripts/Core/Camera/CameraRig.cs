using Core.Input;
using UnityEngine;

namespace Core.Camera
{
	/// <summary>
	/// カメラの動作を制御するクラスです。CameraRigは、ほぼ
	/// 単一平面の地形で最も安定して動作します
	/// </summary>
	public class CameraRig : MonoBehaviour
	{
		/// <summary>
		/// 注視位置の減衰係数
		/// </summary>
		public float lookDampFactor;

		/// <summary>
		/// 移動の減衰係数
		/// </summary>
		public float movementDampFactor;

		/// <summary>
		/// 最も近いズームレベル。タッチ操作では弾力感を出すため、この値を少し超えられます
		/// </summary>
		public float nearestZoom = 15;

		/// <summary>
		/// 最も遠いズームレベル。タッチ操作では弾力感を出すため、この値を少し超えられます
		/// </summary>
		public float furthestZoom = 40;

		/// <summary>
		/// 実際の最大ズームレベル
		/// </summary>
		public float maxZoom = 60;

		/// <summary>
		/// 最遠ズームを超えた分を減衰させるために使う対数
		/// </summary>
		public float zoomLogFactor = 10;

		/// <summary>
		/// ズームが通常範囲へ戻る速さ
		/// </summary>
		public float zoomRecoverSpeed = 20;

		/// <summary>
		/// カメラが基準にする床のY座標
		/// </summary>
		public float floorY;

		/// <summary>
		/// 最大までズームインしたときのカメラ角度
		/// </summary>
		public Transform zoomedCamAngle;

		/// <summary>
		/// 編集モードでCameraRigEditorスクリプトから編集されるマップサイズ
		/// </summary>
		[HideInInspector]
		public Rect mapSize = new Rect(-10, -10, 20, 20);

		/// <summary>
		/// ラバーバンド効果で通常のズーム範囲を超えられるかどうか
		/// </summary>
		public bool springyZoom = true;

		/// <summary>
		/// 現在のカメラ注視位置の速度
		/// </summary>
		Vector3 m_CurrentLookVelocity;

		/// <summary>
		/// 各ズームレベルでのカメラ回転
		/// </summary>
		Quaternion m_MinZoomRotation;
		Quaternion m_MaxZoomRotation;

		/// <summary>
		/// 現在のカメラ速度
		/// </summary>
		Vector3 m_CurrentCamVelocity;

		/// <summary>
		/// 現在再利用している床平面
		/// </summary>
		Plane m_FloorPlane;

		public Plane floorPlane
		{
			get { return m_FloorPlane; }
		}

		/// <summary>
		/// 注視しているグリッド上の目標位置
		/// </summary>
		public Vector3 lookPosition { get; private set; }

		/// <summary>
		/// 現在のカメラ注視位置
		/// </summary>
		public Vector3 currentLookPosition { get; private set; }

		/// <summary>
		/// カメラの目標位置
		/// </summary>
		public Vector3 cameraPosition { get; private set; }

		/// <summary>
		/// マップサイズ、ズームレベル、画面比率や画面サイズに応じた注視範囲
		/// </summary>
		public Rect lookBounds { get; private set; }

		/// <summary>
		/// 現在のズーム距離を取得します
		/// </summary>
		public float zoomDist { get; private set; }

		/// <summary>
		/// クランプやスケーリングを適用する前の内部ズーム距離を取得します
		/// </summary>
		public float rawZoomDist { get; private set; }

		/// <summary>
		/// 追跡中のユニットがあれば取得します
		/// </summary>
		public GameObject trackingObject { get; private set; }

		/// <summary>
		/// キャッシュしたCameraコンポーネント
		/// </summary>
		public UnityEngine.Camera cachedCamera { get; private set; }

		/// <summary>
		/// 参照と床平面を初期化します
		/// </summary>
		protected virtual void Awake()
		{
			cachedCamera = GetComponent<UnityEngine.Camera>();
			m_FloorPlane = new Plane(Vector3.up, new Vector3(0.0f, floorY, 0.0f));
			
			// 初期値を設定します
			var lookRay = new Ray(cachedCamera.transform.position, cachedCamera.transform.forward);

			float dist;
			if (m_FloorPlane.Raycast(lookRay, out dist))
			{
				currentLookPosition = lookPosition = lookRay.GetPoint(dist);
			}
			cameraPosition = cachedCamera.transform.position;

			m_MinZoomRotation = Quaternion.FromToRotation(Vector3.up, -cachedCamera.transform.forward);
			m_MaxZoomRotation = Quaternion.FromToRotation(Vector3.up, -zoomedCamAngle.transform.forward);
			rawZoomDist = zoomDist = (currentLookPosition - cameraPosition).magnitude;
		}

		/// <summary>
		/// 初期ズームレベルとカメラ範囲を設定します
		/// </summary>
		protected virtual void Start()
		{
			RecalculateBoundingRect();
		}

		/// <summary>
		/// カメラの動作を処理します
		/// </summary>
		protected virtual void Update()
		{
			RecalculateBoundingRect();

			// 追跡中かどうか
			if (trackingObject != null)
			{
				PanTo(trackingObject.transform.position);

				if (!trackingObject.activeInHierarchy)
				{
					StopTracking();
				}
			}

			// 注視位置へ近づけます
			currentLookPosition = Vector3.SmoothDamp(currentLookPosition, lookPosition, ref m_CurrentLookVelocity,
			                                         lookDampFactor);

			Vector3 worldPos = transform.position;
			worldPos = Vector3.SmoothDamp(worldPos, cameraPosition, ref m_CurrentCamVelocity,
			                              movementDampFactor);

			transform.position = worldPos;
			transform.LookAt(currentLookPosition);
		}

#if UNITY_EDITOR
		/// <summary>
		/// 範囲確認用のGizmo
		/// </summary>
		void OnDrawGizmosSelected()
		{
			// 編集モードでは表示しません
			if (!Application.isPlaying)
			{
				return;
			}
			if (cachedCamera == null)
			{
				cachedCamera = GetComponent<UnityEngine.Camera>();
			}
			RecalculateBoundingRect();

			Gizmos.color = Color.red;

			Gizmos.DrawLine(
				new Vector3(lookBounds.xMin, 0.0f, lookBounds.yMin),
				new Vector3(lookBounds.xMax, 0.0f, lookBounds.yMin));
			Gizmos.DrawLine(
				new Vector3(lookBounds.xMin, 0.0f, lookBounds.yMin),
				new Vector3(lookBounds.xMin, 0.0f, lookBounds.yMax));
			Gizmos.DrawLine(
				new Vector3(lookBounds.xMax, 0.0f, lookBounds.yMax),
				new Vector3(lookBounds.xMin, 0.0f, lookBounds.yMax));
			Gizmos.DrawLine(
				new Vector3(lookBounds.xMax, 0.0f, lookBounds.yMax),
				new Vector3(lookBounds.xMax, 0.0f, lookBounds.yMin));

			Gizmos.color = Color.yellow;

			Gizmos.DrawLine(transform.position, currentLookPosition);
		}
#endif

		/// <summary>
		/// カメラを指定位置へパンします
		/// </summary>
		/// <param name="position">注視対象</param>
		public void PanTo(Vector3 position)
		{
			Vector3 pos = position;

			// 注視位置を床の高さに合わせます
			pos.y = floorY;

			// 注視範囲内に制限します
			pos.x = Mathf.Clamp(pos.x, lookBounds.xMin, lookBounds.xMax);
			pos.z = Mathf.Clamp(pos.z, lookBounds.yMin, lookBounds.yMax);
			lookPosition = pos;

			// 注視位置、視線ベクトル、ズーム距離からカメラ位置を計算します
			cameraPosition = lookPosition + (GetToCamVector() * zoomDist);
		}

		/// <summary>
		/// カメラにユニットを追跡させます
		/// </summary>
		/// <param name="objectToTrack"></param>
		public void TrackObject(GameObject objectToTrack)
		{
			trackingObject = objectToTrack;
			PanTo(trackingObject.transform.position);
		}

		/// <summary>
		/// ユニットの追跡を停止します
		/// </summary>
		public void StopTracking()
		{
			trackingObject = null;
		}

		/// <summary>
		/// カメラをパンします
		/// </summary>
		/// <param name="panDelta">ワールド空間単位でカメラをパンする距離</param>
		public void PanCamera(Vector3 panDelta)
		{
			Vector3 pos = lookPosition;
			pos += panDelta;

			// 注視範囲内に制限します
			pos.x = Mathf.Clamp(pos.x, lookBounds.xMin, lookBounds.xMax);
			pos.z = Mathf.Clamp(pos.z, lookBounds.yMin, lookBounds.yMax);
			lookPosition = pos;

			// 注視位置、視線ベクトル、ズーム距離からカメラ位置を計算します
			cameraPosition = lookPosition + (GetToCamVector() * zoomDist);
		}

		/// <summary>
		/// 指定した値だけカメラをズームします
		/// </summary>
		/// <param name="zoomDelta">カメラをズームする量</param>
		public void ZoomCameraRelative(float zoomDelta)
		{
			SetZoom(rawZoomDist + zoomDelta);
		}

		/// <summary>
		/// カメラを指定した値までズームします
		/// </summary>
		/// <param name="newZoom">絶対ズーム値</param>
		public void SetZoom(float newZoom)
		{
			if (springyZoom)
			{
				rawZoomDist = newZoom;

				if (newZoom > furthestZoom)
				{
					zoomDist = furthestZoom;
					zoomDist += Mathf.Log((Mathf.Min(rawZoomDist, maxZoom) - furthestZoom) + 1, zoomLogFactor);
				}
				else if (rawZoomDist < nearestZoom)
				{
					zoomDist = nearestZoom;
					zoomDist -= Mathf.Log((nearestZoom - rawZoomDist) + 1, zoomLogFactor);
				}
				else
				{
					zoomDist = rawZoomDist;
				}
			}
			else
			{
				zoomDist = rawZoomDist = Mathf.Clamp(newZoom, nearestZoom, furthestZoom);
			}

			// ズームレベルに基づく境界矩形を更新します
			RecalculateBoundingRect();

			// CameraPositionを強制的に再計算します
			PanCamera(Vector3.zero);
		}

		/// <summary>
		/// 指定したポインターに対応する3D空間上のRayを計算します
		/// </summary>
		/// <param name="pointer">ポインター情報</param>
		/// <returns>画面空間のポインターを3D空間で表すRay</returns>
		public Ray GetRayForPointer(PointerInfo pointer)
		{
			return cachedCamera.ScreenPointToRay(pointer.currentPosition);
		}

		/// <summary>
		/// 指定したワールド座標のスクリーン座標を取得します
		/// </summary>
		/// <param name="worldPos">ワールド座標</param>
		/// <returns>その点のスクリーン座標</returns>
		public Vector3 GetScreenPos(Vector3 worldPos)
		{
			return cachedCamera.WorldToScreenPoint(worldPos);
		}

		/// <summary>
		/// 弾力感を出すため、ズーム範囲を超えた場合はズームを減衰させます
		/// </summary>
		public void ZoomDecay()
		{
			if (springyZoom)
			{
				if (rawZoomDist > furthestZoom)
				{
					float recover = rawZoomDist - furthestZoom;
					SetZoom(Mathf.Max(furthestZoom, rawZoomDist - (recover * zoomRecoverSpeed * Time.deltaTime)));
				}
				else if (rawZoomDist < nearestZoom)
				{
					float recover = nearestZoom - rawZoomDist;
					SetZoom(Mathf.Min(nearestZoom, rawZoomDist + (recover * zoomRecoverSpeed * Time.deltaTime)));
				}
			}
		}

		/// <summary>
		/// 正規化したズーム比率を返します
		/// </summary>
		public float CalculateZoomRatio()
		{
			return Mathf.Clamp01(Mathf.InverseLerp(nearestZoom, furthestZoom, zoomDist));
		}

		/// <summary>
		/// 現在のズームレベルに基づいてカメラ方向へのベクトルを取得します
		/// </summary>
		Vector3 GetToCamVector()
		{
			float t = Mathf.Clamp01((zoomDist - nearestZoom) / (furthestZoom - nearestZoom));
			t = 1 - ((1 - t) * (1 - t));
			Quaternion interpolatedRotation = Quaternion.Slerp(
				m_MaxZoomRotation, m_MinZoomRotation,
				t);
			return interpolatedRotation * Vector3.up;
		}

		/// <summary>
		/// カメラの境界矩形サイズを更新します
		/// </summary>
		void RecalculateBoundingRect()
		{
			Rect mapsize = mapSize;

			// このズームレベルでのワールド空間への投影を取得します
			// カメラを最終的な注視位置へ一時的に移動します
			Vector3 prevCameraPos = transform.position;
			transform.position = cameraPosition;
			transform.LookAt(lookPosition);

			// 画面の四隅と中心を投影します
			var bottomLeftScreen = new Vector3(0, 0);
			var topLeftScreen = new Vector3(0, Screen.height);
			var centerScreen = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);

			Vector3 bottomLeftWorld = Vector3.zero;
			Vector3 topLeftWorld = Vector3.zero;
			Vector3 centerWorld = Vector3.zero;
			float dist;

			Ray ray = cachedCamera.ScreenPointToRay(bottomLeftScreen);
			if (m_FloorPlane.Raycast(ray, out dist))
			{
				bottomLeftWorld = ray.GetPoint(dist);
			}

			ray = cachedCamera.ScreenPointToRay(topLeftScreen);
			if (m_FloorPlane.Raycast(ray, out dist))
			{
				topLeftWorld = ray.GetPoint(dist);
			}

			ray = cachedCamera.ScreenPointToRay(centerScreen);
			if (m_FloorPlane.Raycast(ray, out dist))
			{
				centerWorld = ray.GetPoint(dist);
			}

			Vector3 toTopLeft = topLeftWorld - centerWorld;
			Vector3 toBottomLeft = bottomLeftWorld - centerWorld;

			lookBounds = new Rect(
				mapsize.xMin - toBottomLeft.x,
				mapsize.yMin - toBottomLeft.z,
				Mathf.Max(mapsize.width + (toBottomLeft.x * 2), 0),
				Mathf.Max((mapsize.height - toTopLeft.z) + toBottomLeft.z, 0));

			// カメラ位置を元に戻します
			transform.position = prevCameraPos;
			transform.LookAt(currentLookPosition);
		}
	}
}