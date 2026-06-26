using System;
using ActionGameFramework.Health;
using Core.Utilities;
using TowerDefense.Affectors;
using TowerDefense.Level;
using TowerDefense.Nodes;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefense.Agents
{
	/// <summary>
	/// エージェントはノードのパスをたどります
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(AttackAffector))]
	public abstract class Agent : Targetable
	{	
		/// <summary>
		/// エージェントの経路に沿ってエージェントを追跡する手段
		/// </summary>
		public enum State
		{
			/// <summary>
			/// エージェントが遮断されていないパス上にある場合
			/// </summary>
			OnCompletePath,

			/// <summary>
			/// エージェントがパス上にある場合はブロックされます
			/// </summary>
			OnPartialPath,

			/// <summary>
			/// エージェントがブロックされたパスの終点に到達したとき
			/// </summary>
			Attacking,

			/// <summary>
			/// 飛行エージェントの場合、障害物を越えて移動するとき
			/// </summary>
			PushingThrough,
			
			/// <summary>
			/// エージェントがパスを完了したとき
			/// </summary>
			PathComplete
		}

		/// <summary>
		/// エージェントが最終ノードに到達したときに発生するイベント
		/// </summary>
		public event Action<Node> destinationReached;

		/// <summary>
		/// 適用されたエフェクトの位置オフセット
		/// </summary>
		public Vector3 appliedEffectOffset = Vector3.zero;
		
		/// <summary>
		/// 適用されたエフェクトのスケール調整
		/// </summary>
		public float appliedEffectScale = 1;

		/// <summary>
		/// これにアタッチされた NavMeshAgent コンポーネント
		/// </summary>
		protected NavMeshAgent m_NavMeshAgent;

		/// <summary>
		/// エージェントが移動する必要がある現在のノード
		/// </summary>
		protected Node m_CurrentNode;

		/// <summary>
		/// レベルマネージャーへの参照
		/// </summary>
		protected LevelManager m_LevelManager;

		/// <summary>
		/// 目的地を次のノードに保存するため、毎回新しいランダムな位置を取得する必要がありません
		/// </summary>
		protected Vector3 m_Destination;
		
		/// <summary>
		/// アタッチされたナビゲーション メッシュ エージェントの速度を取得します
		/// </summary>
		public override Vector3 velocity
		{
			get { return m_NavMeshAgent.velocity; }
		}
		
		/// <summary>
		/// パスに沿ったエージェントの現在の状態
		/// </summary>
		public State state { get; protected set; }

		/// <summary>
		/// へのアクセサ <see cref="m_NavMeshAgent"/>
		/// </summary>
		public NavMeshAgent navMeshNavMeshAgent
		{
			get { return m_NavMeshAgent; }
			set { m_NavMeshAgent = value; }
		}

		/// <summary>
		/// アタッチされたナビゲーション メッシュ エージェントのエリア マスク
		/// </summary>
		public int navMeshMask
		{
			get { return m_NavMeshAgent.areaMask; }
		}

		/// <summary>
		/// このエージェントの本来の移動速度を取得します
		/// </summary>
		public float originalMovementSpeed { get; private set; }

		/// <summary>
		/// パスがブロックされているかどうかを確認します
		/// </summary>
		/// <value>
		/// エージェントのパスのステータス
		/// </value>
		protected virtual bool isPathBlocked
		{
			get { return m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathPartial; }
		}

		/// <summary>
		/// エージェントは目的地に十分近いですか?
		/// </summary>
		protected virtual bool isAtDestination
		{
			get { return navMeshNavMeshAgent.remainingDistance <= navMeshNavMeshAgent.stoppingDistance; }
		}

		/// <summary>
		/// 移動先のノードを設定します
		/// </summary>
		/// <param name="node">エージェントが移動するノード</param>
		public virtual void SetNode(Node node)
		{
			m_CurrentNode = node;
		}

		/// <summary>
		/// navMeshAgent を停止し、プールに戻ろうとします
		/// </summary>
		public override void Remove()
		{
			base.Remove();
			
			m_LevelManager.DecrementNumberOfEnemies();
			if (m_NavMeshAgent.enabled)
			{
				m_NavMeshAgent.isStopped = true;
			}
			m_NavMeshAgent.enabled = false;

			Poolable.TryPool(gameObject);
		}

		/// <summary>	
		/// 構成データからこのエージェントに必要なすべてのパラメータをセットアップします
		/// </summary>
		public virtual void Initialize()
		{
			ResetPositionData();
			LazyLoad();
			configuration.SetHealth(configuration.maxHealth);
			state = isPathBlocked ? State.OnPartialPath : State.OnCompletePath;

			m_NavMeshAgent.enabled = true;
			m_NavMeshAgent.isStopped = false;
			
			m_LevelManager.IncrementNumberOfEnemies();
		}

		/// <summary>
		/// パス内の次のノードを検索します
		/// </summary>
		public virtual void GetNextNode(Node currentlyEnteredNode)
		{
			// 呼び出し元のノードが m_CurrentNode と同じ場合は何もしません
			if (m_CurrentNode != currentlyEnteredNode)
			{
				return;
			}
			if (m_CurrentNode == null)
			{
				Debug.LogError("Cannot find current node");
				return;
			}

			Node nextNode = m_CurrentNode.GetNextNode();
			if (nextNode == null)
			{
				if (m_NavMeshAgent.enabled)
				{
					m_NavMeshAgent.isStopped = true;
				}
				HandleDestinationReached();
				return;
			}
			
			Debug.Assert(nextNode != m_CurrentNode);
			SetNode(nextNode);
			MoveToNode();
		}

		/// <summary>
		/// エージェントを次の位置に移動します <see cref="Agent.m_CurrentNode" />
		/// </summary>
		public virtual void MoveToNode()
		{
			Vector3 nodePosition = m_CurrentNode.GetRandomPointInNodeArea();
			nodePosition.y = m_CurrentNode.transform.position.y;
			m_Destination = nodePosition;
			NavigateTo(m_Destination);
		}

		/// <summary>
		/// 目的地に到着したときに何が起こるかのロジック
		/// </summary>
		public virtual void HandleDestinationReached()
		{
			state = State.PathComplete;
			if (destinationReached != null)
			{
				destinationReached(m_CurrentNode);
			} 
		}
		
		/// <summary>
		/// 必要に応じて Lazy Load し、NavMeshAgent が無効になっていることを確認します
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			LazyLoad();
			m_NavMeshAgent.enabled = false;
		}
		
		/// <summary>
		/// エージェントをさまざまな状態で更新します 
		/// パスが古い場合に宛先をリセット
		/// </summary>
		protected virtual void Update()
		{
			// さまざまな状態の動作を更新する
			PathUpdate();
			
			// パスが無効になった場合は、エージェントのパスを宛先に再設定します
			bool validStalePath = m_NavMeshAgent.isOnNavMesh && m_NavMeshAgent.enabled &&
			                      (!m_NavMeshAgent.hasPath && !m_NavMeshAgent.pathPending);
			if (validStalePath)
			{
				// エージェントの停止距離の 2 乗と比較します
				// 実行中に動的に変更できるよう、この値はあえて事前に二乗しません
				float squareStoppingDistance = m_NavMeshAgent.stoppingDistance * m_NavMeshAgent.stoppingDistance;
				if (Vector3.SqrMagnitude(m_Destination - transform.position) < squareStoppingDistance &&
				    m_CurrentNode.GetNextNode() != null)
				{
					// 目的地に着いたら先に進みます
					GetNextNode(m_CurrentNode);
				}
				else
				{
					// そうでなければ経路の再計算を試します
					m_NavMeshAgent.SetDestination(m_Destination);
				}
			}
		}

		/// <summary>
		/// NavMeshAgent の宛先を設定します
		/// </summary>
		/// <param name="nextPoint">移動先の位置</param>
		protected virtual void NavigateTo(Vector3 nextPoint)
		{
			LazyLoad();
			if (m_NavMeshAgent.isOnNavMesh)
			{
				m_NavMeshAgent.SetDestination(nextPoint);
			}
		}

		/// <summary>
		/// これは、エージェントによって使用されるいくつかのコンポーネントをキャッシュする遅延的な方法です
		/// </summary>
		protected virtual void LazyLoad()
		{
			if (m_NavMeshAgent == null)
			{
				m_NavMeshAgent = GetComponent<NavMeshAgent>();
				originalMovementSpeed = m_NavMeshAgent.speed;
			}
			if (m_LevelManager == null)
			{
				m_LevelManager = LevelManager.instance;
			}
		}

		/// <summary>
		/// パスに沿って移動し、次のように変更します <see cref="Agent.State.OnPartialPath" />
		/// </summary>
		protected virtual void OnCompletePathUpdate()
		{
			if (isPathBlocked)
			{
				state = State.OnPartialPath;
			}
		}

		/// <summary>
		/// 関連するパスの更新を実行します
		/// </summary>
		protected abstract void PathUpdate();

		/// <summary>
		/// エージェントがブロックされた場合の動作
		/// </summary>
		protected abstract void OnPartialPathUpdate();
		
		
#if UNITY_EDITOR
		/// <summary>
		/// エージェントのパスを描画します
		/// </summary>
		protected virtual void OnDrawGizmosSelected()
		{
			if (m_NavMeshAgent != null)
			{
				Vector3[] pathPoints = m_NavMeshAgent.path.corners;
				int count = pathPoints.Length;
				for (int i = 0; i < count - 1; i++)
				{
					Vector3 from = pathPoints[i];
					Vector3 to = pathPoints[i + 1];
					Gizmos.DrawLine(from, to);
				}
				Gizmos.DrawWireSphere(m_NavMeshAgent.destination, 0.2f);
			}
		}
#endif
	}
}
