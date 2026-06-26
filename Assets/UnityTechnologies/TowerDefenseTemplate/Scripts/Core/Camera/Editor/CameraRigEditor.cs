using UnityEditor;
using UnityEngine;

namespace Core.Camera.Editor
{
	[CustomEditor(typeof(CameraRig))]
	public class CameraRigEditor : UnityEditor.Editor
	{
		CameraRig m_CameraRig;
		Rect m_MapSize;
		SerializedProperty m_SerializedPropertyMapSize;

		/// <summary>
		/// デフォルトのInspector GUIの下にラベルを追加します
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Resize the map rect size using the handles in scene.", EditorStyles.boldLabel);
		}

		/// <summary>
		/// マップサイズを操作するための表示と入力処理を行います
		/// </summary>
		void OnSceneGUI()
		{
			float y = m_CameraRig.floorY;
			Plane floor = new Plane(Vector3.up, y);

			float middleX = (m_MapSize.xMax + m_MapSize.xMin) * 0.5f;
			float middleY = (m_MapSize.yMax + m_MapSize.yMin) * 0.5f;
			
			Vector3 bottomPosition = new Vector3(middleX, y, m_MapSize.yMin),
			        topPosition = new Vector3(middleX, y, m_MapSize.yMax),
			        leftPosition = new Vector3(m_MapSize.xMin, y, middleY),
			        rightPosition = new Vector3(m_MapSize.xMax, y, middleY);

			// マップ矩形のサイズ変更用ハンドルを描画します
			float size = HandleUtility.GetHandleSize(m_CameraRig.transform.position) * 0.125f;
			Vector3 snap = Vector3.one * 0.5f;
			var fmh_42_60_639081509944640010 = Quaternion.LookRotation(Vector3.up); Vector3 bottom = Handles.FreeMoveHandle(bottomPosition, size, snap,
			                                        Handles.RectangleHandleCap);
			var fmh_44_54_639081509944645620 = Quaternion.LookRotation(Vector3.up); Vector3 top = Handles.FreeMoveHandle(topPosition, size, snap,
			                                     Handles.RectangleHandleCap);
			var fmh_46_56_639081509944647190 = Quaternion.LookRotation(Vector3.up); Vector3 left = Handles.FreeMoveHandle(leftPosition, size, snap,
			                                      Handles.RectangleHandleCap);
			var fmh_48_58_639081509944648460 = Quaternion.LookRotation(Vector3.up); Vector3 right = Handles.FreeMoveHandle(rightPosition, size, snap,
			                                       Handles.RectangleHandleCap);
			
			ReprojectOntoFloor(ref bottom, floor);
			ReprojectOntoFloor(ref top, floor);
			ReprojectOntoFloor(ref left, floor);
			ReprojectOntoFloor(ref right, floor);

			// マップ矩形を表すボックスを描画します
			Vector3 topLeft = new Vector3(m_MapSize.x, y, m_MapSize.y),
			        topRight = topLeft + new Vector3(m_MapSize.width, 0, 0),
			        bottomLeft = topLeft + new Vector3(0, 0, m_MapSize.height),
			        bottomRight = bottomLeft + new Vector3(m_MapSize.width, 0, 0);
			Handles.DrawLine(topLeft, topRight);
			Handles.DrawLine(topRight, bottomRight);
			Handles.DrawLine(bottomRight, bottomLeft);
			Handles.DrawLine(bottomLeft, topLeft);

			m_MapSize.xMin = left.x;
			m_MapSize.xMax = right.x;
			m_MapSize.yMin = bottom.z;
			m_MapSize.yMax = top.z;

			if (m_SerializedPropertyMapSize.rectValue != m_MapSize)
			{
				m_SerializedPropertyMapSize.rectValue = m_MapSize;
				serializedObject.ApplyModifiedProperties();
			}
		}

		/// <summary>
		/// 移動した位置を床平面へ再投影します（3D空間で移動するため操作が重く感じることがあります
		/// 新しい位置を取得して平面のY座標へ戻すように制限する場合）
		/// </summary>
		static void ReprojectOntoFloor(ref Vector3 worldPoint, Plane floor)
		{
			UnityEngine.Camera sceneCam = UnityEngine.Camera.current;
			if (sceneCam != null)
			{
				float dist;
				Ray camray = sceneCam.ScreenPointToRay(sceneCam.WorldToScreenPoint(worldPoint));
				if (floor.Raycast(camray, out dist))
				{
					worldPoint = camray.GetPoint(dist);
				}
			}
		}

		/// <summary>
		/// 編集用のserializedObjectを取得します
		/// </summary>
		void OnEnable()
		{
			m_CameraRig = target as CameraRig;
			m_SerializedPropertyMapSize = serializedObject.FindProperty("mapSize");
			m_MapSize = m_SerializedPropertyMapSize.rectValue;
		}
	}
}