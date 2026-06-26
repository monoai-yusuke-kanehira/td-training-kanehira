using UnityEngine;

namespace TowerDefense.Agents.Data
{
	[CreateAssetMenu(fileName = "AgentConfiguration.asset", menuName = "TowerDefense/Agent Configuration", order = 1)]
	public class AgentConfiguration : ScriptableObject
	{
		/// <summary>
		/// エージェントの名前
		/// </summary>
		public string agentName;

		/// <summary>
		/// エージェントの簡単な概要
		/// </summary>
		[Multiline]
		public string agentDescription;

		/// <summary>
		/// インスタンス化で使用されるエージェント プレハブ
		/// </summary>
		public Agent agentPrefab;
	}
}