using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Health
{
	/// <summary>
	/// HealthChangeInfoを渡すUnityEvent
	/// </summary>
	[Serializable]
	public class HealthChangeEvent : UnityEvent<HealthChangeInfo>
	{
	}

	/// <summary>
	/// HitInfoを渡すUnityEvent
	/// </summary>
	[Serializable]
	public class HitEvent : UnityEvent<HitInfo>
	{
	}

	/// <summary>
	/// Damageableのリスナー。
	/// </summary>
	public class DamageableListener : MonoBehaviour
	{
		// 監視するDamageableBehaviour
		[Tooltip("Leave this empty if the DamageableBehaviour and DamageableListener are on the same component")]
		public DamageableBehaviour damageableBehaviour;

		// 体力変化（回復/ダメージ）用のイベント。Editorで設定します
		public HealthChangeEvent damaged;
		
		public HealthChangeEvent healed;

		// 死亡と最大体力用のイベント。Editorで設定します
		public UnityEvent died;

		public UnityEvent reachedMaxHealth;

		// 体力が変化したときのイベント
		public HealthChangeEvent healthChanged;
		
		// ヒット用のイベント
		[Header("The hit event is different from the damage event as it also contains hit position data")]
		public HitEvent hit;

		/// <summary>
		/// DamageableBehaviourを遅延読み込みします
		/// </summary>
		protected virtual void Awake()
		{
			LazyLoad();
		}

		/// <summary>
		/// イベントを購読します
		/// </summary>
		protected virtual void OnEnable()
		{
			damageableBehaviour.configuration.died += OnDeath;
			damageableBehaviour.configuration.reachedMaxHealth += OnReachedMaxHealth;
			damageableBehaviour.configuration.healed += OnHealed;
			damageableBehaviour.configuration.damaged += OnDamaged;
			damageableBehaviour.configuration.healthChanged += OnHealthChanged;
			damageableBehaviour.hit += OnHit;
		}

		/// <summary>
		/// 無効化時にイベントの購読を解除します
		/// </summary>
		protected virtual void OnDisable()
		{
			damageableBehaviour.configuration.died -= OnDeath;
			damageableBehaviour.configuration.reachedMaxHealth -= OnReachedMaxHealth;
			damageableBehaviour.configuration.healed -= OnHealed;
			damageableBehaviour.configuration.damaged -= OnDamaged;
			damageableBehaviour.configuration.healthChanged -= OnHealthChanged;
			damageableBehaviour.hit -= OnHit;
		}

		/// <summary>
		/// 死亡のUnityEventを発火します。
		/// </summary>
		protected virtual void OnDeath(HealthChangeInfo info)
		{
			died.Invoke();
		}

		/// <summary>
		/// 最大体力のUnityEventを発火します。
		/// </summary>
		protected virtual void OnReachedMaxHealth()
		{
			reachedMaxHealth.Invoke();
		}

		/// <summary>
		/// 回復のUnityEventを発火します。
		/// </summary>
		/// <param name="info">Info.</param>
		protected virtual void OnHealed(HealthChangeInfo info)
		{
			healed.Invoke(info);
		}

		/// <summary>
		/// ダメージのUnityEventを発火します。
		/// </summary>
		/// <param name="info">Info.</param>
		protected virtual void OnDamaged(HealthChangeInfo info)
		{
			damaged.Invoke(info);
		}
		
		/// <summary>
		/// healthChangedのUnityEventを発火します。
		/// </summary>
		/// <param name="info">Info.</param>
		protected virtual void OnHealthChanged(HealthChangeInfo info)
		{
			healthChanged.Invoke(info);
		}

		/// <summary>
		/// ヒットのUnityEventを発火します。
		/// </summary>
		/// <param name="info">Info.</param>
		protected virtual void OnHit(HitInfo info)
		{
			hit.Invoke(info);
		}

		/// <summary>
		/// damageableBehaviourがまだ割り当てられていない場合に探します
		/// Editorで割り当てられている場合や、以前のLazyLoad()呼び出しで設定されている場合があります
		/// </summary>
		protected void LazyLoad()
		{
			if (damageableBehaviour != null)
			{
				return;
			}

			damageableBehaviour = GetComponent<DamageableBehaviour>();
		}
	}
}