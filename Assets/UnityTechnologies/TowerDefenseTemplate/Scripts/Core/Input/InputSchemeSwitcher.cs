using UnityEngine;

namespace Core.Input
{
	/// <summary>
	/// 有効な入力スキームを切り替える基底コンポーネント
	/// </summary>
	[DisallowMultipleComponent]
	public class InputSchemeSwitcher : MonoBehaviour
	{
		/// <summary>
		/// アタッチされている入力スキーム
		/// </summary>
		protected InputScheme[] m_InputSchemes;

		/// <summary>
		/// プラットフォームに基づくデフォルトスキーム
		/// </summary>
		protected InputScheme m_DefaultScheme;

		/// <summary>
		/// 現在有効なスキーム
		/// </summary>
		protected InputScheme m_CurrentScheme;

		/// <summary>
		/// スキームをキャッシュし、デフォルトを有効化します
		/// </summary>
		protected virtual void Awake()
		{
			m_InputSchemes = GetComponents<InputScheme>();
			foreach (InputScheme scheme in m_InputSchemes)
			{
				scheme.Deactivate(null);
				if (m_CurrentScheme == null && scheme.isDefault)
				{
					m_DefaultScheme = scheme;
				}
			}
			if (m_DefaultScheme == null)
			{
				Debug.LogError("[InputSchemeSwitcher] Default scheme not set.");
				return;
			}
			m_DefaultScheme.Activate(null);
			m_CurrentScheme = m_DefaultScheme;
		}

		/// <summary>
		/// 各スキームを確認し、必要に応じて有効化します
		/// </summary>
		protected virtual void Update()
		{
			foreach (InputScheme scheme in m_InputSchemes)
			{
				if (scheme.enabled || !scheme.shouldActivate)
				{
					continue;
				}
				if (m_CurrentScheme != null)
				{
					m_CurrentScheme.Deactivate(scheme);
				}
				scheme.Activate(m_CurrentScheme);
				m_CurrentScheme = scheme;
				break;
			}
		}
	}
}