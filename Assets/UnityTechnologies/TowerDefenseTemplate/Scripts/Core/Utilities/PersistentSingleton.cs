namespace Core.Utilities
{
	/// <summary>
	/// 複数のシーンをまたいで存在し続けるSingleton
	/// </summary>
	public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
	{
		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
	}
}