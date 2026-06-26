using System.Collections.Generic;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.UI
{
	public class RadiusVisualizerController : MonoBehaviour
	{
		/// <summary>
		/// タワーの効果範囲を可視化するために使用するPrefab
		/// </summary>
		public GameObject radiusVisualizerPrefab;

		public float radiusVisualizerHeight = 0.02f;

		/// <summary>
		/// ローカルのオイラー角
		/// </summary>
		public Vector3 localEuler;

		readonly List<GameObject> m_RadiusVisualizers = new List<GameObject>();

		/// <summary>
		/// タワーまたはゴーストタワー用の範囲ビジュアライザーを設定する
		/// </summary>
		/// <param name="tower">
		/// データを取得するタワー
		/// </param>
		/// <param name="ghost">ビジュアライザーの親にするゴーストのTransform。</param>
		public void SetupRadiusVisualizers(Tower tower, Transform ghost = null)
		{
			// 必要な影響範囲の可視化オブジェクトを作成する
			List<ITowerRadiusProvider> providers =
				tower.levels[tower.currentLevel].GetRadiusVisualizers();

			int length = providers.Count;
			for (int i = 0; i < length; i++)
			{
				if (m_RadiusVisualizers.Count < i + 1)
				{
					m_RadiusVisualizers.Add(Instantiate(radiusVisualizerPrefab));
				}

				ITowerRadiusProvider provider = providers[i];

				GameObject radiusVisualizer = m_RadiusVisualizers[i];
				radiusVisualizer.SetActive(true);
				radiusVisualizer.transform.SetParent(ghost == null ? tower.transform : ghost);
				radiusVisualizer.transform.localPosition = new Vector3(0, radiusVisualizerHeight, 0);
				radiusVisualizer.transform.localScale = Vector3.one * provider.effectRadius * 2.0f;
				radiusVisualizer.transform.localRotation = new Quaternion {eulerAngles = localEuler};

				var visualizerRenderer = radiusVisualizer.GetComponent<Renderer>();
				if (visualizerRenderer != null)
				{
					visualizerRenderer.material.color = provider.effectColor;
				}
			}
		}

		/// <summary>
		/// 範囲ビジュアライザーを非表示にする
		/// </summary>
		public void HideRadiusVisualizers()
		{
			foreach (GameObject radiusVisualizer in m_RadiusVisualizers)
			{
				radiusVisualizer.transform.parent = transform;
				radiusVisualizer.SetActive(false);
			}
		}
	}
}
