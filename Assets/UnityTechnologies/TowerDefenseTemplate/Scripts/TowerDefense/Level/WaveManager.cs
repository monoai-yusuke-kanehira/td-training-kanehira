using System;
using System.Collections.Generic;
using Core.Extensions;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// WaveManager - ウェーブの初期化と完了を処理します
	/// </summary>
	public class WaveManager : MonoBehaviour
	{
		/// <summary>
		/// 現在使用されている波形
		/// </summary>
		protected int m_CurrentIndex;

		/// <summary>
		/// WaveManager が Awake で wave を開始するかどうか - LevelManager がこの関数を呼び出す必要があるため、デフォルトは null です
		/// </summary>
		public bool startWavesOnAwake;

		/// <summary>
		/// 順番に走る波
		/// </summary>
		[Tooltip("Specify this list in order")]
		public List<Wave> waves = new List<Wave>();

		/// <summary>
		/// 現在の波数
		/// </summary>
		public int waveNumber
		{
			get { return m_CurrentIndex + 1; }
		}

		/// <summary>
		/// 波の総数
		/// </summary>
		public int totalWaves
		{
			get { return waves.Count; }
		}

		public float waveProgress
		{
			get
			{
				if (waves == null || waves.Count <= m_CurrentIndex)
				{
					return 0;
				}
				return waves[m_CurrentIndex].progress;
			}
		}

		/// <summary>
		/// Wave の開始時に呼び出されます
		/// </summary>
		public event Action waveChanged;

		/// <summary>
		/// すべての Wave が終了したときに呼び出されます
		/// </summary>
		public event Action spawningCompleted;

		/// <summary>
		/// 波を起こす
		/// </summary>
		public virtual void StartWaves()
		{
			if (waves.Count > 0)
			{
				InitCurrentWave();
			}
			else
			{
				Debug.LogWarning("[LEVEL] No Waves on wave manager. Calling spawningCompleted");
				SafelyCallSpawningCompleted();
			}
		}

		/// <summary>
		/// 最初のウェーブを開始します
		/// </summary>
		protected virtual void Awake()
		{
			if (startWavesOnAwake)
			{
				StartWaves();
			}
		}

		/// <summary>
		/// 次のウェーブを設定します
		/// </summary>
		protected virtual void NextWave()
		{
			waves[m_CurrentIndex].waveCompleted -= NextWave;
			if (waves.Next(ref m_CurrentIndex))
			{
				InitCurrentWave();
			}
			else
			{
				SafelyCallSpawningCompleted();
			}
		}

		/// <summary>
		/// 現在の波形を初期化します
		/// </summary>
		protected virtual void InitCurrentWave()
		{
			Wave wave = waves[m_CurrentIndex];
			wave.waveCompleted += NextWave;
			wave.Init();
			if (waveChanged != null)
			{
				waveChanged();
			}
		}

		/// <summary>
		/// spawningCompleted イベントを呼び出します
		/// </summary>
		protected virtual void SafelyCallSpawningCompleted()
		{
			if (spawningCompleted != null)
			{
				spawningCompleted();
			}
		}
	}
}