namespace Core.Data
{
	/// <summary>
	/// データ保存用のインターフェース
	/// </summary>
	public interface IDataSaver<T> where T : IDataStore
	{
		void Save(T data);

		bool Load(out T data);

		void Delete();
	}
}