using System;
using Core.Extensions;
using UnityEngine;

namespace ActionGameFramework.Spawning
{
	/// <summary>
	/// 重み付きオブジェクトのリスト
	/// </summary>
	[Serializable]
	public class WeightedObjectList
	{
		/// <summary>
		/// 重み付きの項目
		/// </summary>
		public WeightedObject[] weightedItems;

		/// <summary>
		/// 項目の重みの合計
		/// </summary>
		protected int m_WeightSum = -1;

		/// <summary>
		/// 重みの合計を取得します。
		/// </summary>
		/// <value>重みの合計。</value>
		public int weightSum
		{
			get
			{
				if (m_WeightSum < 0)
				{
					CalculateWeightSum();
				}

				return m_WeightSum;
			}
		}

		/// <summary>
		/// 重みに基づいてランダムなGameObjectを返します
		/// </summary>
		/// <returns>選択された項目。</returns>
		public GameObject WeightedSelection()
		{
			if (weightedItems.Length == 0)
			{
				return null;
			}

			WeightedObject item = weightedItems.WeightedSelection(weightSum, t => t.weight);
			return item.gameObject;
		}

		/// <summary>
		/// 重みの合計を計算します。
		/// </summary>
		protected void CalculateWeightSum()
		{
			m_WeightSum = 0;
			int count = weightedItems.Length;
			for (int i = 0; i < count; i++)
			{
				m_WeightSum += weightedItems[i].weight;
			}
		}
	}
}