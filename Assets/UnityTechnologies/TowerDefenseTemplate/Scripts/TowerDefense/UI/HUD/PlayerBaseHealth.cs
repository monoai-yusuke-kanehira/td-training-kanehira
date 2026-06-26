using System.Globalization;
using Core.Health;
using TowerDefense.Level;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
	/// <summary>
	/// プレイヤー拠点の体力を表示するUIのシンプルな実装
	/// </summary>
	public class PlayerBaseHealth : MonoBehaviour
	{
		/// <summary>
		/// 情報を表示するText要素
		/// </summary>
		public Text display;

		/// <summary>
		/// 拠点が取り得る最大体力
		/// </summary>
		protected float m_MaxHealth;

		/// <summary>
		/// プレイヤー拠点の最大体力を取得する
		/// </summary>
		protected virtual void Start()
		{
			LevelManager levelManager = LevelManager.instance;
			if (levelManager == null)
			{
				return;
			}
			if (levelManager.numberOfHomeBases > 0)
			{
				Damageable baseConfig = levelManager.playerHomeBases[0].configuration;
				baseConfig.damaged += OnBaseDamaged;
				float currentHealth = baseConfig.currentHealth;
				float noramlisedHealth = baseConfig.normalisedHealth;
				m_MaxHealth = currentHealth / noramlisedHealth;
			}
			UpdateDisplay();
		}

		/// <summary>
		/// プレイヤー拠点の体力変更イベントを購読する
		/// </summary>
		/// <param name="info">
		/// 関連する体力変更情報
		/// </param>
		protected virtual void OnBaseDamaged(HealthChangeInfo info)
		{
			UpdateDisplay();
		}

		/// <summary>
		/// Home Baseの現在の体力を取得し、m_Displayに表示する
		/// </summary>
		protected void UpdateDisplay()
		{
			LevelManager levelManager = LevelManager.instance;
			if (levelManager == null)
			{
				return;
			}
			float currentHealth = levelManager.GetAllHomeBasesHealth();
			display.text = currentHealth.ToString(CultureInfo.InvariantCulture);
		}
	}
}
