using UnityEngine;

namespace Core.Input
{
	/// <summary>
	/// いつどのように自身を有効化するかを知っている入力スキームの基底クラス
	/// </summary>
	public abstract class InputScheme : MonoBehaviour
	{
		/// <summary>
		/// このスキームを有効化すべきかどうかを取得します
		/// </summary>
		public abstract bool shouldActivate { get; }

		/// <summary>
		/// このスキームをデフォルトにするべきかどうかを取得します
		/// </summary>
		public abstract bool isDefault { get; }

		/// <summary>
		/// まだ有効でない場合は有効化します
		/// </summary>
		/// <param name="previousScheme">
		/// 以前有効だったスキーム。
		/// 起動時はnullになります。
		/// </param>
		public virtual void Activate(InputScheme previousScheme)
		{
			if (!enabled)
			{
				enabled = true;
			}
		}

		/// <summary>
		/// まだ無効でない場合は無効化します
		/// </summary>
		/// <param name="nextScheme">
		/// 次に有効化されるスキーム。
		/// 起動時はnullになります。
		/// </param>
		public virtual void Deactivate(InputScheme nextScheme)
		{
			if (enabled)
			{
				enabled = false;
			}
		}
	}
}