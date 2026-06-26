using Core.UI;
using TowerDefense.Game;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// 音量を設定するためのシンプルなオプションメニュー
	/// </summary>
	public class OptionsMenu : SimpleMainMenuPage
	{
		public Slider masterSlider;

		public Slider sfxSlider;
		
		public Slider musicSlider;

		/// <summary>
		/// スライダーが変更されたときに発火するイベント
		/// </summary>
		public void UpdateVolumes()
		{
			float masterVolume, sfxVolume, musicVolume;
			GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume);

			if (GameManager.instanceExists)
			{
				GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, false);
			}
		}

		/// <summary>
		/// スライダーの初期値を設定する
		/// </summary>
		public override void Show()
		{
			if (GameManager.instanceExists)
			{
				float master, sfx, music;
				GameManager.instance.GetVolumes(out master, out sfx, out music);

				if (masterSlider != null)
				{
					masterSlider.value = master;
				}
				if (sfxSlider != null)
				{
					sfxSlider.value = sfx;
				}
				if (musicSlider != null)
				{
					musicSlider.value = music;
				}
			}

			base.Show();
		}

		/// <summary>
		/// 音量をデータストアへ保存する
		/// </summary>
		public override void Hide()
		{
			float masterVolume, sfxVolume, musicVolume;
			GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume);

			if (GameManager.instanceExists)
			{
				GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, true);
			}

			base.Hide();
		}

		/// <summary>
		/// スライダーから値を取得する
		/// </summary>
		void GetSliderVolumes(out float masterVolume, out float sfxVolume, out float musicVolume)
		{
			masterVolume = masterSlider != null ? masterSlider.value : 1;
			sfxVolume = sfxSlider != null ? sfxSlider.value : 1;
			musicVolume = musicSlider != null ? musicSlider.value : 1;
		}
	}
}
