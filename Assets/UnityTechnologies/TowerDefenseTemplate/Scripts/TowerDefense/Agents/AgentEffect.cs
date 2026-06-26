using UnityEngine;

namespace TowerDefense.Agents
{
	/// <summary>
	/// エージェントにさまざまな効果を与えるコンポーネント
	/// </summary>
	public abstract class AgentEffect : MonoBehaviour
	{
		/// <summary>
		/// 影響を受けるエージェントへの参照
		/// </summary>
		protected Agent m_Agent;

		public virtual void Awake()
		{
			LazyLoad();
		}

		/// <summary>
		/// それを確実にするための怠惰な方法 <see cref="m_Agent"/> nullにはなりません
		/// </summary>
		public virtual void LazyLoad()
		{
			if (m_Agent == null)
			{
				m_Agent = GetComponent<Agent>();
			}
		}
	}
}