using System.Collections.Generic;
using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// どの所属にダメージを与えられるかを定義するシンプルなScriptableObjectです。
	/// SimpleAlignmentではない所属にはダメージを与えられません
	/// </summary>
	[CreateAssetMenu(fileName = "Alignment.asset", menuName = "StarterKit/Simple Alignment", order = 1)]
	public class SimpleAlignment : ScriptableObject, IAlignmentProvider
	{
		/// <summary>
		/// ダメージを与えられる他の所属オブジェクトのコレクション
		/// </summary>
		public List<SimpleAlignment> opponents;

		/// <summary>
		/// 指定した所属が既知の敵リストに含まれているかどうかを取得します
		/// </summary>
		public bool CanHarm(IAlignmentProvider other)
		{
			if (other == null)
			{
				return true;
			}
			
			var otherAlignment = other as SimpleAlignment;
			
			return otherAlignment != null && opponents.Contains(otherAlignment);
		}
	}
}