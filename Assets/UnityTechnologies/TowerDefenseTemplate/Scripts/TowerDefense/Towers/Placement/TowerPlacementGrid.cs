using System;
using Core.Utilities;
using TowerDefense.UI.HUD;
using UnityEngine;

namespace TowerDefense.Towers.Placement
{
	/// <summary>
	/// グリッドで構成されたTower配置場所。
	/// 原点は右下セルの中央にあります。任意の向きに回転できます
	/// </summary>
	[RequireComponent(typeof(BoxCollider))]
	public class TowerPlacementGrid : MonoBehaviour, IPlacementArea
	{
		/// <summary>
		/// グリッド表示に使用するPrefab
		/// </summary>
		public PlacementTile placementTilePrefab;
		
		/// <summary>
		/// モバイル環境で生成する表示用Prefab
		/// </summary>
		public PlacementTile placementTilePrefabMobile;

		/// <summary>
		/// グリッドの寸法
		/// </summary>
		public IntVector2 dimensions;

		/// <summary>
		/// セル1辺のサイズ
		/// </summary>
		[Tooltip("The size of the edge of one grid cell for this area. Should match the physical grid size of towers")]
		public float gridSize = 1;

		/// <summary>
		/// 乗算に使うグリッドサイズの逆数
		/// </summary>
		float m_InvGridSize;

		/// <summary>
		/// 利用可能なセルの配列
		/// </summary>
		bool[,] m_AvailableCells;

		/// <summary>
		/// <see cref="PlacementTile"/>の配列
		/// </summary>
		PlacementTile[,] m_Tiles;

		/// <summary>
		/// ワールド空間の位置をローカルのグリッド座標に変換します。
		/// </summary>
		/// <param name="worldLocation">変換するワールド空間座標を示す<see cref="Vector3"/>。</param>
		/// <param name="sizeOffset">中央に合わせるオブジェクトのサイズを示す<see cref="IntVector2"/>。</param>
		/// <returns>この位置に対応するグリッド座標を含む<see cref="IntVector2"/>。</returns>
		public IntVector2 WorldToGrid(Vector3 worldLocation, IntVector2 sizeOffset)
		{
			Vector3 localLocation = transform.InverseTransformPoint(worldLocation);

			// グリッドサイズの逆数でスケールします
			localLocation *= m_InvGridSize;

			// 半分のサイズ分だけオフセットします
			var offset = new Vector3(sizeOffset.x * 0.5f, 0.0f, sizeOffset.y * 0.5f);
			localLocation -= offset;

			int xPos = Mathf.RoundToInt(localLocation.x);
			int yPos = Mathf.RoundToInt(localLocation.z);

			return new IntVector2(xPos, yPos);
		}

		/// <summary>
		/// グリッド位置に対応するワールド座標を返します。
		/// </summary>
		/// <param name="gridPosition">グリッド空間での座標</param>
		/// <param name="sizeOffset">中央に合わせるオブジェクトのサイズを示す<see cref="IntVector2"/>。</param>
		/// <returns>指定したグリッドセルのワールド座標を含むVector3。</returns>
		public Vector3 GridToWorld(IntVector2 gridPosition, IntVector2 sizeOffset)
		{
			// スケール済みのローカル位置を計算します
			Vector3 localPos = new Vector3(gridPosition.x + (sizeOffset.x * 0.5f), 0, gridPosition.y + (sizeOffset.y * 0.5f)) *
			                   gridSize;

			return transform.TransformPoint(localPos);
		}

		/// <summary>
		/// 指定したセル範囲が有効な配置場所かテストします。
		/// </summary>
		/// <param name="gridPos">グリッド上の位置</param>
		/// <param name="size">アイテムのサイズ</param>
		/// <returns>指定した範囲が配置に有効かどうか。</returns>
		public TowerFitStatus Fits(IntVector2 gridPos, IntVector2 size)
		{
			// Towerのタイルサイズが配置エリアの寸法を超える場合、すぐに配置不可にします。
			if ((size.x > dimensions.x) || (size.y > dimensions.y))
			{
				return TowerFitStatus.OutOfBounds;
			}

			IntVector2 extents = gridPos + size;

			// 範囲外です
			if ((gridPos.x < 0) || (gridPos.y < 0) ||
			    (extents.x > dimensions.x) || (extents.y > dimensions.y))
			{
				return TowerFitStatus.OutOfBounds;
			}

			// Towerの占有タイル内に既存のTowerがないことを確認します。
			for (int y = gridPos.y; y < extents.y; y++)
			{
				for (int x = gridPos.x; x < extents.x; x++)
				{
					if (m_AvailableCells[x, y])
					{
						return TowerFitStatus.Overlaps;
					}
				}
			}

			// ここまで到達した場合、有効な位置です。
			return TowerFitStatus.Fits;
		}

		/// <summary>
		/// セル範囲をTowerが占有中として設定します。
		/// </summary>
		/// <param name="gridPos">グリッド上の位置</param>
		/// <param name="size">アイテムのサイズ</param>
		public void Occupy(IntVector2 gridPos, IntVector2 size)
		{
			IntVector2 extents = gridPos + size;

			// 寸法とサイズを検証します
			if ((size.x > dimensions.x) || (size.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("size", "Given dimensions do not fit in our grid");
			}

			// 範囲外です
			if ((gridPos.x < 0) || (gridPos.y < 0) ||
			    (extents.x > dimensions.x) || (extents.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("gridPos", "Given footprint is out of range of our grid");
			}

			// 該当する位置を埋めます
			for (int y = gridPos.y; y < extents.y; y++)
			{
				for (int x = gridPos.x; x < extents.x; x++)
				{
					m_AvailableCells[x, y] = true;
					
					// 配置タイルがある場合はクリアします
					if (m_Tiles != null && m_Tiles[x, y] != null)
					{
						m_Tiles[x, y].SetState(PlacementTileState.Filled);
					}
				}
			}
		}

		/// <summary>
		/// グリッドからTowerを取り除き、そのセルを未使用に設定します。
		/// </summary>
		/// <param name="gridPos">グリッド上の位置</param>
		/// <param name="size">アイテムのサイズ</param>
		public void Clear(IntVector2 gridPos, IntVector2 size)
		{
			IntVector2 extents = gridPos + size;

			// 寸法とサイズを検証します
			if ((size.x > dimensions.x) || (size.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("size", "Given dimensions do not fit in our grid");
			}

			// 範囲外です
			if ((gridPos.x < 0) || (gridPos.y < 0) ||
			    (extents.x > dimensions.x) || (extents.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("gridPos", "Given footprint is out of range of our grid");
			}

			// 該当する位置を埋めます
			for (int y = gridPos.y; y < extents.y; y++)
			{
				for (int x = gridPos.x; x < extents.x; x++)
				{
					m_AvailableCells[x, y] = false;
					
					// 配置タイルがある場合はクリアします
					if (m_Tiles != null && m_Tiles[x, y] != null)
					{
						m_Tiles[x, y].SetState(PlacementTileState.Empty);
					}
				}
			}
		}

		/// <summary>
		/// 値を初期化します
		/// </summary>
		protected virtual void Awake()
		{
			ResizeCollider();

			// 空のbool配列を初期化します（既定値はfalseで、この用途に合っています）
			m_AvailableCells = new bool[dimensions.x, dimensions.y];

			// 座標変換のたびに除算しなくて済むよう、グリッドサイズの逆数を事前計算します
			m_InvGridSize = 1 / gridSize;

			SetUpGrid();
		}

		/// <summary>
		/// Colliderのサイズと中心を設定します
		/// </summary>
		void ResizeCollider()
		{
			var myCollider = GetComponent<BoxCollider>();
			Vector3 size = new Vector3(dimensions.x, 0, dimensions.y) * gridSize;
			myCollider.size = size;

			// Colliderの原点は左下の角です
			myCollider.center = size * 0.5f;
		}

		/// <summary>
		/// グリッド表示用のTileオブジェクトを生成し、<see cref="m_AvailableCells" />を設定します
		/// </summary>
		protected void SetUpGrid()
		{		
			PlacementTile tileToUse;
#if UNITY_STANDALONE
			tileToUse = placementTilePrefab;
#else
			tileToUse = placementTilePrefabMobile;
#endif
			
			if (tileToUse != null)
			{
				// セルを保持するコンテナを作成します。
				var tilesParent = new GameObject("Container");
				tilesParent.transform.parent = transform;
				tilesParent.transform.localPosition = Vector3.zero;
				tilesParent.transform.localRotation = Quaternion.identity;
				m_Tiles  = new PlacementTile[dimensions.x, dimensions.y];
				
				for (int y = 0; y < dimensions.y; y++)
				{
					for (int x = 0; x < dimensions.x; x++)
					{
						Vector3 targetPos = GridToWorld(new IntVector2(x, y), new IntVector2(1, 1));
						targetPos.y += 0.01f;
						PlacementTile newTile = Instantiate(tileToUse);
						newTile.transform.parent = tilesParent.transform;
						newTile.transform.position = targetPos;
						newTile.transform.localRotation = Quaternion.identity;

						m_Tiles[x, y] = newTile;
						newTile.SetState(PlacementTileState.Empty);
					}
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// EditorまたはInspectorでの検証時に、Colliderのサイズが正しいことを確認します。
		/// また、設定を誤って変更されないようにColliderコンポーネントを非表示にし、整合性を保ちます。
		/// ユーザーがこれらの値を変更する必要がないことも示します。
		/// </summary>
		void OnValidate()
		{
			// グリッドサイズを検証します
			if (gridSize <= 0)
			{
				Debug.LogError("Negative or zero grid size is invalid");
				gridSize = 1;
			}

			// 寸法を検証します
			if (dimensions.x <= 0 ||
			    dimensions.y <= 0)
			{
				Debug.LogError("Negative or zero grid dimensions are invalid");
				dimensions = new IntVector2(Mathf.Max(dimensions.x, 1), Mathf.Max(dimensions.y, 1));
			}

			// Colliderが正しいサイズになるようにします
			ResizeCollider();

			GetComponent<BoxCollider>().hideFlags = HideFlags.HideInInspector;
		}

		/// <summary>
		/// Sceneビューにグリッドを描画します
		/// </summary>
		void OnDrawGizmos()
		{
			Color prevCol = Gizmos.color;
			Gizmos.color = Color.cyan;

			Matrix4x4 originalMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;

			// ローカル空間で平らなキューブを描画します
			for (int y = 0; y < dimensions.y; y++)
			{
				for (int x = 0; x < dimensions.x; x++)
				{
					var position = new Vector3((x + 0.5f) * gridSize, 0, (y + 0.5f) * gridSize);
					Gizmos.DrawWireCube(position, new Vector3(gridSize, 0, gridSize));
				}
			}

			Gizmos.matrix = originalMatrix;
			Gizmos.color = prevCol;
			
			// 位置の中央にアイコンも描画します
			Vector3 center = transform.TransformPoint(new Vector3(gridSize * dimensions.x * 0.5f,
			                                                      1,
			                                                      gridSize * dimensions.y * 0.5f));
			Gizmos.DrawIcon(center, "build_zone.png", true);
		}
#endif
	}
}