using UnityEngine;
using UnityEngine.PostProcessing;

namespace TowerDefense.Cameras
{
	/// <summary>
	/// モバイル上で低品質の後処理構成を選択するためのシンプルなコンポーネント
	/// </summary>
	[RequireComponent(typeof(PostProcessingBehaviour))]
	public class PostProcessorConfigurationSelector : MonoBehaviour
	{
		public PostProcessingProfile highQualityProfile;
		
		public PostProcessingProfile lowQualityProfile;

		protected virtual void Awake()
		{
			var attachedPostProcessor = GetComponent<PostProcessingBehaviour>();

			PostProcessingProfile selectedProfile;

#if UNITY_STANDALONE
			selectedProfile = highQualityProfile;
#else
			selectedProfile = lowQualityProfile;
#endif

			attachedPostProcessor.profile = selectedProfile;
		}
	}
}