using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using ActionGameFramework.Projectiles;
using TowerDefense.Level;
using UnityEngine;

namespace TowerDefense.Towers.TowerLaunchers
{
	/// <summary>
	/// Ballistic Projectile用Tower Launcherの実装
	/// </summary>
	public class BallisticLauncher : Launcher
	{
		/// <summary>
		/// 発射フィードバックに使うParticle System
		/// </summary>
		public ParticleSystem fireParticleSystem;

		/// <summary>
		/// 1つの発射ポイントから1体の敵へ1つのProjectileを発射します
		/// </summary>
		/// <param name="enemy">
		/// 狙う敵
		/// </param>
		/// <param name="projectile">
		/// 攻撃に使うProjectile
		/// </param>
		/// <param name="firingPoint">
		/// 発射元の点
		/// </param>
		public override void Launch(Targetable enemy, GameObject projectile, Transform firingPoint)
		{
			Vector3 startPosition = firingPoint.position;
			var ballisticProjectile = projectile.GetComponent<BallisticProjectile>();
			if (ballisticProjectile == null)
			{
				Debug.LogError("No ballistic projectile attached to projectile");
				DestroyImmediate(projectile);
				return;
			}
			Vector3 targetPoint;
			if (ballisticProjectile.fireMode == BallisticFireMode.UseLaunchSpeed)
			{
				// 速度を使います
				targetPoint = Ballistics.CalculateBallisticLeadingTargetPointWithSpeed(
					startPosition,
					enemy.position, enemy.velocity,
					ballisticProjectile.startSpeed, ballisticProjectile.arcPreference, Physics.gravity.y, 4);
			}
			else
			{
				// 角度を使います
				targetPoint = Ballistics.CalculateBallisticLeadingTargetPointWithAngle(
					startPosition,
					enemy.position, enemy.velocity, ballisticProjectile.firingAngle,
					ballisticProjectile.arcPreference, Physics.gravity.y, 4);
			}
			ballisticProjectile.FireAtPoint(startPosition, targetPoint);
			ballisticProjectile.IgnoreCollision(LevelManager.instance.environmentColliders);
			PlayParticles(fireParticleSystem, startPosition, targetPoint);
		}
	}
}