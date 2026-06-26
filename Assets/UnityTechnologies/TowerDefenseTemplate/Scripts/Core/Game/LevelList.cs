using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Game
{
	/// <summary>
	/// レベル設定用のScriptableObject
	/// </summary>
	[CreateAssetMenu(fileName = "LevelList", menuName = "StarterKit/Create Level List", order = 1)]
	public class LevelList : ScriptableObject, IList<LevelItem>,
	                         IDictionary<string, LevelItem>, 
	                         ISerializationCallbackReceiver
	{
		public LevelItem[] levels;

		/// <summary>
		/// IDごとにレベルをキャッシュしたDictionary
		/// </summary>
		IDictionary<string, LevelItem> m_LevelDictionary;

		/// <summary>
		/// レベル数を取得します
		/// </summary>
		public int Count
		{
			get { return levels.Length; }
		}

		/// <summary>
		/// レベルリストは常に読み取り専用です
		/// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// インデックスでレベルを取得します
		/// </summary>
		public LevelItem this[int i]
		{
			get { return levels[i]; }
		}

		/// <summary>
		/// IDでレベルを取得します
		/// </summary>
		public LevelItem this[string key]
		{
			get { return m_LevelDictionary[key]; }
		}

		/// <summary>
		/// すべてのレベルキーのコレクションを取得します
		/// </summary>
		public ICollection<string> Keys
		{
			get { return m_LevelDictionary.Keys; }
		}

		/// <summary>
		/// 指定したレベルのインデックスを取得します
		/// </summary>
		public int IndexOf(LevelItem item)
		{
			if (item == null)
			{
				return -1;
			}

			for (int i = 0; i < levels.Length; ++i)
			{
				if (levels[i] == item)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// このレベルがリストに存在するかどうかを取得します
		/// </summary>
		public bool Contains(LevelItem item)
		{
			return IndexOf(item) >= 0;
		}

		/// <summary>
		/// 指定したIDのレベルが存在するかどうかを取得します
		/// </summary>
		public bool ContainsKey(string key)
		{
			return m_LevelDictionary.ContainsKey(key);
		}

		/// <summary>
		/// 指定したキーでレベルの取得を試みます
		/// </summary>
		public bool TryGetValue(string key, out LevelItem value)
		{
			return m_LevelDictionary.TryGetValue(key, out value);
		}

		/// <summary>
		/// 指定したシーンに関連付けられた <see cref="LevelItem"/> を取得します
		/// </summary>
		public LevelItem GetLevelByScene(string scene)
		{
			for (int i = 0; i < levels.Length; ++i)
			{
				LevelItem item = levels[i];
				if (item != null &&
				    item.sceneName == scene)
				{
					return item;
				}
			}
			
			return null;
		}

		// 明示的なインターフェース実装
		// Dictionaryを作成するためのシリアライズイベント
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			m_LevelDictionary = levels.ToDictionary(l => l.id);
		}
		
		ICollection<LevelItem> IDictionary<string, LevelItem>.Values
		{
			get { return m_LevelDictionary.Values; }
		}

		LevelItem IList<LevelItem>.this[int i]
		{
			get { return levels[i]; }
			set { throw new NotSupportedException("Level List is read only"); }
		}

		LevelItem IDictionary<string, LevelItem>.this[string key]
		{
			get { return m_LevelDictionary[key]; }
			set { throw new NotSupportedException("Level List is read only"); }
		}

		void IList<LevelItem>.Insert(int index, LevelItem item)
		{
			throw new NotSupportedException("Level List is read only");
		}

		void IList<LevelItem>.RemoveAt(int index)
		{
			throw new NotSupportedException("Level List is read only");
		}

		void ICollection<LevelItem>.Add(LevelItem item)
		{
			throw new NotSupportedException("Level List is read only");
		}

		void ICollection<KeyValuePair<string, LevelItem>>.Add(KeyValuePair<string, LevelItem> item)
		{
			throw new NotSupportedException("Level List is read only");
		}

		void ICollection<KeyValuePair<string, LevelItem>>.Clear()
		{
			throw new NotSupportedException("Level List is read only");
		}

		bool ICollection<KeyValuePair<string, LevelItem>>.Contains(KeyValuePair<string, LevelItem> item)
		{
			return m_LevelDictionary.Contains(item);
		}

		void ICollection<KeyValuePair<string, LevelItem>>.CopyTo(KeyValuePair<string, LevelItem>[] array, int arrayIndex)
		{
			m_LevelDictionary.CopyTo(array, arrayIndex);
		}

		void ICollection<LevelItem>.Clear()
		{
			throw new NotSupportedException("Level List is read only");
		}

		void ICollection<LevelItem>.CopyTo(LevelItem[] array, int arrayIndex)
		{
			levels.CopyTo(array, arrayIndex);
		}

		bool ICollection<LevelItem>.Remove(LevelItem item)
		{
			throw new NotSupportedException("Level List is read only");
		}

		public IEnumerator<LevelItem> GetEnumerator()
		{
			return ((IList<LevelItem>) levels).GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, LevelItem>> IEnumerable<KeyValuePair<string, LevelItem>>.GetEnumerator()
		{
			return m_LevelDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return levels.GetEnumerator();
		}

		void IDictionary<string, LevelItem>.Add(string key, LevelItem value)
		{
			throw new NotSupportedException("Level List is read only");
		}

		bool ICollection<KeyValuePair<string, LevelItem>>.Remove(KeyValuePair<string, LevelItem> item)
		{
			throw new NotSupportedException("Level List is read only");
		}

		bool IDictionary<string, LevelItem>.Remove(string key)
		{
			throw new NotSupportedException("Level List is read only");
		}
	}
}