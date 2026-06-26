using System;
using System.Collections.Generic;
using UnityDebug = UnityEngine.Debug;
using UnityRandom = UnityEngine.Random;

namespace Core.Extensions
{
	/// <summary>
	/// IList用の拡張メソッド
	/// </summary>
	public static class IListExtensions
	{
		static readonly Random s_SharedRandom = new Random();

		/// <summary>
		/// 重み付き選択を使ってリストから項目を選びます。
		/// </summary>
		/// <remarks>これは O(n) の処理で、均等なランダム選択のような定数時間の処理ではありません。</remarks>
		/// <param name="elements">選択対象となる要素の <see cref="System.Collections.Generic.IList{T}" /></param>
		/// <param name="weightSum">すべての要素の重みの合計</param>
		/// <param name="getElementWeight">特定の要素の重みを取得するためのデリゲート</param>
		/// <returns><paramref name="elements" /> からランダムに選ばれた要素</returns>
		public static T WeightedSelection<T>(this IList<T> elements, int weightSum, Func<T, int> getElementWeight)
		{
			int index = elements.WeightedSelectionIndex(weightSum, getElementWeight);
			return elements[index];
		}

		/// <summary>
		/// 重み付き選択を使ってリストから項目を選びます。
		/// </summary>
		/// <remarks>これは O(n) の処理で、均等なランダム選択のような定数時間の処理ではありません。</remarks>
		/// <param name="elements">選択対象となる要素の <see cref="System.Collections.Generic.IList{T}" /></param>
		/// <param name="weightSum">すべての要素の重みの合計</param>
		/// <param name="getElementWeight">特定の要素の重みを取得するためのデリゲート</param>
		/// <returns><paramref name="elements" /> からランダムに選ばれた要素</returns>
		public static T WeightedSelection<T>(this IList<T> elements, float weightSum, Func<T, float> getElementWeight)
		{
			int index = elements.WeightedSelectionIndex(weightSum, getElementWeight);
			return elements[index];
		}

		/// <summary>
		/// 重み付き選択を使ってリストから項目のインデックスを選びます。
		/// </summary>
		/// <remarks>これは O(n) の処理で、均等なランダム選択のような定数時間の処理ではありません。</remarks>
		/// <param name="elements">選択対象となる要素の <see cref="System.Collections.Generic.IList{T}" /></param>
		/// <param name="weightSum">すべての要素の重みの合計</param>
		/// <param name="getElementWeight">特定の要素の重みを取得するためのデリゲート</param>
		/// <returns><paramref name="elements" /> からランダムに選ばれた要素のインデックス</returns>
		public static int WeightedSelectionIndex<T>(this IList<T> elements, int weightSum, Func<T, int> getElementWeight)
		{
			if (weightSum <= 0)
			{
				throw new ArgumentException("WeightSum should be a positive value", "weightSum");
			}

			int selectionIndex = 0;
			int selectionWeightIndex = UnityRandom.Range(0, weightSum);
			int elementCount = elements.Count;

			if (elementCount == 0)
			{
				throw new InvalidOperationException("Cannot perform selection on an empty collection");
			}

			int itemWeight = getElementWeight(elements[selectionIndex]);
			while (selectionWeightIndex >= itemWeight)
			{
				selectionWeightIndex -= itemWeight;
				selectionIndex++;

				if (selectionIndex >= elementCount)
				{
					throw new ArgumentException("Weighted selection exceeded indexable range. Is your weightSum correct?",
					                            "weightSum");
				}

				itemWeight = getElementWeight(elements[selectionIndex]);
			}

			return selectionIndex;
		}

		/// <summary>
		/// 重み付き選択を使ってリストから項目のインデックスを選びます。
		/// </summary>
		/// <remarks>これは O(n) の処理で、均等なランダム選択のような定数時間の処理ではありません。</remarks>
		/// <param name="elements">選択対象となる要素の <see cref="System.Collections.Generic.IList{T}" /></param>
		/// <param name="weightSum">すべての要素の重みの合計</param>
		/// <param name="getElementWeight">特定の要素の重みを取得するためのデリゲート</param>
		/// <returns><paramref name="elements" /> からランダムに選ばれた要素のインデックス</returns>
		public static int WeightedSelectionIndex<T>(this IList<T> elements, float weightSum, Func<T, float> getElementWeight)
		{
			if (weightSum <= 0)
			{
				throw new ArgumentException("WeightSum should be a positive value", "weightSum");
			}

			int selectionIndex = 0;

			double selectedWeight = s_SharedRandom.NextDouble() * weightSum;
			int elementCount = elements.Count;

			if (elementCount == 0)
			{
				throw new InvalidOperationException("Cannot perform selection on an empty collection");
			}

			double itemWeight = getElementWeight(elements[selectionIndex]);
			while (selectedWeight >= itemWeight)
			{
				selectedWeight -= itemWeight;
				selectionIndex++;

				if (selectionIndex >= elementCount)
				{
					throw new ArgumentException("Weighted selection exceeded indexable range. Is your weightSum correct?",
					                            "weightSum");
				}

				itemWeight = getElementWeight(elements[selectionIndex]);
			}

			return selectionIndex;
		}

		/// <summary>
		/// このListをシャッフルして新しい配列コピーにします
		/// </summary>
		public static T[] Shuffle<T>(this IList<T> original)
		{
			int numItems = original.Count;
			T[] result = new T[numItems];

			for (int i = 0; i < numItems; ++i)
			{
				int j = UnityRandom.Range(0, i + 1);

				if (j != i)
				{
					result[i] = result[j];
				}

				result[j] = original[i];
			}

			return result;
		}

		/// <summary>
		/// リストの次の要素へ進みます
		/// </summary>
		/// <param name="elements">選択対象となる要素の <see cref="System.Collections.Generic.IList{T}" /></param>
		/// <param name="currentIndex">参照渡しで変更される現在のインデックス</param>
		/// <param name="wrap">リストの末尾から先頭へ回り込むかどうか</param>
		/// <typeparam name="T">リストのジェネリック型</typeparam>
		/// <returns>リスト内に次の要素がある場合は true</returns>
		public static bool Next<T>(this IList<T> elements, ref int currentIndex, bool wrap = false)
		{
			int count = elements.Count;
			if (count == 0)
			{
				return false;
			}

			currentIndex++;

			if (currentIndex >= count)
			{
				if (wrap)
				{
					currentIndex = 0;
					return true;
				}
				currentIndex = count - 1;
				return false;
			}

			return true;
		}

		/// <summary>
		/// リストの前の要素へ戻ります
		/// </summary>
		/// <param name="elements">選択対象となる要素の <see cref="System.Collections.Generic.IList{T}" /></param>
		/// <param name="currentIndex">参照渡しで変更される現在のインデックス</param>
		/// <param name="wrap">リストの先頭から末尾へ回り込むかどうか</param>
		/// <typeparam name="T">リストのジェネリック型</typeparam>
		/// <returns>リスト内に前の要素がある場合は true</returns>
		public static bool Prev<T>(this IList<T> elements, ref int currentIndex, bool wrap = false)
		{
			int count = elements.Count;
			if (count == 0)
			{
				return false;
			}

			currentIndex--;

			if (currentIndex < 0)
			{
				if (wrap)
				{
					currentIndex = count - 1;
					return true;
				}
				currentIndex = 0;
				return false;
			}

			return true;
		}
	}
}
