using TowerDefense.Level;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// ウェーブのフィードバックを表示するクラス
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class WaveUI : MonoBehaviour
	{
		/// <summary>
		/// 情報を表示するText要素
		/// </summary>
		public Text display;

		public Image waveFillImage;

		/// <summary>
		/// このレベルのウェーブ総数
		/// </summary>
		protected int m_TotalWaves;

		protected Canvas m_Canvas;

		/// <summary>
		/// ウェーブ総数をキャッシュし、
		/// 表示を更新して
		/// waveChangedを購読する
		/// </summary>
		protected virtual void Start()
		{
			m_Canvas = GetComponent<Canvas>();
			m_Canvas.enabled = false;
			m_TotalWaves = LevelManager.instance.waveManager.totalWaves;
			LevelManager.instance.waveManager.waveChanged += UpdateDisplay;
		}

		/// <summary>
		/// 現在のウェーブ数を表示に書き込む
		/// </summary>
		protected void UpdateDisplay()
		{
			m_Canvas.enabled = true;
			int currentWave = LevelManager.instance.waveManager.waveNumber;
			string output = string.Format("{0}/{1}", currentWave, m_TotalWaves);
			display.text = output;
		}

		protected virtual void Update()
		{
			waveFillImage.fillAmount = LevelManager.instance.waveManager.waveProgress;
		}

		/// <summary>
		/// イベントの購読を解除する
		/// </summary>
		protected void OnDestroy()
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.waveManager.waveChanged -= UpdateDisplay;
			}
		}
	}
}
