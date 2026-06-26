using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Health
{
	/// <summary>
	/// ダメージを受け取り、DamageableBehaviour に渡す領域
	/// 実際のダメージ処理は Collision、Trigger、その他の仕組みにできるため抽象基底クラスにしている
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public abstract class DamageZone : MonoBehaviour
	{
		/// <summary>
		/// この Zone に対応する DamageableBehaviour をユーザーが指定できるようにする
		/// 複数の DamageZone から 1 つの DamageableBehaviour を参照できるため、
		/// ヘッドショットのような部位別ダメージを扱える
		/// </summary>
		[Tooltip("If this is empty, DamageZone will try to use a DamageableBehaviour on the same object.")]
		public DamageableBehaviour damageableBehaviour;

		/// <summary>
		/// ダメージの倍率を設定できるようにする。同じ Damager から受けるダメージを Zone ごとに変えられる
		/// </summary>
		public float damageScale = 1f;

		/// <summary>
		/// ダメージを倍率で調整する
		/// </summary>
		/// <returns>調整後のダメージ。</returns>
		/// <param name="damage">元のダメージ。</param>
		protected float ScaleDamage(float damage)
		{
			return damageScale * damage;
		}

		/// <summary>
		/// damageableBehaviour が未設定なら探して割り当てる
		/// エディター上、または以前の LazyLoad() 呼び出しで設定済みの場合がある
		/// </summary>
		protected void LazyLoad()
		{
			if (damageableBehaviour != null)
			{
				return;
			}

			damageableBehaviour = GetComponent<DamageableBehaviour>();
		}
	}
}
