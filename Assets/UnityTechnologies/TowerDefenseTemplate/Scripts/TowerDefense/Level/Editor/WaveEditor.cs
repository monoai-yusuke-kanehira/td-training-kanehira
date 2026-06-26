using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace TowerDefense.Level.Editor
{
	/// <summary>
	/// 波形時間の合計を表示するカスタムエディター
	/// </summary>
	[CustomEditor(typeof(Wave), true)]
	public class WaveEditor : UnityEditor.Editor
	{
		Wave m_Wave;

		void OnEnable()
		{
			m_Wave = (Wave) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			// すべてのスポーン命令の概要を描画します
			List<SpawnInstruction> spawnInstructions = m_Wave.spawnInstructions;
			if (spawnInstructions == null)
			{
				return;
			}
			
			// カウントスポーン命令
			float lastSpawnTime = spawnInstructions.Sum(t => t.delayToSpawn);

			// 種類ごとにも数えられるよう、敵タイプでグループ化します
			var groups = spawnInstructions.GroupBy(t => t.agentConfiguration);
			var groupCounts = groups.Select(g => new {Number = g.Count(), Item = g.Key.agentName});

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Wave summary");

			EditorGUILayout.LabelField(string.Format("Last spawn time: {0}", lastSpawnTime));
			EditorGUILayout.Space();
			foreach (var groupCount in groupCounts)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(string.Format("Enemy:\t{0}", groupCount.Item));
				EditorGUILayout.LabelField(string.Format("Count:\t{0}", groupCount.Number));
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
