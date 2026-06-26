using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.UI
{
	/// <summary>
	/// 名前でシーンを読み込むシンプルなコンポーネント
	/// </summary>
	public class SceneLoader : MonoBehaviour
	{
		/// <summary>
		/// 読み込むシーンの名前
		/// </summary>
		public string sceneToLoadName = "LevelSelect";

		/// <summary>
		/// その名前のシーンが存在する場合、
		/// <see cref="sceneToLoadName" /> からシーンを読み込む
		/// </summary>
		public void LoadScene()
		{
			SceneManager.LoadScene(sceneToLoadName);
		}

		/// <summary>
		/// 現在のシーンを再読み込みする
		/// </summary>
		public void RestartCurrentScene()
		{
			Scene activeScene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(activeScene.name);
		}
	}
}
