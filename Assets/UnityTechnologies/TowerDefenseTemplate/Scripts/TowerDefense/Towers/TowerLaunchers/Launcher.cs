using System.Collections.Generic;
using ActionGameFramework.Health;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Towers.TowerLaunchers
{
	public abstract class Launcher : MonoBehaviour, ILauncher
	{
		public abstract void Launch(Targetable enemy, GameObject attack, Transform firingPoint);

		/// <summary>
		/// Poolから攻撃オブジェクトのインスタンスを取得して発射します
		/// </summary>
		/// <param name="enemies">
		/// 抽出元となる敵リスト
		/// </param>
		/// <param name="attack">
		/// 攻撃に使うオブジェクト
		/// </param>
		/// <param name="firingPoints"></param>
		public virtual void Launch(List<Targetable> enemies, GameObject attack, Transform[] firingPoints)
		{
			int count = enemies.Count;
			int currentFiringPointIndex = 0;
			int firingPointLength = firingPoints.Length;
			for (int i = 0; i < count; i++)
			{
				Targetable enemy = enemies[i];
				Transform firingPoint = firingPoints[currentFiringPointIndex];
				currentFiringPointIndex = (currentFiringPointIndex + 1) % firingPointLength;
				var poolable = Poolable.TryGetPoolable<Poolable>(attack);
				if (poolable == null)
				{
					return;
				}
				Launch(enemy, poolable.gameObject, firingPoint);
			}
		}

		/// <summary>
		/// Poolから攻撃インスタンスを取得して発射します
		/// </summary>
		/// <param name="enemy">
		/// Launcherが攻撃している敵
		/// </param>
		/// <param name="attack">
		/// 敵を攻撃するために使うオブジェクト
		/// </param>
		/// <param name="firingPoints"></param>
		public virtual void Launch(Targetable enemy, GameObject attack, Transform[] firingPoints)
		{
			var poolable = Poolable.TryGetPoolable<Poolable>(attack);
			if (poolable == null)
			{
				return;
			}
			Launch(enemy, poolable.gameObject, GetRandomTransform(firingPoints));
		}

		/// <summary>
		/// 照準フィードバック用のParticle Systemを設定します
		/// </summary>
		/// <param name="particleSystemToPlay">
		/// 再生するParticle System
		/// </param>
		/// <param name="origin">
		/// Particle Systemの位置
		/// </param>
		/// <param name="lookPosition">
		/// Particle Systemが向く方向
		/// </param>
		public void PlayParticles(ParticleSystem particleSystemToPlay, Vector3 origin, Vector3 lookPosition)
		{
			if (particleSystemToPlay == null)
			{
				return;
			}
			particleSystemToPlay.transform.position = origin;
			particleSystemToPlay.transform.LookAt(lookPosition);
			particleSystemToPlay.Play();
		}

		/// <summary>
		/// リストからランダムなTransformを取得します
		/// </summary>
		/// <param name="launchPoints">
		/// 使用するTransformのリスト
		/// </param>
		public Transform GetRandomTransform(Transform[] launchPoints)
		{
			int index = Random.Range(0, launchPoints.Length);
			return launchPoints[index];
		}
	}
}