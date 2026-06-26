using System;
using Core.Utilities;

namespace Core.Health
{
	/// <summary>
	/// ダメージ判定用のチームや所属を提供できるオブジェクトのインターフェース
	/// </summary>
	public interface IAlignmentProvider : ISerializableInterface
	{
		/// <summary>
		/// この所属が別の所属にダメージを与えられるかどうかを取得します
		/// </summary>
		bool CanHarm(IAlignmentProvider other);
	}

	/// <summary>
	/// 上記インターフェースのシリアライズ可能な具象版
	/// </summary>
	[Serializable]
	public class SerializableIAlignmentProvider : SerializableInterface<IAlignmentProvider>
	{
	}
}