using System;
using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// イベントを使って体力を扱うDamageableクラス
	/// プレイヤー、敵、破壊可能なワールドオブジェクトにも使用できます
	/// </summary>
	[Serializable]
	public class Damageable
	{
		/// <summary>
		/// このインスタンスの最大体力
		/// </summary>
		public float maxHealth;
		
		public float startingHealth;

		/// <summary>
		/// ダメージを与えた側の所属
		/// </summary>
		public SerializableIAlignmentProvider alignment;

		/// <summary>
		/// 現在の体力を取得します。
		/// </summary>
		public float currentHealth { protected set; get; }

		/// <summary>
		/// 正規化された体力を取得します。
		/// </summary>
		public float normalisedHealth
		{
			get
			{
				if (Math.Abs(maxHealth) <= Mathf.Epsilon)
				{
					Debug.LogError("Max Health is 0");
					maxHealth = 1f;
				}
				return currentHealth / maxHealth;
			}
		}

		/// <summary>
		/// このインスタンスの <see cref="IAlignmentProvider"/> を取得します
		/// </summary>
		public IAlignmentProvider alignmentProvider
		{
			get
			{
				return alignment != null ? alignment.GetInterface() : null;
			}
		}

		/// <summary>
		/// このインスタンスが死亡しているかどうかを取得します。
		/// </summary>
		public bool isDead
		{
			get { return currentHealth <= 0f; }
		}

		/// <summary>
		/// このインスタンスの体力が最大かどうかを取得します。
		/// </summary>
		public bool isAtMaxHealth
		{
			get { return Mathf.Approximately(currentHealth, maxHealth); }
		}

		// イベント
		public event Action reachedMaxHealth;

		public event Action<HealthChangeInfo> damaged, healed, died, healthChanged;

		/// <summary>
		/// このインスタンスを初期化します
		/// </summary>
		public virtual void Init()
		{
			currentHealth = startingHealth;
		}

		/// <summary>
		/// 最大体力と開始時の体力を同じ値に設定します
		/// </summary>
		public void SetMaxHealth(float health)
		{
			if (health <= 0)
			{
				return;
			}
			maxHealth = startingHealth = health;
		}

		/// <summary>
		/// 最大体力と開始時の体力を別々に設定します
		/// </summary>
		public void SetMaxHealth(float health, float startingHealth)
		{
			if (health <= 0)
			{
				return;
			}
			maxHealth = health;
			this.startingHealth = startingHealth;
		}

		/// <summary>
		/// このインスタンスの体力を直接設定します。
		/// </summary>
		/// <param name="health">
		/// <see cref="currentHealth"/> に設定する値
		/// </param>
		public void SetHealth(float health)
		{
			var info = new HealthChangeInfo
			{
				damageable = this,
				newHealth = health, 
				oldHealth = currentHealth
			};
			
			currentHealth = health;
			
			if (healthChanged != null)
			{
				healthChanged(info);
			}
		}

		/// <summary>
		/// 所属を使って、ダメージを受けることが有効な処理か確認します
		/// </summary>
		/// <param name="damage">
		/// 受けるダメージ量
		/// </param>
		/// <param name="damageAlignment">
		/// 相手側の所属
		/// </param>
		/// <param name="output">
		/// ダメージを受けた場合の出力データ
		/// </param>
		/// <returns>
		/// <value>このインスタンスがダメージを受けた場合は true</value>
		/// <value>このインスタンスがすでに死亡している、または所属の判定でダメージが許可されなかった場合は false</value>
		/// </returns>
		public bool TakeDamage(float damage, IAlignmentProvider damageAlignment, out HealthChangeInfo output)
		{
			output = new HealthChangeInfo
			{
				damageAlignment = damageAlignment, damageable = this,
				newHealth = currentHealth, oldHealth = currentHealth
			};
			
			bool canDamage = damageAlignment == null || alignmentProvider == null ||
			                 damageAlignment.CanHarm(alignmentProvider);
			
			if (isDead || !canDamage)
			{
				return false;
			}

			ChangeHealth(-damage, output);
			SafelyDoAction(damaged, output);
			if (isDead)
			{
				SafelyDoAction(died, output);
			}
			return true;
		}

		/// <summary>
		/// 体力を増やす処理。
		/// </summary>
		/// <param name="health">増やす体力量</param>
		public HealthChangeInfo IncreaseHealth(float health)
		{
			var info = new HealthChangeInfo {damageable = this};
			ChangeHealth(health, info);
			SafelyDoAction(healed, info);
			if (isAtMaxHealth)
			{
				SafelyDoAction(reachedMaxHealth);
			}

			return info;
		}

		/// <summary>
		/// 体力を変更します。
		/// </summary>
		/// <param name="healthIncrement">体力の増減量</param>
		/// <param name="info">この変更に使う HealthChangeInfo</param>
		protected void ChangeHealth(float healthIncrement, HealthChangeInfo info)
		{
			info.oldHealth = currentHealth;
			currentHealth += healthIncrement;
			currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
			info.newHealth = currentHealth;
			
			if (healthChanged != null)
			{
				healthChanged(info);
			}
		}

		/// <summary>
		/// アクションのnullチェック用ヘルパーメソッド
		/// </summary>
		/// <param name="action">実行する Action</param>
		protected void SafelyDoAction(Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		/// <summary>
		/// アクションのnullチェック用ヘルパーメソッド
		/// </summary>
		/// <param name="action">実行する Action</param>
		/// <param name="info">Action に渡す HealthChangeInfo</param>
		protected void SafelyDoAction(Action<HealthChangeInfo> action, HealthChangeInfo info)
		{
			if (action != null)
			{
				action(info);
			}
		}
	}
}
