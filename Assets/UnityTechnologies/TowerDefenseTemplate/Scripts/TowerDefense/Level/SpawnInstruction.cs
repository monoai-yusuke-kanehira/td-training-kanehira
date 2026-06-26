using System;
using TowerDefense.Agents.Data;
using TowerDefense.Nodes;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// エージェントを生成するプロパティを指定するための直列化可能なクラス
	/// </summary>
	[Serializable]
	public class SpawnInstruction
	{
		/// <summary>
		/// スポーンするエージェント - つまり、ウェーブのモンスター
		/// </summary>
		public AgentConfiguration agentConfiguration;

		/// <summary>
		/// 前回の生成からこのエージェントが生成されるまでの遅延
		/// </summary>
		[Tooltip("The delay from the previous spawn until when this agent is spawned")]
		public float delayToSpawn;

		/// <summary>
		/// エージェントが生成される開始ノード
		/// </summary>
		public Node startingNode;
	}
}