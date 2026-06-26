using TowerDefense.Agents;
using TowerDefense.Targetting;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Affectors
{
	/// <summary>
	/// <see cref="Agent"/>に<see cref="AgentEffect"/>を適用するために使う抽象クラス
	/// </summary>
	[RequireComponent(typeof(Targetter))]
	public abstract class PassiveAffector : Affector, ITowerRadiusProvider
	{
		/// <summary>
		/// 効果範囲を可視化するときの色
		/// </summary>
		public  Color radiusEffectColor;

		public Targetter towerTargetter;

		/// <summary>
		/// 攻撃範囲を取得または設定する
		/// </summary>
		public float effectRadius
		{
			get { return towerTargetter.effectRadius; }
		}

		/// <summary>
		/// 効果範囲の可視化に使う色を取得する
		/// </summary>
		public Color effectColor
		{
			get { return radiusEffectColor; }
		}

		/// <summary>
		/// Targetterを取得する
		/// </summary>
		public Targetter targetter
		{
			get { return towerTargetter; }
		}
	}
}
