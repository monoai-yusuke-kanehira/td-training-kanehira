using System;
using System.Collections.Generic;
using ActionGameFramework.Health;
using Core.Health;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense.Targetting
{
	/// <summary>
	/// Affector用のターゲットを追跡するクラス
	/// </summary>
	public class Targetter : MonoBehaviour
	{
		/// <summary>
		/// Targetableが対象Colliderに入ったときに発火します
		/// </summary>
		public event Action<Targetable> targetEntersRange;

		/// <summary>
		/// Targetableが対象Colliderから出たときに発火します
		/// </summary>
		public event Action<Targetable> targetExitsRange;

		/// <summary>
		/// 適切なターゲットが見つかったときに発火します
		/// </summary>
		public event Action<Targetable> acquiredTarget;

		/// <summary>
		/// 現在のターゲットを見失ったときに発火します
		/// </summary>
		public event Action lostTarget;

		/// <summary>
		/// ターゲットへ向けるTransform
		/// </summary>
		public Transform turret;

		/// <summary>
		/// タレットのX回転範囲
		/// </summary>
		public Vector2 turretXRotationRange = new Vector2(0, 359);

		/// <summary>
		/// m_Turretが自由に回転するか、Y軸だけで回転するか
		/// </summary>
		public bool onlyYTurretRotation;

		/// <summary>
		/// 1秒あたりの検索回数
		/// </summary>
		public float searchRate;

		/// <summary>
		/// タレット待機中のY回転速度（度/秒）
		/// </summary>
		public float idleRotationSpeed = 39f;

		/// <summary>
		/// Towerが待機中にX回転を補正するまでの時間（秒）
		/// </summary>
		public float idleCorrectionTime = 2.0f;

		/// <summary>
		/// TargetterにアタッチされたCollider
		/// </summary>
		public Collider attachedCollider;

		/// <summary>
		/// タレットが回転を始める前に待機状態で待つ時間（秒）
		/// </summary>
		public float idleWaitTime = 2.0f;

		/// <summary>
		/// Collider内にいる現在のTargetable
		/// </summary>
		protected List<Targetable> m_TargetsInRange = new List<Targetable>();

		/// <summary>
		/// 検索が可能になるまでの秒数
		/// </summary>
		protected float m_SearchTimer = 0.0f;

		/// <summary>
		/// Towerが回転を始めるまでの秒数
		/// </summary>
		protected float m_WaitTimer = 0.0f;

		/// <summary>
		/// 現在のTargetable
		/// </summary>
		protected Targetable m_CurrrentTargetable;

		/// <summary>
		/// X回転補正に使うカウンター
		/// </summary>
		protected float m_XRotationCorrectionTime;

		/// <summary>
		/// 前フレームにTargetableがいたかどうか
		/// </summary>
		protected bool m_HadTarget;

		/// <summary>
		/// このタレットの回転速度
		/// </summary>
		protected float m_CurrentRotationSpeed;

		/// <summary>
		/// Colliderがどちらの種類でも半径を返します
		/// SphereまたはCapsuleのどちらでも
		/// </summary>
		public float effectRadius
		{
			get
			{
				var sphere = attachedCollider as SphereCollider;
				if (sphere != null)
				{
					return sphere.radius;
				}
				var capsule = attachedCollider as CapsuleCollider;
				if (capsule != null)
				{
					return capsule.radius;
				}
				return 0;
			}
		}

		/// <summary>
		/// Affectorの属性
		/// </summary>
		public IAlignmentProvider alignment;

		/// <summary>
		/// 現在のターゲットを返します
		/// </summary>
		public Targetable GetTarget()
		{
			return m_CurrrentTargetable;
		}

		/// <summary>
		/// 現在のターゲットリストと全イベントをクリアします
		/// </summary>
		public void ResetTargetter()
		{
			m_TargetsInRange.Clear();
			m_CurrrentTargetable = null;

			targetEntersRange = null;
			targetExitsRange = null;
			acquiredTarget = null;
			lostTarget = null;

			// タレットの向きをリセットします
			if (turret != null)
			{
				turret.localRotation = Quaternion.identity;
			}
		}

		/// <summary>
		/// Collider内のすべてのターゲットを返します。このリストは処理に使っているため変更しないでください
		/// Targetterのリストです。変更するとTargetterが壊れる可能性があります
		/// </summary>
		public List<Targetable> GetAllTargets()
		{
			return m_TargetsInRange;
		}

		/// <summary>
		/// Targetableが有効なターゲットか確認します
		/// </summary>
		/// <param name="targetable"></param>
		/// <returns>Targetableが有効ならtrue、そうでなければfalse</returns>
		protected virtual bool IsTargetableValid(Targetable targetable)
		{
			if (targetable == null)
			{
				return false;
			}
			
			IAlignmentProvider targetAlignment = targetable.configuration.alignmentProvider;
			bool canDamage = alignment == null || targetAlignment == null ||
			                 alignment.CanHarm(targetAlignment);
			
			return canDamage;
		}

		/// <summary>
		/// Triggerから出たとき、有効なTargetableを追跡リストから削除します。
		/// </summary>
		/// <param name="other">衝突した相手のCollider</param>
		protected virtual void OnTriggerExit(Collider other)
		{
			var targetable = other.GetComponent<Targetable>();
			if (!IsTargetableValid(targetable))
			{
				return;
			}
			
			m_TargetsInRange.Remove(targetable);
			if (targetExitsRange != null)
			{
				targetExitsRange(targetable);
			}
			if (targetable == m_CurrrentTargetable)
			{
				OnTargetRemoved(targetable);
			}
			else
			{
				// 実際のターゲットでない場合だけ削除します。実際のターゲットの場合は上のOnTargetRemovedが処理します
				targetable.removed -= OnTargetRemoved;
			}
		}
 
		/// <summary>
		/// Triggerに入ったとき、有効なTargetableを追跡リストに追加します。
		/// </summary>
		/// <param name="other">衝突した相手のCollider</param>
		protected virtual void OnTriggerEnter(Collider other)
		{
			var targetable = other.GetComponent<Targetable>();
			if (!IsTargetableValid(targetable))
			{
				return;
			}
			targetable.removed += OnTargetRemoved;
			m_TargetsInRange.Add(targetable);
			if (targetEntersRange != null)
			{
				targetEntersRange(targetable);
			}
		}

		/// <summary>
		/// 現在追跡中のTargetableの中で最も近いものを返します
		/// </summary>
		/// <returns>最も近いTargetable。存在しない場合はnull</returns>
		protected virtual Targetable GetNearestTargetable()
		{
			int length = m_TargetsInRange.Count;

			if (length == 0)
			{
				return null;
			}

			Targetable nearest = null;
			float distance = float.MaxValue;
			for (int i = length - 1; i >= 0; i--)
			{
				Targetable targetable = m_TargetsInRange[i];
				if (targetable == null || targetable.isDead)
				{
					m_TargetsInRange.RemoveAt(i);
					continue;
				}
				float currentDistance = Vector3.Distance(transform.position, targetable.position);
				if (currentDistance < distance)
				{
					distance = currentDistance;
					nearest = targetable;
				}
			}

			return nearest;
		}

		/// <summary>
		/// 検索タイマーを開始します
		/// </summary>
		protected virtual void Start()
		{
			m_SearchTimer = searchRate;
			m_WaitTimer = idleWaitTime;
		}

		/// <summary>
		/// 破壊されたターゲットがあるか確認し、必要に応じて新しいTargetableを取得します
		/// </summary>
		protected virtual void Update()
		{
			m_SearchTimer -= Time.deltaTime;

			if (m_SearchTimer <= 0.0f && m_CurrrentTargetable == null && m_TargetsInRange.Count > 0)
			{
				m_CurrrentTargetable = GetNearestTargetable();
				if (m_CurrrentTargetable != null)
				{
					if (acquiredTarget != null)
					{
						acquiredTarget(m_CurrrentTargetable);
					}
					m_SearchTimer = searchRate;
				}
			}

			AimTurret();

			m_HadTarget = m_CurrrentTargetable != null;
		}

		/// <summary>
		/// Agentの死亡イベント、または現在のターゲットが範囲外に出たときに呼ばれます。
		/// lostTargetイベントを発火します。
		/// </summary>
		void OnTargetRemoved(DamageableBehaviour target)
		{
			target.removed -= OnTargetRemoved;
			if (m_CurrrentTargetable != null && target.configuration == m_CurrrentTargetable.configuration)
			{
				if (lostTarget != null)
				{
					lostTarget();
				}
				m_HadTarget = false;
				m_TargetsInRange.Remove(m_CurrrentTargetable);
				m_CurrrentTargetable = null;
				m_XRotationCorrectionTime = 0.0f;
			}
			else //現在のターゲットではないため、ターゲットリストから探して削除します
			{
				for (int i = 0; i < m_TargetsInRange.Count; i++)
				{
					if (m_TargetsInRange[i].configuration == target.configuration)
					{
						m_TargetsInRange.RemoveAt(i);
						break;
					}
				}
			}
		}

		/// <summary>
		/// タレットを現在のターゲットへ向けます
		/// </summary>
		protected virtual void AimTurret()
		{
			if (turret == null)
			{
				return;
			}

			if (m_CurrrentTargetable == null) // 待機中の回転を行います
			{
				if (m_WaitTimer > 0)
				{
					m_WaitTimer -= Time.deltaTime;
					if (m_WaitTimer <= 0)
					{
						m_CurrentRotationSpeed = (Random.value * 2 - 1) * idleRotationSpeed;
					}
				}
				else
				{
					Vector3 euler = turret.rotation.eulerAngles;
					euler.x = Mathf.Lerp(Wrap180(euler.x), 0, m_XRotationCorrectionTime);
					m_XRotationCorrectionTime = Mathf.Clamp01((m_XRotationCorrectionTime + Time.deltaTime) / idleCorrectionTime);
					euler.y += m_CurrentRotationSpeed * Time.deltaTime;

					turret.eulerAngles = euler;
				}
			}
			else
			{
				m_WaitTimer = idleWaitTime;

				Vector3 targetPosition = m_CurrrentTargetable.position;
				if (onlyYTurretRotation)
				{
					targetPosition.y = turret.position.y;
				}
				Vector3 direction = targetPosition - turret.position;
				Quaternion look = Quaternion.LookRotation(direction, Vector3.up);
				Vector3 lookEuler = look.eulerAngles;
				// 最小値と最大値で角度を制限できるように、回転を-180から180の範囲に変換します
				float x = Wrap180(lookEuler.x);
				lookEuler.x = Mathf.Clamp(x, turretXRotationRange.x, turretXRotationRange.y);
				look.eulerAngles = lookEuler;
				turret.rotation = look;
			}
		}

		/// <summary>
		/// 角度を-180から180の範囲に変換するシンプルな関数
		/// </summary>
		static float Wrap180(float angle)
		{
			angle %= 360;
			if (angle < -180)
			{
				angle += 360;
			}
			else if (angle > 180)
			{
				angle -= 360;
			}
			return angle;
		}
	}
}