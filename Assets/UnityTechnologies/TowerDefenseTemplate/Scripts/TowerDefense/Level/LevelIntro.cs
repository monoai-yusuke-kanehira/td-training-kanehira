using System;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// レベルのイントロを表す抽象基本クラス
	/// </summary>
	public abstract class LevelIntro : MonoBehaviour
	{
		/// <summary>
		/// イントロが完了すると呼び出されます
		/// </summary>
		public event Action introCompleted;

		/// <summary>
		/// イントロが完了したことをマークするために、派生クラスによって起動される必要があります
		/// </summary>
		protected void SafelyCallIntroCompleted()
		{
			if (introCompleted != null)
			{
				introCompleted();
			}
		}
	}
}