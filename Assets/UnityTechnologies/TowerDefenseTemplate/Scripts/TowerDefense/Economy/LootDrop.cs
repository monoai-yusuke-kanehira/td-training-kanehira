using Core.Health;
using TowerDefense.Level;
using UnityEngine;

namespace TowerDefense.Economy
{
	/// <summary>
	/// アタッチされたDamageableBehaviourが終了したときに通貨にお金を追加するクラス
	/// </summary>
	[RequireComponent(typeof(DamageableBehaviour))]
	public class LootDrop : MonoBehaviour
	{
		/// <summary>
		/// オブジェクトが「死亡」したときにドロップされる戦利品/通貨の量
		/// </summary>
		public int lootDropped = 1;

		/// <summary>
		/// 添付のDamgableBehaviour
		/// </summary>
		protected DamageableBehaviour m_DamageableBehaviour;

		/// <summary>
		/// キャッシュがアタッチされています
		/// </summary>
		protected virtual void OnEnable()
		{
			if (m_DamageableBehaviour == null)
			{
				m_DamageableBehaviour = GetComponent<DamageableBehaviour>();
			}
			m_DamageableBehaviour.configuration.died += OnDeath;
		}

		/// <summary>
		/// からの購読を解除しました <see cref="m_DamageableBehaviour"/> 死亡イベント
		/// </summary>
		protected virtual void OnDisable()
		{
			m_DamageableBehaviour.configuration.died -= OnDeath;
		}

		/// <summary>
		/// アタッチされたオブジェクトが「死亡」したときのコールバック
		/// 追加 <see cref="lootDropped"/> 現在の通貨に換算
		/// </summary>
		protected virtual void OnDeath(HealthChangeInfo info)
		{
			m_DamageableBehaviour.configuration.died -= OnDeath;

			if (info.damageAlignment == null ||
				!info.damageAlignment.CanHarm(m_DamageableBehaviour.configuration.alignmentProvider))
			{
				return;
			}
			
			LevelManager levelManager = LevelManager.instance;
			if (levelManager == null)
			{
				return;
			}
			levelManager.currency.AddCurrency(lootDropped);
		}
	}
}