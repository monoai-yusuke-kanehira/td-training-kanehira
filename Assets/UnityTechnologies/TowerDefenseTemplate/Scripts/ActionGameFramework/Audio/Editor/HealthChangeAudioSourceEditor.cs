using UnityEditor;
using UnityEngine;

namespace ActionGameFramework.Audio.Editor
{
	/// <summary>
	/// <see cref="HealthChangeAudioSource"/> 用のカスタムエディター。必要に応じてサウンドを並べ替える
	/// </summary>
	[CustomEditor(typeof(HealthChangeAudioSource))]
	public class HealthChangeAudioSourceEditor : UnityEditor.Editor
	{
		protected const string k_HelpMessage =
			"This list needs to be sorted in order " +
			"for sounds to be played correctly" +
			"\nList will sort automatically when this component is deselected." +
			"\nYou can also press the \'Sort\' button once you are done editing the sound list.";

		/// <summary>
		/// 選択中の <see cref="HealthChangeAudioSource"/>
		/// </summary>
		protected HealthChangeAudioSource m_Source;

		/// <summary>
		/// <see cref="HealthChangeAudioSource"/> が選択されたときにサウンドを並べ替える
		/// </summary>
		protected void OnEnable()
		{
			m_Source = target as HealthChangeAudioSource;
		}

		/// <summary>
		/// <see cref="HealthChangeAudioSource"/> の選択が外れたときにサウンドを並べ替える
		/// </summary>
		protected void OnDisable()
		{
			Sort();
		}

		/// <summary>
		/// <see cref="HealthChangeAudioSource"/> のサウンドリストを並べ替える
		/// </summary>
		protected void Sort()
		{
			if (m_Source != null)
			{
				m_Source.Sort();
				EditorUtility.SetDirty(m_Source);
			}
		}

		/// <summary>
		/// 編集したサウンドを手動で並べ替えるボタンを表示する
		/// </summary>
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(k_HelpMessage, MessageType.Info);
			base.OnInspectorGUI();
			if (GUILayout.Button("Sort"))
			{
				Sort();
			}
		}
	}
}
