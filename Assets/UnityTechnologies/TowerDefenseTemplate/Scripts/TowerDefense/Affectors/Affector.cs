using Core.Health;
using UnityEngine;

namespace TowerDefense.Affectors
{
	/// <summary>
	/// 派生クラスの効果が有効かどうかを判断するための
	/// 情報を提供するクラス
	/// </summary>
	public abstract class Affector : MonoBehaviour
	{
		/// <summary>
		/// UIに表示するAffectorの短い説明
		/// </summary>
		public string description;

		/// <summary>
		/// 所属情報を取得または設定する
		/// </summary>
		public IAlignmentProvider alignment { get; protected set; }

		/// <summary>
		/// 判定対象にする物理マスク
		/// </summary>
		public LayerMask enemyMask { get; protected set; }

		/// <summary>
		/// 検索用データを使って効果を初期化する
		/// </summary>
		/// <param name="affectorAlignment">
		/// 検索に使う効果の所属情報
		/// </param>
		/// <param name="mask">
		/// 検索対象にする物理レイヤー
		/// </param>
		public virtual void Initialize(IAlignmentProvider affectorAlignment, LayerMask mask)
		{
			alignment = affectorAlignment;
			enemyMask = mask;
		}

		/// <summary>
		/// 検索用データを使って効果を初期化する
		/// </summary>
		/// <param name="affectorAlignment">
		/// 検索に使う効果の所属情報
		/// </param>
		public virtual void Initialize(IAlignmentProvider affectorAlignment)
		{
			Initialize(affectorAlignment, -1);
		}
	}
}
