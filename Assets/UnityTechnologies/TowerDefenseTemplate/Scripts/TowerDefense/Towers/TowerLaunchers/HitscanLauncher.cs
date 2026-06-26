using ActionGameFramework.Health;
using TowerDefense.Towers.Projectiles;
using UnityEngine;

namespace TowerDefense.Towers.TowerLaunchers
{
	/// <summary>
	/// Hitscan攻撃用Tower Launcherの実装
	/// </summary>
	public class HitscanLauncher : Launcher
	{
		/// <summary>
		/// 発射フィードバックに使うParticle System
		/// </summary>
		public ParticleSystem fireParticleSystem;


		/// <summary>
		/// Hitscanオブジェクトに正しいダメージを設定し、
		/// 敵を即座に攻撃します。
		/// 攻撃オブジェクトにHitscanAttack.csがアタッチされていない場合は早期returnします
		/// </summary>
		/// <param name="enemy">
		/// このTowerが狙っている敵
		/// </param>
		/// <param name="attack">
		/// 敵にダメージを与えるために使う攻撃コンポーネント
		/// </param>
		/// <param name="firingPoint"></param>
		public override void Launch(Targetable enemy, GameObject attack, Transform firingPoint)
		{
			var hitscanAttack = attack.GetComponent<HitscanAttack>();
			if (hitscanAttack == null)
			{
				return;
			}
			hitscanAttack.transform.position = firingPoint.position;
			hitscanAttack.AttackEnemy(firingPoint.position, enemy);
			PlayParticles(fireParticleSystem, firingPoint.position, enemy.position);
		}
	}
}