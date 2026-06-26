using System;
using System.Collections.Generic;
using Core.Extensions;
using Core.Utilities;
using TowerDefense.Agents;
using TowerDefense.Agents.Data;
using TowerDefense.Nodes;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// Wave は TimedBehaviour であり、RepeatingTimer を使用して敵を生成します
	/// </summary>
	public class Wave : TimedBehaviour
	{
		/// <summary>
		/// 敵の出現方法のリスト
		/// </summary>
		public List<SpawnInstruction> spawnInstructions;

		/// <summary>
		/// スポーンする現在の敵のインデックス
		/// </summary>
		protected int m_CurrentIndex;

		/// <summary>
		/// 敵をスポーンするために使用されるRepeatingTimer
		/// </summary>
		protected RepeatingTimer m_SpawnTimer;

		/// <summary>
		/// Wave が完了したときに発生するイベント
		/// </summary>
		public event Action waveCompleted;

		public virtual float progress
		{
			get { return (float) (m_CurrentIndex) / spawnInstructions.Count; }
		}

		/// <summary>
		/// ウェーブを初期化します
		/// </summary>
		public virtual void Init()
		{
			// Wave が空の場合は、レベル デザイナーに警告し、Complete イベントを発生させます
			if (spawnInstructions.Count == 0)
			{
				Debug.LogWarning("[LEVEL] Empty Wave");
				SafelyBroadcastWaveCompletedEvent();
				return;
			}

			m_SpawnTimer = new RepeatingTimer(spawnInstructions[0].delayToSpawn, SpawnCurrent);
			StartTimer(m_SpawnTimer);
		}

		/// <summary>
		/// 現在のエージェントの生成を処理し、次のエージェントの生成をセットアップします
		/// </summary>
		protected virtual void SpawnCurrent()
		{
			Spawn();
			if (!TrySetupNextSpawn())
			{
				SafelyBroadcastWaveCompletedEvent();
				// これは波の進行状況が正確であるために必要です
				m_CurrentIndex = spawnInstructions.Count;
				StopTimer(m_SpawnTimer);
			}
		}

		/// <summary>
		/// 現在のエージェントを生成します
		/// </summary>
		protected void Spawn()
		{
			SpawnInstruction spawnInstruction = spawnInstructions[m_CurrentIndex];
			SpawnAgent(spawnInstruction.agentConfiguration, spawnInstruction.startingNode);
		}

		/// <summary>
		/// 次のスポーンをセットアップしようとします
		/// </summary>
		/// <returns>別のスポーン命令がある場合は true、そうでない場合は false</returns>
		protected bool TrySetupNextSpawn()
		{
			bool hasNext = spawnInstructions.Next(ref m_CurrentIndex);
			if (hasNext)
			{
				SpawnInstruction nextSpawnInstruction = spawnInstructions[m_CurrentIndex];
				if (nextSpawnInstruction.delayToSpawn <= 0f)
				{
					SpawnCurrent();
				}
				else
				{
					m_SpawnTimer.SetTime(nextSpawnInstruction.delayToSpawn);
				}
			}

			return hasNext;
		}

		/// <summary>
		/// エージェントを生成します
		/// </summary>
		/// <param name="agentConfig">生成するエージェント</param>
		/// <param name="node">エージェントが使用する開始ノード</param>
		protected virtual void SpawnAgent(AgentConfiguration agentConfig, Node node)
		{
			Vector3 spawnPosition = node.GetRandomPointInNodeArea();

			var poolable = Poolable.TryGetPoolable<Poolable>(agentConfig.agentPrefab.gameObject);
			if (poolable == null)
			{
				return;
			}
			var agentInstance = poolable.GetComponent<Agent>();
			agentInstance.transform.position = spawnPosition;
			agentInstance.Initialize();
			agentInstance.SetNode(node);
			agentInstance.transform.rotation = node.transform.rotation;
		}

		/// <summary>
		/// waveCompleted イベントを開始する
		/// </summary>
		protected void SafelyBroadcastWaveCompletedEvent()
		{
			if (waveCompleted != null)
			{
				waveCompleted();
			}
		}
	}
}