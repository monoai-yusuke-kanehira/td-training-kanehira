using Core.Extensions;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace TowerDefense.Nodes
{
	/// <summary>
	/// リストに並んでいる順番でNodeを決定的に選択します
	/// </summary>
	public class FixedNodeSelector : NodeSelector
	{
		/// <summary>
		/// 次に選択するNodeを追跡するためのインデックス
		/// </summary>
		protected int m_NodeIndex;

		/// <summary>
		/// <see cref="m_NodeIndex" />を使って次のNodeを選択します
		/// </summary>
		/// <returns>次に選択されたNode。有効なNodeがない場合はnull</returns>
		public override Node GetNextNode()
		{
			if (linkedNodes.Next(ref m_NodeIndex, true))
			{
				return linkedNodes[m_NodeIndex];
			}
			return null;
		}

#if UNITY_EDITOR
		protected override void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			base.OnDrawGizmos();
		}
#endif
	}
}