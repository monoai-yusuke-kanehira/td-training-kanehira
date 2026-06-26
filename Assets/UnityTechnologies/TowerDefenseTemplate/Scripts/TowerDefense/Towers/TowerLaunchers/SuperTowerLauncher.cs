using System.Collections.Generic;
using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using ActionGameFramework.Projectiles;
using Core.Utilities;
using TowerDefense.Affectors;
using TowerDefense.Level;
using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense.Towers.TowerLaunchers
{
	/// <summary>
	/// ホーミングProjectileを連射するLauncher
	/// </summary>
	public class SuperTowerLauncher : HomingLauncher
	{
		/// <summary>
		/// Towerが有効でいる時間
		/// </summary>
		public float towerLifeSpan = 10;

		/// <summary>
		/// 発射ベクトルをX軸方向に回転させる角度（度）
		/// </summary>
		public float fireVectorXRotationAdjustment = 45.0f;

		/// <summary>
		/// Projectileの最大数に達したときに発火します
		/// </summary>
		public UnityEvent death;
		
		/// <summary>
		/// 時間切れ時にUnityEventを呼び出すTimer
		/// </summary>
		protected Timer m_LifeTimer;

		/// <summary>
		///	リストからランダムな敵を選び、ランダムな点から発射します
		/// </summary>
		/// <param name="enemies">
		/// 抽出元となる敵リスト
		/// </param>
		/// <param name="attack">
		/// 攻撃に使うオブジェクト
		/// </param>
		/// <param name="firingPoints"></param>
		public override void Launch(List<Targetable> enemies, GameObject attack, Transform[] firingPoints)
		{
			var poolable = Poolable.TryGetPoolable<Poolable>(attack);
			if (poolable == null)
			{
				return;
			}
			Targetable enemy = enemies[Random.Range(0, enemies.Count)];
			Transform firingPoint = GetRandomTransform(firingPoints);
			Launch(enemy, poolable.gameObject, firingPoint);
		}

		public override void Launch(Targetable enemy, GameObject attack, Transform firingPoint)
		{
			var homingMissile = attack.GetComponent<HomingLinearProjectile>();
			if (homingMissile == null)
			{
				Debug.LogError("No HomingLinearProjectile attached to attack object");
				return;
			}
			Vector3 startingPoint = firingPoint.position;
			Vector3 targetPoint = Ballistics.CalculateLinearLeadingTargetPoint(
				startingPoint, enemy.position,
				enemy.velocity, homingMissile.startSpeed,
				homingMissile.acceleration);

			homingMissile.SetHomingTarget(enemy);

			var attackAffector = GetComponent<AttackAffector>();
			Vector3 direction = attackAffector.towerTargetter.turret.forward;

			Vector3 binormal = Vector3.Cross(direction, Vector3.up);
			Quaternion rotation = Quaternion.AngleAxis(fireVectorXRotationAdjustment, binormal);

			Vector3 adjustedFireVector = rotation * direction;

			homingMissile.FireInDirection(startingPoint, adjustedFireVector);

			PlayParticles(fireParticleSystem, startingPoint, targetPoint);
		}

		/// <summary>
		/// Level ManagerのonStateChangedを購読します
		/// Waveがすでに始まっている場合は死亡タイマーを開始します
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!LevelManager.instanceExists)
			{
				return;
			}
			LevelState currentState = LevelManager.instance.levelState;
			if (currentState == LevelState.SpawningEnemies || currentState == LevelState.AllEnemiesSpawned)
			{
				m_LifeTimer = new Timer(towerLifeSpan, OnLifeTimerElapsed);
			}
			else
			{
				LevelManager.instance.levelStateChanged += OnLevelStateChanged;
			}
		}

		/// <summary>
		/// Level ManagerのonStateChangedの購読を解除します
		/// </summary>
		protected virtual void OnDisable()
		{
			if (LevelManager.instanceExists)
			{
				LevelManager.instance.levelStateChanged -= OnLevelStateChanged;
			}
		}

		/// <summary>
		/// Timerを進めます
		/// </summary>
		protected void Update()
		{
			if (m_LifeTimer == null)
			{
				return;
			}
			m_LifeTimer.Tick(Time.deltaTime);
		}

		/// <summary>
		/// Timerが終了したらUnityEventを呼び出します
		/// </summary>
		protected void OnLifeTimerElapsed()
		{
			death.Invoke();
		}

		/// <summary>
		/// 現在の状態を確認し、有効な状態であれば死亡タイマーを開始します
		/// </summary>
		/// <param name="previousState">
		/// LevelManagerの直前の状態
		/// </param>
		/// <param name="currentState">
		/// LevelManagerの現在の状態
		/// </param>
		void OnLevelStateChanged(LevelState previousState, LevelState currentState)
		{
			if (currentState == LevelState.SpawningEnemies || currentState == LevelState.AllEnemiesSpawned)
			{
				m_LifeTimer = new Timer(towerLifeSpan, OnLifeTimerElapsed);
			}
		}
	}
}