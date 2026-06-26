using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense.MeshCreator
{
	/// <summary>
	/// エリアを表すメッシュを作成します
	/// </summary>
	[Serializable]
	public class AreaMeshCreator : MonoBehaviour
	{
		[HideInInspector]
		public MeshObject meshObject;

		public Transform outSidePointsParent;

		/// <summary>
		/// メッシュ内のポイントの親変換
		/// </summary>
		public Transform pointsCenter
		{
			get
			{
				if (outSidePointsParent == null)
				{
					var points = new GameObject("Points");
					outSidePointsParent = points.transform;
					outSidePointsParent.SetParent(transform, false);
					outSidePointsParent.eulerAngles = new Vector3(90, 0, 0);
				}
#if UNITY_EDITOR
				outSidePointsParent.hideFlags = HideFlags.HideInHierarchy;
#endif
				return outSidePointsParent;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// このメッシュ内のポイントの変換の配列を取得します - エディター スクリプトでのみ使用されます
		/// </summary>
		public Transform[] pointsTransforms
		{
			get
			{
				Transform[] childern = new Transform[pointsCenter.childCount];
				int length = pointsCenter.childCount;
				for (int i = 0; i < length; i++)
				{
					childern[i] = pointsCenter.GetChild(i);
				}
				return childern;
			}
		}
#endif

		/// <summary>
		/// このメッシュ内の点の位置に対応する Vector3 のリストを取得します
		/// </summary>
		/// <returns>ポイント一覧</returns>
		public List<Vector3> GetPoints()
		{
			return GetChildrenPositions(pointsCenter);
		}

		/// <summary>
		/// メッシュ オブジェクト内にあるランダムな Vector3 を取得します
		/// </summary>
		/// <returns>ランダムポイント</returns>
		public Vector3 GetRandomPointInside()
		{
			return transform.TransformPoint(meshObject.RandomPointInMesh());
		}

		/// <summary>
		/// すべての点のローカル "y" 位置が 0 になるように強制します
		/// ポイントを同一平面上にそろえます
		/// </summary>
		public void ForcePointsFlat()
		{
			int length = pointsCenter.childCount;
			for (int i = 0; i < length; i++)
			{
				Transform t = pointsCenter.GetChild(i);
				Vector3 position = t.localPosition;
				position.z = 0;
				t.localPosition = position;
			}
		}

		List<Vector3> GetChildrenPositions(Transform parent)
		{
			int length = parent.childCount;
			List<Vector3> points = new List<Vector3>();
			for (int i = 0; i < length; i++)
			{
				points.Add(parent.GetChild(i).position);
			}
			return points;
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			int count = pointsCenter.childCount;
			for (int i = 0; i < count - 1; i++)
			{
				Vector3 from = pointsCenter.GetChild(i).position;
				Vector3 to = pointsCenter.GetChild(i + 1).position;
				Gizmos.DrawLine(from, to);
			}
			// 最後から最初へ
			Vector3 last = pointsCenter.GetChild(count - 1).position;
			Vector3 first = pointsCenter.GetChild(0).position;
			Gizmos.DrawLine(last, first);
		}
#endif
	}

	[Serializable]
	public class Triangle
	{
		public Vector3 v0;

		public Vector3 v1;
		
		public Vector3 v2;

		public float area;

		/// <summary>
		/// メッシュ内の三角形を表します
		/// </summary>
		/// <param name="v0">最初のポイント</param>
		/// <param name="v1">2 番目のポイント</param>
		/// <param name="v2">3番目のポイント</param>
		public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			this.v0 = v0;
			this.v1 = v1;
			this.v2 = v2;

			// 面積を事前計算します
			float a = Vector3.Distance(v0, v1), b = Vector3.Distance(v1, v2), c = Vector3.Distance(v2, v0);
			float s = (a + b + c) / 2;
			area = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
		}
	}

	/// <summary>
	/// メッシュの三角形が含まれます
	/// メッシュ領域を計算します
	/// メッシュ領域内のランダムな点を取得できます
	/// </summary>
	[Serializable]
	public class MeshObject
	{
		public List<Triangle> triangles;

		public float completeArea;

		public MeshObject(List<Triangle> triangles)
		{
			this.triangles = triangles;
			completeArea = this.triangles.Sum(x => x.area);
		}

		/// <summary>
		/// メッシュ内のランダムな点を取得します
		/// </summary>
		/// <returns>ランダムポイント</returns>
		public Vector3 RandomPointInMesh()
		{
			Triangle randomTriangle = triangles.WeightedSelection(completeArea, t => t.area);
			float x = Random.value, y = Random.value;
			if (x + y >= 1)
			{
				x = 1 - x;
				y = 1 - y;
			}
			float z = 1 - x - y;
			var randomBaryCentricPoint = new Vector3(x, y, z);
			Vector3 cartesianPoint = (randomBaryCentricPoint.x * randomTriangle.v0) + (randomBaryCentricPoint.y *
			                                                                           randomTriangle.v1) +
			                         (randomBaryCentricPoint.z * randomTriangle.v2);
			return cartesianPoint;
		}
	}
}
