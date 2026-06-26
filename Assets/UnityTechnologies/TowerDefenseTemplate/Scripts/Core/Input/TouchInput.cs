using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Core.Input
{
	/// <summary>
	/// タッチデバイス向けの基本操作スキーム。CameraRigを操作する
	/// </summary>
	public class TouchInput : CameraInputScheme
	{
		/// <summary>
		/// パン速度の設定
		/// </summary>
		public float panSpeed = 5;

		/// <summary>
		/// フリックの勢いがどれだけ速く減衰するか
		/// </summary>
		public float flickDecayFactor = 0.2f;

		/// <summary>
		/// フリック方向
		/// </summary>
		Vector3 m_FlickDirection;

		/// <summary>
		/// この操作スキームを有効にするかどうかを取得する
		/// </summary>
		public override bool shouldActivate
		{
			get { return UnityInput.touchCount > 0; }
		}

		/// <summary>
		/// iOSおよびAndroidデバイスでは、この操作スキームをデフォルトにする
		/// </summary>
		public override bool isDefault
		{
			get
			{
#if UNITY_IOS || UNITY_ANDROID
				return true;
#else
				return false;
#endif
			}
		}

		/// <summary>
		/// 入力イベントを登録する
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!InputController.instanceExists)
			{
				Debug.LogError("[UI] Keyboard and Mouse UI requires InputController");
				return;
			}
			
			// ドラッグイベントを登録する
			InputController inputController = InputController.instance;
			inputController.pressed += OnPress;
			inputController.released += OnRelease;
			inputController.dragged += OnDrag;
			inputController.pinched += OnPinch;
		}
		
		/// <summary>
		/// 入力イベントの登録を解除する
		/// </summary>
		protected virtual void OnDisable()
		{
			if (!InputController.instanceExists)
			{
				return;
			}
			
			if (InputController.instanceExists)
			{
				InputController inputController = InputController.instance;
				inputController.pressed -= OnPress;
				inputController.released -= OnRelease;
				inputController.dragged -= OnDrag;
				inputController.pinched -= OnPinch;
			}
		}

		/// <summary>
		/// フリックとズームを実行する
		/// </summary>
		protected virtual void Update()
		{
			if (cameraRig != null)
			{
				UpdateFlick();
				DecayZoom();
			}
		}

		/// <summary>
		/// 入力が押されたときに呼ばれる
		/// </summary>
		protected virtual void OnPress(PointerActionInfo pointer)
		{
			if (cameraRig != null)
			{
				DoFlickCatch(pointer);
			}
		}
		
		/// <summary>
		/// 入力が離されたときに呼ばれる
		/// </summary>
		protected virtual void OnRelease(PointerActionInfo pointer)
		{
			if (cameraRig != null)
			{
				DoReleaseFlick(pointer);
			}
		}

		/// <summary>
		/// ドラッグしたときに呼ばれる
		/// </summary>
		protected virtual void OnDrag(PointerActionInfo pointer)
		{
			// タッチ入力のドラッグでパンする
			if (cameraRig != null)
			{
				DoDragPan(pointer);
			}
		}

		/// <summary>
		/// ピンチ操作で呼ばれる
		/// </summary>
		protected virtual void OnPinch(PinchInfo pinch)
		{
			if (cameraRig != null)
			{
				DoPinchZoom(pinch);
			}
		}

		/// <summary>
		/// 現在のフリック速度を更新する
		/// </summary>
		protected void UpdateFlick()
		{
			// フリック中か
			if (m_FlickDirection.sqrMagnitude > Mathf.Epsilon)
			{
				cameraRig.PanCamera(m_FlickDirection * Time.deltaTime);
				m_FlickDirection *= flickDecayFactor;
			}
		}

		/// <summary>
		/// アクティブなタッチがなければズームを減衰させる
		/// </summary>
		protected void DecayZoom()
		{
			if (InputController.instance.activeTouchCount == 0)
			{
				cameraRig.ZoomDecay();
			}
		}

		/// <summary>
		/// 押されたときにフリックを「捕まえて」、パンの勢いを止める
		/// </summary>
		/// <param name="pointer">押下時のポインターイベント</param>
		protected void DoFlickCatch(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;
			// タッチ時にフリックを止める
			if (touchInfo != null)
			{
				m_FlickDirection = Vector2.zero;
				cameraRig.StopTracking();
			}
		}
		
		/// <summary>
		/// 離されたときだけフリックを行う
		/// </summary>
		/// <param name="pointer">リリース時のポインターイベント</param>
		protected void DoReleaseFlick(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;

			if (touchInfo != null && touchInfo.flickVelocity.sqrMagnitude > Mathf.Epsilon)
			{
				// フリックが発生している
				// 動きから速度を計算する
				Ray prevRay = cameraRig.cachedCamera.ScreenPointToRay(pointer.currentPosition -
																		pointer.flickVelocity);
				Ray currRay = cameraRig.cachedCamera.ScreenPointToRay(pointer.currentPosition);

				Vector3 startPoint = Vector3.zero;
				Vector3 endPoint = Vector3.zero;
				float dist;

				if (cameraRig.floorPlane.Raycast(prevRay, out dist))
				{
					startPoint = prevRay.GetPoint(dist);
				}
				if (cameraRig.floorPlane.Raycast(currRay, out dist))
				{
					endPoint = currRay.GetPoint(dist);
				}

				// その移動量を1秒あたりの単位に変換する
				m_FlickDirection = (startPoint - endPoint) / Time.deltaTime;
			}
		}

		/// <summary>
		/// ドラッグでパンを制御する
		/// </summary>
		protected void DoDragPan(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;
			if (touchInfo != null)
			{
				// 差分位置から床平面へレイキャストする
				// その距離から移動量を計算する
				Ray currRay = cameraRig.cachedCamera.ScreenPointToRay(touchInfo.currentPosition);

				Vector3 endPoint = Vector3.zero;
				float dist;
				if (cameraRig.floorPlane.Raycast(currRay, out dist))
				{
					endPoint = currRay.GetPoint(dist);
				}
				// パンする
				Ray prevRay = cameraRig.cachedCamera.ScreenPointToRay(touchInfo.previousPosition);
				Vector3 startPoint = Vector3.zero;

				if (cameraRig.floorPlane.Raycast(prevRay, out dist))
				{
					startPoint = prevRay.GetPoint(dist);
				}
				Vector3 panAmount = startPoint - endPoint;
				// タッチ入力の場合は、パン量をタッチ数で割る
				if (UnityInput.touchCount > 0)
				{
					panAmount /= UnityInput.touchCount;
				}
				
				PanCamera(panAmount);
			}
		}
		
		/// <summary>
		/// 指定されたピンチ操作でズームする
		/// </summary>
		protected void DoPinchZoom(PinchInfo pinch)
		{
			float currentDistance = (pinch.touch1.currentPosition - pinch.touch2.currentPosition).magnitude;
			float prevDistance = (pinch.touch1.previousPosition - pinch.touch2.previousPosition).magnitude;

			float zoomChange = prevDistance / currentDistance;
			float prevZoomDist = cameraRig.zoomDist;

			cameraRig.SetZoom(zoomChange * cameraRig.rawZoomDist);

			// クランプ後の実際のズーム変化量を計算する
			zoomChange = cameraRig.zoomDist / prevZoomDist;

			// まずジェスチャー中央の床上の位置を取得する
			Vector2 averageScreenPos = (pinch.touch1.currentPosition + pinch.touch2.currentPosition) * 0.5f;
			Ray ray = cameraRig.cachedCamera.ScreenPointToRay(averageScreenPos);

			Vector3 worldPos = Vector3.zero;
			float dist;

			if (cameraRig.floorPlane.Raycast(ray, out dist))
			{
				worldPos = ray.GetPoint(dist);
			}

			// 現在の注視位置からこの点までのベクトル
			Vector3 offsetValue = worldPos - cameraRig.lookPosition;

			// ズーム中心に近づく、または離れるようにパンする
			PanCamera(offsetValue * (1 - zoomChange));
		}
		
		/// <summary>
		/// カメラをパンする
		/// </summary>
		/// <param name="panAmount">
		/// パンに使うベクトル
		/// </param>
		protected void PanCamera(Vector3 panAmount)
		{
			cameraRig.StopTracking();
			cameraRig.PanCamera(panAmount);
		}
	}
}
