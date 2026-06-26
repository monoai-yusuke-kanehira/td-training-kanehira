namespace Core.Data
{
	/// <summary>
	/// データストア用のインターフェース
	/// </summary>
	public interface IDataStore
	{
		void PreSave();

		void PostLoad();
	}
}