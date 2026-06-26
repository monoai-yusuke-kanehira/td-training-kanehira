namespace Core.Data
{
	/// <summary>
	/// GameManagerが保存する基本データストアです。音量保存用のデータだけを含みます
	/// </summary>
	public abstract class GameDataStoreBase : IDataStore
	{
		public float masterVolume = 1;

		public float sfxVolume = 1;

		public float musicVolume = 1;

		/// <summary>
		/// 保存直前に呼び出されます
		/// </summary>
		public abstract void PreSave();

		/// <summary>
		/// 読み込み直後に呼び出されます
		/// </summary>
		public abstract void PostLoad();
	}
}