using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// ダメージ情報。一部のダメージリスナーで必要なクラス
	/// </summary>
	public struct HitInfo
	{
		readonly HealthChangeInfo m_HealthChangeInfo;
		readonly Vector3 m_DamagePoint;

		/// <summary>
		/// 体力変化情報を取得または設定します。
		/// </summary>
		/// <value>体力変化情報。</value>
		public HealthChangeInfo healthChangeInfo
		{
			get { return m_HealthChangeInfo; }
		}

		/// <summary>
		/// ダメージ位置を取得または設定します。
		/// </summary>
		/// <value>ダメージ位置。</value>
		public Vector3 damagePoint
		{
			get { return m_DamagePoint; }
		}

		/// <summary>
		/// <see cref="HitInfo" /> 構造体の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="info">体力変化情報</param>
		/// <param name="damageLocation">ダメージ位置。</param>
		public HitInfo(HealthChangeInfo info, Vector3 damageLocation)
		{
			m_DamagePoint = damageLocation;
			m_HealthChangeInfo = info;
		}
	}
}