using System;
using Core.Extensions;
using UnityEngine;

namespace ActionGameFramework.Audio
{
	/// <summary>
	/// 重み付き Audio リスト
	/// </summary>
	[Serializable]
	public class WeightedAudioList
	{
		/// <summary>
		/// 各項目と対応する重み
		/// </summary>
		public WeightedAudioClip[] weightedItems;

		/// <summary>
		/// すべての項目の重みの合計
		/// </summary>
		protected int m_WeightSum = -1;

		/// <summary>
		/// 重みの合計を取得する
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
		/// 重み付きリストからランダムな AudioClip を取得する
		/// </summary>
		/// <returns>選ばれた AudioClip。</returns>
		public AudioClip WeightedSelection()
		{
			if (weightedItems.Length == 0)
			{
				return null;
			}

			WeightedAudioClip item = weightedItems.WeightedSelection(weightSum, t => t.weight);
			return item.clip;
		}

		/// <summary>
		/// すべての項目の重みの合計を計算する
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
