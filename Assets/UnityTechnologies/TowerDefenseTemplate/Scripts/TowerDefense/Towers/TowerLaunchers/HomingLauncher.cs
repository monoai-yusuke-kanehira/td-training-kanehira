using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using ActionGameFramework.Projectiles;
using UnityEngine;

namespace TowerDefense.Towers.TowerLaunchers
{
	/// <summary>
	/// ホーミングミサイルを発射するILauncherの実装
	/// </summary>
	public class HomingLauncher : Launcher
	{
		public ParticleSystem fireParticleSystem;

		/// <summary>
		/// 開始位置からターゲットへホーミングミサイルを発射します
		/// </summary>
		/// <param name="enemy">
		/// 攻撃する敵
		/// </param>
		/// <param name="attack">
		/// 攻撃に使うProjectile
		/// </param>
		/// <param name="firingPoint">
		/// Projectileの発射元となる点
		/// </param>
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
			homingMissile.FireAtPoint(startingPoint, targetPoint);
			PlayParticles(fireParticleSystem, startingPoint, targetPoint);
		}
	}
}