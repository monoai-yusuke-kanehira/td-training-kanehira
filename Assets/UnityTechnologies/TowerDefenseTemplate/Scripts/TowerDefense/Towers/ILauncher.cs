using System.Collections.Generic;
using ActionGameFramework.Health;
using UnityEngine;

namespace TowerDefense.Towers
{
	/// <summary>
	/// TowerConfigurationが委譲できるようにするクラス
	/// 異なる発射ロジックをこのコンポーネントへ渡します
	/// </summary>
	public interface ILauncher
	{
		/// <summary>
		/// Towerの発射ロジックを組み立てるためのメソッド
		/// </summary>
		/// <param name="enemy">
		/// Towerが狙っている敵
		/// </param>
		/// <param name="attack">
		/// 敵を攻撃するために使うProjectileコンポーネント
		/// </param>
		/// <param name="firingPoint"></param>
		void Launch(Targetable enemy, GameObject attack, Transform firingPoint);

		/// <summary>
		/// Towerの発射ロジックを組み立てるためのメソッド
		/// </summary>
		/// <param name="enemy">
		/// Towerが狙っている敵
		/// </param>
		/// <param name="attack">
		/// 敵を攻撃するために使うProjectileコンポーネント
		/// </param>
		/// <param name="firingPoints">
		/// 発射元として使う発射ポイントのリスト
		/// </param>
		void Launch(Targetable enemy, GameObject attack, Transform[] firingPoints);

		/// <summary>
		/// 複数の敵に対する発射ロジックを組み立てるためのメソッド
		/// </summary>
		/// <param name="enemies">
		/// 攻撃対象の敵コレクション
		/// </param>
		/// <param name="attack">
		/// 敵を攻撃するために使うProjectileコンポーネント
		/// </param>
		/// <param name="firingPoints"></param>
		void Launch(List<Targetable> enemies, GameObject attack, Transform[] firingPoints);
	}
}