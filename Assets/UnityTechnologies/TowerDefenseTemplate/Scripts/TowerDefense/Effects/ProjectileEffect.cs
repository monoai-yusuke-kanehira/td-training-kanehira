using ActionGameFramework.Projectiles;
using Core.Utilities;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Effects
{
	/// <summary>
	/// この発射体の生成と効果の管理のためのクラス。持続する必要がある効果に使用されます
	/// 発射体が破壊/再プールされた後、少し時間がかかります。イネーブル時にエフェクトを作成し、次の場所に移動します 
	/// 有効な間は毎フレームこのオブジェクトを追従させます。
	/// 
	/// 無効にすると、効果の破壊をトリガーするための SelfDestroyTimer を見つけようとします。それ以外の場合は、
	/// すぐに再プールします
	/// </summary>
	[RequireComponent(typeof(IProjectile))]
	public class ProjectileEffect : MonoBehaviour
	{
		/// <summary>
		/// この発射体が発射されたときに生成される Preafb
		/// </summary>
		public GameObject effectPrefab;

		/// <summary>
		/// エフェクトを次のように変換します
		/// </summary>
		public Transform followTransform;

		/// <summary>
		/// 生成済みエフェクトのキャッシュです
		/// </summary>
		GameObject m_SpawnedEffect;
		
		/// <summary>
		/// スポーンされたオブジェクトのキャッシュされた破壊タイマー
		/// </summary>
		SelfDestroyTimer m_DestroyTimer;

		/// <summary>
		/// スポーンされたオブジェクトに対するキャッシュされたプール可能なエフェクト
		/// </summary>
		PoolableEffect m_Resetter;

		/// <summary>
		/// Projectileのキャッシュです
		/// </summary>
		IProjectile m_Projectile;

		/// <summary>
		/// 発射物発射イベントを登録する
		/// </summary>
		protected virtual void Awake()
		{
			m_Projectile = GetComponent<IProjectile>();
			m_Projectile.fired += OnFired;
			if (followTransform != null)
			{
				followTransform = transform;
			}
		}

		/// <summary>
		/// 代理人の登録を解除する
		/// </summary>
		protected virtual void OnDestroy()
		{
			m_Projectile.fired -= OnFired;
		}

		/// <summary>
		/// エフェクトを生成する
		/// </summary>
		protected virtual void OnFired()
		{
			if (effectPrefab != null)
			{
				m_SpawnedEffect = Poolable.TryGetPoolable(effectPrefab);
				m_SpawnedEffect.transform.parent = null;
				m_SpawnedEffect.transform.position = followTransform.position;
				m_SpawnedEffect.transform.rotation = followTransform.rotation;
				
				// このオブジェクトが破壊されないように、タイマーが最初にオンになっている場合は必ず無効にしてください
				m_DestroyTimer = m_SpawnedEffect.GetComponent<SelfDestroyTimer>();
				if (m_DestroyTimer != null)
				{
					m_DestroyTimer.enabled = false;
				}
				m_Resetter = m_SpawnedEffect.GetComponent<PoolableEffect>();
				if (m_Resetter != null)
				{
					m_Resetter.TurnOnAllSystems();
				}
			}
		}

		/// <summary>
		/// エフェクトをこのオブジェクトに追従させます
		/// </summary>
		protected virtual void Update()
		{
			// 効果が私たちの立場に従うようにします
			// 親化したときに無効にしてはいけないため、親化はしません
			if (m_SpawnedEffect != null)
			{
				m_SpawnedEffect.transform.position = followTransform.position;
			}
		}

		/// <summary>
		///破壊して効果破壊開始
		/// </summary>
		protected virtual void OnDisable()
		{
			if (m_SpawnedEffect == null)
			{
				return;
			}
			
			// 破壊タイマーを開始する
			if (m_DestroyTimer != null)
			{
				m_DestroyTimer.enabled = true;

				if (m_Resetter != null)
				{
					m_Resetter.StopAll();
				}
			}
			else
			{
				// すぐに再プールします
				Poolable.TryPool(m_SpawnedEffect);
			}

			m_SpawnedEffect = null;
			m_DestroyTimer = null;
		}
	}
}
