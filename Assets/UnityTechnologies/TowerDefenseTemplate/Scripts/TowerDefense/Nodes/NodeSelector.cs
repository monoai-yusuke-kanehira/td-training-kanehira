using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Nodes
{
	/// <summary>
	/// Agentが向かうNodeを選択する仕組みを提供します
	/// </summary>
	public abstract class NodeSelector : MonoBehaviour
	{
		/// <summary>
		/// このNodeSelectorが選択できるNodeのリスト
		/// </summary>
		public List<Node> linkedNodes;

		/// <summary>
		/// 固定されたNodeリストから次のNodeを取得します
		/// </summary>
		/// <returns>Nodeリスト内の次のNode。終端Nodeの場合はnull</returns>
		public abstract Node GetNextNode();

#if UNITY_EDITOR
		/// <summary>
		/// Editor用にNode間のリンクを描画します
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			if (linkedNodes == null)
			{
				return;
			}
			int count = linkedNodes.Count;
			for (int i = 0; i < count; i++)
			{
				Node node = linkedNodes[i];
				if (node != null)
				{
					Gizmos.DrawLine(transform.position, node.transform.position);
				}
			}
		}
#endif
	}
}