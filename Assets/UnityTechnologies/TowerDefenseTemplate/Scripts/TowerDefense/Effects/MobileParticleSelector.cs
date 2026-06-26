using UnityEngine;

namespace TowerDefense.Effects
{
	/// <summary>
	/// 状況に応じて 2 つの別個のゲーム オブジェクトを切り替えるための単純なクラス
	/// モバイルプラットフォーム上かどうか
	/// </summary>
	public class MobileParticleSelector : MonoBehaviour
	{
		/// <summary>
		/// 非モバイルプラットフォームで使用するシステム
		/// </summary>
		public ParticleSystem defaultParticles;
		/// <summary>
		/// モバイルプラットフォームで使用するシステム
		/// </summary>
		public ParticleSystem mobileParticles;


		protected virtual void Awake()
		{
			ParticleSystem selectedSystem;
			
#if UNITY_STANDALONE
			selectedSystem = defaultParticles;
#else
			selectedSystem = mobileParticles;
#endif

			defaultParticles.gameObject.SetActive(selectedSystem == defaultParticles);
			mobileParticles.gameObject.SetActive(selectedSystem == mobileParticles);
		}
	}
}