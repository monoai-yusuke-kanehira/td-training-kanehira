using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// Damageableの体力を表示するクラス
	/// </summary>
	public class HealthVisualizer : MonoBehaviour
	{
		/// <summary>
		/// Damageableの割り当てに使うDamageableBehaviour
		/// </summary>
		[Tooltip("This field does not need to be populated here, it can be set up in code using AssignDamageable")]
		public DamageableBehaviour damageableBehaviour;
		
		/// <summary>
		/// ヘルスバーを減らすためにXスケールを変更するオブジェクト。初期状態では均一なスケールにしてください
		/// </summary>
		public Transform healthBar;
		
		/// <summary>
		/// ヘルスバー背景を増やすためにXスケールを変更するオブジェクト。初期状態では均一なスケールにしてください
		/// </summary>
		public Transform backgroundBar;

		/// <summary>
		/// 体力が満タンでもこのヘルスバーを表示するかどうか
		/// </summary>
		public bool showWhenFull;

		/// <summary>
		/// 表示を向ける対象のカメラ
		/// </summary>
		protected Transform m_CameraToFace;

		/// <summary>
		/// 体力を表示する対象のDamageable
		/// </summary>
		protected Damageable m_Damageable;

		/// <summary>
		/// 上dates the visualization of the health
		/// </summary>
		/// <param name="normalizedHealth">Normalized health value</param>
		public void UpdateHealth(float normalizedHealth)
		{
			Vector3 scale = Vector3.one;

			if (healthBar != null)
			{
				scale.x = normalizedHealth;
				healthBar.transform.localScale = scale;
			}

			if (backgroundBar != null)
			{
				scale.x = 1 - normalizedHealth;
				backgroundBar.transform.localScale = scale;
			}

			SetVisible(showWhenFull || normalizedHealth < 1.0f);
		}

		/// <summary>
		/// この表示オブジェクトの表示状態を設定します
		/// </summary>
		public void SetVisible(bool visible)
		{
			gameObject.SetActive(visible);
		}

		/// <summary>
		/// damageableを割り当て、ダメージイベントを購読します
		/// </summary>
		/// <param name="damageable">Damageable to assign</param>
		public void AssignDamageable(Damageable damageable)
		{
			if (m_Damageable != null)
			{
				m_Damageable.healthChanged -= OnHealthChanged;
			}
			m_Damageable = damageable;
			m_Damageable.healthChanged += OnHealthChanged;
		}

		/// <summary>
		/// カメラの方を向かせます
		/// </summary>
		protected virtual void Update()
		{
			Vector3 direction = m_CameraToFace.transform.forward;
			transform.forward = -direction;
		}

		/// <summary>
		/// damageableBehaviourが設定されている場合、damageableを割り当てます
		/// </summary>
		protected virtual void Awake()
		{
			if (damageableBehaviour != null)
			{
				AssignDamageable(damageableBehaviour.configuration);
			}
		}

		/// <summary>
		/// メインカメラをキャッシュします
		/// </summary>
		protected virtual void Start()
		{
			m_CameraToFace = UnityEngine.Camera.main.transform;
		}

		void OnHealthChanged(HealthChangeInfo healthChangeInfo)
		{
			UpdateHealth(m_Damageable.normalisedHealth);
		}
	}
}