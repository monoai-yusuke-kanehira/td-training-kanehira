using Core.Extensions;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace TowerDefense.Nodes
{
	/// <summary>
	/// 次のNodeをランダムに選択します
	/// </summary>
	public class RandomNodeSelector : NodeSelector
	{
		/// <summary>
		/// m_LinkedNodes内の全Nodeの重みの合計
		/// </summary>
		protected int m_WeightSum;

		/// <summary>
		/// リスト内からランダムなNodeを取得します
		/// </summary>
		/// <returns>ランダムに選択されたNode</returns>
		public override Node GetNextNode()
		{
			if (linkedNodes == null)
			{
				return null;
			}
			int totalWeight = m_WeightSum;
			return linkedNodes.WeightedSelection(totalWeight, t => t.weight);
		}

		protected void Awake()
		{
			// リンクされたNodeの重みをキャッシュします
			m_WeightSum = TotalLinkedNodeWeights();
		}
#if UNITY_EDITOR
		protected override void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			base.OnDrawGizmos();
		}
#endif
		/// <summary>
		/// ランダム選択のためにリンクされたNodeの重みを合計します
		/// </summary>
		/// <returns>リンクされたNodeの重みの合計</returns>
		protected int TotalLinkedNodeWeights()
		{
			int totalWeight = 0;
			int count = linkedNodes.Count;
			for (int i = 0; i < count; i++)
			{
				totalWeight += linkedNodes[i].weight;
			}
			return totalWeight;
		}
	}
}