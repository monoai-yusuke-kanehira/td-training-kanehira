using System;

namespace Core.Game
{
	/// <summary>
	/// レベルを表す要素
	/// </summary>
	[Serializable]
	public class LevelItem
	{
		/// <summary>
		/// ID。永続化で使用されます
		/// </summary>
		public string id;

		/// <summary>
		/// 人が読めるレベル名
		/// </summary>
		public string name;

		/// <summary>
		/// レベルの説明文
		/// </summary>
		public string description;

		/// <summary>
		/// 読み込むシーン名
		/// </summary>
		public string sceneName;
	}
}