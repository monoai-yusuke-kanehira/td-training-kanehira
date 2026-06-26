namespace TowerDefense.Level
{
	/// <summary>
	/// さまざまなレベルの状態を表す列挙型
	/// </summary>
	public enum LevelState
	{
		Intro,
		Building,
		SpawningEnemies,
		AllEnemiesSpawned,
		Lose,
		Win
	}
}