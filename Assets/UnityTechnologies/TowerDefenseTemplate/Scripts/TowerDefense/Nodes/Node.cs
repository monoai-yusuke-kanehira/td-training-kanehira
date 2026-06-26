using TowerDefense.Agents;
using TowerDefense.MeshCreator;
using UnityEngine;

namespace TowerDefense.Nodes
{
	/// <summary>
	/// AgentがNodeSelectorから次の指示を受け取る前に向かう、経路上の地点
	/// Colliderを手動で追加する必要があります。
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class Node : MonoBehaviour
	{
		/// <summary>
		/// AreaMeshCreatorによって作成されたMeshObjectへの参照
		/// </summary>
		[HideInInspector]
		public AreaMeshCreator areaMesh;

		/// <summary>
		/// Nodeの選択重み
		/// </summary>
		public int weight = 1;

		/// <summary>
		/// Selectorから次のNodeを取得します
		/// </summary>
		/// <returns>次のNode。終端Nodeの場合はnull</returns>
		public Node GetNextNode()
		{
			var selector = GetComponent<NodeSelector>();
			if (selector != null)
			{
				return selector.GetNextNode();
			}
			return null;
		}

		/// <summary>
		/// NodeのMeshCreatorで定義された範囲内のランダムな点を取得します
		/// </summary>
		/// <returns>MeshObjectの範囲内にあるランダムな点</returns>
		public Vector3 GetRandomPointInNodeArea()
		{
			// メッシュがない場合は自身の位置を代わりに使います
			return areaMesh == null ? transform.position : areaMesh.GetRandomPointInside();
		}

		/// <summary>
		/// AgentがNodeの範囲に入ったとき、次のNodeを取得します
		/// </summary>
		public virtual void OnTriggerEnter(Collider other)
		{
			var agent = other.gameObject.GetComponent<Agent>();
			if (agent != null)
			{
				agent.GetNextNode(this);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// ColliderがTriggerになるようにします
		/// </summary>
		protected void OnValidate()
		{
			var trigger = GetComponent<Collider>();
			if (trigger != null)
			{
				trigger.isTrigger = true;
			}
			
			// AreaMeshCreatorを探します
			if (areaMesh == null)
			{
				areaMesh = GetComponentInChildren<AreaMeshCreator>();
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position + Vector3.up, "movement_node.png", true);
		}
#endif
	}
}