using Core.Utilities;
using TowerDefense.UI.HUD;
using UnityEngine;

namespace TowerDefense.Towers.Placement
{
	/// <summary>
	/// 単一のTower配置に適したエリア
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class SingleTowerPlacementArea : MonoBehaviour, IPlacementArea
	{
		/// <summary>
		/// 生成する表示用Prefab
		/// </summary>
		public PlacementTile placementTilePrefab;
		
		/// <summary>
		/// モバイル環境で生成する表示用Prefab
		/// </summary>
		public PlacementTile placementTilePrefabMobile;
		
		/// <summary>
		/// この場所に生成した<see cref="PlacementTile"/>
		/// </summary>
		PlacementTile m_SpawnedTile;

		/// <summary>
		/// エリアが占有されているかどうか
		/// </summary>
		bool m_IsOccupied;

		/// <summary>
		/// 表示用タイルを設定します
		/// </summary>
		protected void Awake()
		{
			PlacementTile tileToUse;
#if UNITY_STANDALONE
			tileToUse = placementTilePrefab;
#else
			tileToUse = placementTilePrefabMobile;
#endif
			
			if (tileToUse != null)
			{
				m_SpawnedTile = Instantiate(tileToUse);
				m_SpawnedTile.transform.SetParent(transform);
				m_SpawnedTile.transform.localPosition = new Vector3(0f, 0.05f, 0f);
			}
		}

		/// <summary>
		/// 利用可能な場所が1つだけなので、(0, 0)を返します
		/// </summary>
		/// <param name="worldPosition">変換するワールド空間座標を示す<see cref="Vector3"/>。</param>
		/// <param name="sizeOffset">中央に合わせるオブジェクトのサイズを示す<see cref="IntVector2"/>。</param>
		public IntVector2 WorldToGrid(Vector3 worldPosition, IntVector2 sizeOffset)
		{
			return new IntVector2(0, 0);
		}

		/// <summary>
		/// 利用可能な場所が1つだけなので、transform.positionを返します
		/// </summary>
		/// <param name="gridPosition">グリッド空間での座標</param>
		/// <param name="sizeOffset">中央に合わせるオブジェクトのサイズを示す<see cref="IntVector2"/>。</param>
		public Vector3 GridToWorld(IntVector2 gridPosition, IntVector2 sizeOffset)
		{
			return transform.position;
		}

		/// <summary>
		/// 配置エリアが有効かテストします。
		/// </summary>
		/// <param name="gridPos">グリッド上の位置</param>
		/// <param name="size">アイテムのサイズ</param>
		public TowerFitStatus Fits(IntVector2 gridPos, IntVector2 size)
		{
			return m_IsOccupied ? TowerFitStatus.Overlaps : TowerFitStatus.Fits;
		}

		/// <summary>
		/// エリアを占有します
		/// </summary>
		/// <param name="gridPos"></param>
		/// <param name="size"></param>
		public void Occupy(IntVector2 gridPos, IntVector2 size)
		{
			m_IsOccupied = true;

			if (m_SpawnedTile != null)
			{
				m_SpawnedTile.SetState(PlacementTileState.Filled);
			}
		}

		/// <summary>
		/// エリアをクリアします
		/// </summary>
		/// <param name="gridPos"></param>
		/// <param name="size"></param>
		public void Clear(IntVector2 gridPos, IntVector2 size)
		{
			m_IsOccupied = false;

			if (m_SpawnedTile != null)
			{
				m_SpawnedTile.SetState(PlacementTileState.Empty);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Sceneビューでこの場所を小さな球として描画します。
		/// </summary>
		void OnDrawGizmos()
		{
			Color prevCol = Gizmos.color;
			Gizmos.color = Color.cyan;

			Matrix4x4 originalMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;

			Gizmos.DrawWireSphere(Vector3.zero, 1);

			Gizmos.matrix = originalMatrix;
			Gizmos.color = prevCol;
			
			// アイコンも描画します
			Gizmos.DrawIcon(transform.position + Vector3.up, "build_zone.png", true);
		}
#endif
	}
}