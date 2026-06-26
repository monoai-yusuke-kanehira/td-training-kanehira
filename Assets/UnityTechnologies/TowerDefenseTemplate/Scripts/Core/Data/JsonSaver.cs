using System.IO;
using UnityEngine;

namespace Core.Data
{
	/// <summary>
	/// ファイル保存処理のJSON実装
	/// </summary>
	public class JsonSaver<T> : FileSaver<T> where T : IDataStore
	{
		public JsonSaver(string filename)
			: base(filename)
		{
		}

		/// <summary>
		/// 指定したデータストアを保存します
		/// </summary>
		public override void Save(T data)
		{
			string json = JsonUtility.ToJson(data);

			using (StreamWriter writer = GetWriteStream())
			{
				writer.Write(json);
			}
		}

		/// <summary>
		/// 指定したデータストアを読み込みます
		/// </summary>
		public override bool Load(out T data)
		{
			if (!File.Exists(m_Filename))
			{
				data = default(T);
				return false;
			}

			using (StreamReader reader = GetReadStream())
			{
				data = JsonUtility.FromJson<T>(reader.ReadToEnd());
			}

			return true;
		}
	}
}