using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// 体力変化情報。体力変化に関する情報を保持します
	/// </summary>
	public struct HealthChangeInfo
	{
		public Damageable damageable;

		public float oldHealth;

		public float newHealth;

		public IAlignmentProvider damageAlignment;

		public float healthDifference
		{
			get { return newHealth - oldHealth; }
		}

		public float absHealthDifference
		{
			get { return Mathf.Abs(healthDifference); }
		}
	}
}