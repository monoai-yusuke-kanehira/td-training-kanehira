using System;

namespace Core.Utilities
{
	/// <summary>
	/// シリアライズ可能なインターフェースラッパーオブジェクトの抽象基底クラス
	/// </summary>
	public abstract class SerializableInterface
	{
		/// <summary>
		/// このインターフェース型としてシリアライズされるUnityコンポーネント
		/// </summary>
		public UnityEngine.Object unityObjectReference;
	}

	/// <summary>
	/// UnityのGameObjectでインターフェースをシリアライズできるようにする汎用的な仕組み
	/// </summary>
	/// <typeparam name="T">ISerializableInterfaceを実装した任意のインターフェース</typeparam>
	[Serializable]
	public class SerializableInterface<T> : SerializableInterface where T: ISerializableInterface
	{
		T m_InterfaceReference;
		
		/// <summary>
		/// Unityコンポーネントからインターフェースを取得してキャッシュする
		/// </summary>
		public T GetInterface()
		{
			if (m_InterfaceReference == null && unityObjectReference != null)
			{
				m_InterfaceReference = (T)(ISerializableInterface)unityObjectReference;
			}

			return m_InterfaceReference;
		}
	}

	/// <summary>
	/// すべてのシリアライズ可能なインターフェースが継承する必要がある基底インターフェース
	/// </summary>
	public interface ISerializableInterface
	{
	}
}