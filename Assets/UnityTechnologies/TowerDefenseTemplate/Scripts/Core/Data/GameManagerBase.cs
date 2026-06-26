using System;
using Core.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Data
{
	/// <summary>
	/// 基本ゲームマネージャー
	/// </summary>
	public abstract class GameManagerBase<TGameManager, TDataStore> : PersistentSingleton<TGameManager>
		where TDataStore : GameDataStoreBase, new()
		where TGameManager : GameManagerBase<TGameManager, TDataStore>
	{
		/// <summary>
		/// 保存ゲームのファイル名
		/// </summary>
		const string k_SavedGameFile = "save";

		/// <summary>
		/// 音量変更に使うAudioMixerへの参照
		/// </summary>
		public AudioMixer gameMixer;

		/// <summary>
		/// ミキサーのマスター音量パラメーター
		/// </summary>
		public string masterVolumeParameter;

		/// <summary>
		/// ミキサーの効果音音量パラメーター
		/// </summary>
		public string sfxVolumeParameter;

		/// <summary>
		/// ミキサーの音楽音量パラメーター
		/// </summary>
		public string musicVolumeParameter;

		/// <summary>
		/// 永続化に使うシリアライズ実装
		/// </summary>
		protected JsonSaver<TDataStore> m_DataSaver;

		/// <summary>
		/// 永続化に使うオブジェクト
		/// </summary>
		protected TDataStore m_DataStore;

		/// <summary>
		/// データストアから音量を取得します
		/// </summary>
		public virtual void GetVolumes(out float master, out float sfx, out float music)
		{
			master = m_DataStore.masterVolume;
			sfx = m_DataStore.sfxVolume;
			music = m_DataStore.musicVolume;
		}

		/// <summary>
		/// ゲーム音量を設定して永続化します
		/// </summary>
		public virtual void SetVolumes(float master, float sfx, float music, bool save)
		{
			// ミキサーが設定されていない場合は早期終了します
			if (gameMixer == null)
			{
				return;
			}
			
			// 0から1の値を-80から0の対数値へ変換します
			if (masterVolumeParameter != null)
			{
				gameMixer.SetFloat(masterVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(master)));
			}
			if (sfxVolumeParameter != null)
			{
				gameMixer.SetFloat(sfxVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(sfx)));
			}
			if (musicVolumeParameter != null)
			{
				gameMixer.SetFloat(musicVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(music)));
			}

			if (save)
			{
				// 保存データにも適用します
				m_DataStore.masterVolume = master;
				m_DataStore.sfxVolume = sfx;
				m_DataStore.musicVolume = music;
				SaveData();
			}
		}

		/// <summary>
		/// データを読み込みます
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			LoadData();
		}

		/// <summary>
		/// 音量を初期化します。Awakeではミキサーのパラメーターを変更できません
		/// </summary>
		protected virtual void Start()
		{
			SetVolumes(m_DataStore.masterVolume, m_DataStore.sfxVolume, m_DataStore.musicVolume, false);
		}

		/// <summary>
		/// 永続化を設定します
		/// </summary>
		protected void LoadData()
		{
			// Unity Editor上ではデバッグしやすい標準JSONを使い、それ以外の配布版では暗号化します
#if UNITY_EDITOR
			m_DataSaver = new JsonSaver<TDataStore>(k_SavedGameFile);
#else
			m_DataSaver = new EncryptedJsonSaver<TDataStore>(k_SavedGameFile);
#endif

			try
			{
				if (!m_DataSaver.Load(out m_DataStore))
				{
					m_DataStore = new TDataStore();
					SaveData();
				}
			}
			catch (Exception)
			{
				Debug.Log("Failed to load data, resetting");
				m_DataStore = new TDataStore();
				SaveData();
			}
		}

		/// <summary>
		/// ゲームを保存します
		/// </summary>
		protected virtual void SaveData()
		{
			m_DataSaver.Save(m_DataStore);
		}

		/// <summary>
		/// 音量をリニア値から対数値へ変換します
		/// </summary>
		protected static float LogarithmicDbTransform(float volume)
		{
			volume = (Mathf.Log(89 * volume + 1) / Mathf.Log(90)) * 80;
			return volume - 80;
		}
	}
}