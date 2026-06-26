using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// 配置するタワーの位置と、その配置が有効かどうかを示すタワー配置用の「ゴースト」。
	/// テスト用にマウス操作を想定して作られているが、タッチUI向けには多くの処理を子クラスへ
	/// 抽象化できるはず。
	/// 
	/// 最適な配置判定のため、専用のレイヤーに置く必要がある。
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class TowerPlacementGhost : MonoBehaviour
	{
		/// <summary>
		/// このゴーストが表すタワー
		/// </summary>
		public Tower controller { get; private set; }

		/// <summary>
		/// タワーの効果範囲を可視化するために使用するPrefab
		/// </summary>
		public GameObject radiusVisualizer;

		/// <summary>
		/// 範囲ビジュアライザーの高さオフセット
		/// </summary>
		public float radiusVisualizerHeight = 0.02f;

		/// <summary>
		/// 移動の減衰係数
		/// </summary>
		public float dampSpeed = 0.075f;

		/// <summary>
		/// 有効な配置と無効な配置をそれぞれ表すために使用する2つのMaterial
		/// </summary>
		public Material material;
		
		public Material invalidPositionMaterial;

		/// <summary>
		/// アタッチされているMeshRendererの一覧
		/// </summary>
		protected MeshRenderer[] m_MeshRenderers;

		/// <summary>
		/// スムーズな減衰に使う移動速度
		/// </summary>
		protected Vector3 m_MoveVel;

		/// <summary>
		/// 目標のワールド座標
		/// </summary>
		protected Vector3 m_TargetPosition;

		/// <summary>
		/// 有効なワールド座標にいる場合はtrue
		/// </summary>
		protected bool m_ValidPos;

		/// <summary>
		/// アタッチされているCollider
		/// </summary>
		public Collider ghostCollider { get; private set; }

		/// <summary>
		/// このゴーストを初期化する
		/// </summary>
		/// <param name="tower">このゴーストが表すタワーコントローラー</param>
		public virtual void Initialize(Tower tower)
		{
			m_MeshRenderers = GetComponentsInChildren<MeshRenderer>();
			controller = tower;
			if (GameUI.instanceExists)
			{
				GameUI.instance.SetupRadiusVisualizer(controller, transform);
			}
			ghostCollider = GetComponent<Collider>();
			m_MoveVel = Vector3.zero;
			m_ValidPos = false;
		}

		/// <summary>
		/// このゴーストを非表示にする
		/// </summary>
		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// このゴーストを表示する
		/// </summary>
		public virtual void Show()
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
				m_MoveVel = Vector3.zero;

				m_ValidPos = false;
			}
		}

		/// <summary>
		/// このゴーストを指定されたワールド座標へ移動する
		/// </summary>
		/// <param name="worldPosition">移動先となる新しいワールド座標</param>
		/// <param name="rotation">適用する新しいワールド回転</param>
		/// <param name="validLocation">この位置が有効かどうか。無効な位置ではゴーストの表示が変わる場合がある</param>
		public virtual void Move(Vector3 worldPosition, Quaternion rotation, bool validLocation)
		{
			m_TargetPosition = worldPosition;

			if (!m_ValidPos)
			{
				// 指定された位置へ即座に移動する
				m_ValidPos = true;
				transform.position = m_TargetPosition;
			}
			
			transform.rotation = rotation;
			foreach (MeshRenderer meshRenderer in m_MeshRenderers)
			{
				meshRenderer.sharedMaterial = validLocation ? material : invalidPositionMaterial;
			}
		}


		/// <summary>
		/// ゴーストの移動を減衰させる
		/// </summary>
		protected virtual void Update()
		{
			Vector3 currentPos = transform.position;

			if (Vector3.SqrMagnitude(currentPos - m_TargetPosition) > 0.01f)
			{
				currentPos = Vector3.SmoothDamp(currentPos, m_TargetPosition, ref m_MoveVel, dampSpeed);

				transform.position = currentPos;
			}
			else
			{
				m_MoveVel = Vector3.zero;
			}
		}
	}
}
