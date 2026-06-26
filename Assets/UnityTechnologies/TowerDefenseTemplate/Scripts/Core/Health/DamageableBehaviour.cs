using System;
using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// ダメージを受けられるMonoBehaviour用の抽象クラス
	/// </summary>
	public class DamageableBehaviour : MonoBehaviour
	{
		/// <summary>
		/// Damageableオブジェクト
		/// </summary>
		public Damageable configuration;

		/// <summary>
		/// この <see cref="DamageableBehaviour" /> が死亡しているかどうかを取得します。
		/// </summary>
		/// <value>死亡している場合は true</value>
		public bool isDead
		{
			get { return configuration.isDead; }
		}

		/// <summary>
		/// Transformの位置
		/// </summary>
		public virtual Vector3 position
		{
			get { return transform.position; }
		}

		/// <summary>
		/// ダメージを受けたときに発生します
		/// </summary>
		public event Action<HitInfo> hit;
		
		/// <summary>
		/// このインスタンスがプールへ戻されたり破棄されたりして削除されたときに発生するイベント
		/// </summary>
		public event Action<DamageableBehaviour> removed;
		
		/// <summary>
		/// このインスタンスが倒されたときに発生するイベント
		/// </summary>
		public event Action<DamageableBehaviour> died;
		

		/// <summary>
		/// ダメージを受け、ダメージが発生した位置も受け取ります
		/// </summary>
		/// <param name="damageValue">ダメージ量</param>
		/// <param name="damagePoint">ダメージ位置。</param>
		/// <param name="alignment">所属情報</param>
		public virtual void TakeDamage(float damageValue, Vector3 damagePoint, IAlignmentProvider alignment)
		{
			HealthChangeInfo info;
			configuration.TakeDamage(damageValue, alignment, out info);
			var damageInfo = new HitInfo(info, damagePoint);
			if (hit != null)
			{
				hit(damageInfo);
			}
		}

		protected virtual void Awake()
		{
			configuration.Init();
			configuration.died += OnConfigurationDied;
		}

		/// <summary>
		/// このDamageableを倒します
		/// </summary>
		protected virtual void Kill()
		{
			HealthChangeInfo healthChangeInfo;
			configuration.TakeDamage(configuration.currentHealth, null, out healthChangeInfo);
		}


		/// <summary>
		/// 死亡扱いにせず、このDamageableを削除します
		/// </summary>
		public virtual void Remove()
		{
			// このBehaviourが死亡状態に見えるよう体力を0にします。死亡イベントは発生しません
			configuration.SetHealth(0);
			OnRemoved();
		}

		/// <summary>
		/// killイベントを発火します
		/// </summary>
		void OnDeath()
		{
			if (died != null)
			{
				died(this);
			}
		}
		
		/// <summary>
		/// removedイベントを発火します
		/// </summary>
		void OnRemoved()
		{
			if (removed != null)
			{
				removed(this);
			}
		}
		
		/// <summary>
		/// Damageableが致命的なダメージを受けたときに発生するイベント
		/// </summary>
		void OnConfigurationDied(HealthChangeInfo changeInfo)
		{
			OnDeath();
			Remove();
		}
	}
}
