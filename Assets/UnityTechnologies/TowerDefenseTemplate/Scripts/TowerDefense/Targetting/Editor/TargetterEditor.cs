using UnityEditor;
using UnityEngine;

namespace TowerDefense.Targetting.Editor
{
	/// <summary>
	/// Targetterを設定するためのEditor
	/// </summary>
	[CustomEditor(typeof(Targetter)), CanEditMultipleObjects]
	public class TargetterEditor : UnityEditor.Editor
	{
		/// <summary>
		/// 使用するColliderの設定
		/// </summary>
		public enum TargetterCollider
		{
			/// <summary>
			/// Sphere Collider用
			/// </summary>
			Sphere,

			/// <summary>
			/// Capsule Collider用
			/// </summary>
			Capsule
		}

		/// <summary>
		/// 編集対象のTargetter
		/// </summary>
		Targetter m_Targetter;

		/// <summary>
		/// 使用する衝突設定
		/// </summary>
		TargetterCollider m_ColliderConfiguration;

		/// <summary>
		/// Colliderの半径
		/// </summary>
		float m_ColliderRadius;

		// Capsule固有の情報

		/// <summary>
		/// Capsule Colliderの高さ
		/// </summary>
		float m_ExtraVerticalRange;

		/// <summary>
		/// アタッチされているCollider
		/// </summary>
		Collider m_AttachedCollider;

		/// <summary>
		/// <see cref="m_AttachedCollider"/>を表すSerializedProperty
		/// </summary>
		SerializedProperty m_SerializedAttachedCollider;

		/// <summary>
		/// デフォルトのInspectorを描画します
		/// その後、Collider用の設定を描画します
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			// Inspectorを少し見やすくするため
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Targetter Collider Configuration", EditorStyles.boldLabel);

			m_ColliderConfiguration =
				(TargetterCollider) EditorGUILayout.EnumPopup("Targetter Collider", m_ColliderConfiguration);
			AttachCollider();
			m_ColliderRadius = EditorGUILayout.FloatField("Radius", m_ColliderRadius);
			if (m_ColliderConfiguration == TargetterCollider.Capsule)
			{
				m_ExtraVerticalRange = EditorGUILayout.FloatField("Vertical Range", m_ExtraVerticalRange);
			}
			SetValues();
			EditorUtility.SetDirty(m_Targetter);
			EditorUtility.SetDirty(m_AttachedCollider);
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// 適切なColliderをアタッチして非表示にするため
		/// </summary>
		void AttachCollider()
		{
			switch (m_ColliderConfiguration)
			{
				case TargetterCollider.Sphere:
					if (m_AttachedCollider is SphereCollider)
					{
						GetValues();
						return;
					}
					if (m_AttachedCollider != null)
					{
						DestroyImmediate(m_AttachedCollider, true);
					}
					m_AttachedCollider = m_Targetter.gameObject.AddComponent<SphereCollider>();
					m_SerializedAttachedCollider.objectReferenceValue = m_AttachedCollider;
					break;
				case TargetterCollider.Capsule:
					if (m_AttachedCollider is CapsuleCollider)
					{
						GetValues();
						return;
					}
					if (m_AttachedCollider != null)
					{
						DestroyImmediate(m_AttachedCollider, true);
					}
					m_AttachedCollider = m_Targetter.gameObject.AddComponent<CapsuleCollider>();
					m_SerializedAttachedCollider.objectReferenceValue = m_AttachedCollider;
					break;
			}
			SetValues();
			m_AttachedCollider.hideFlags = HideFlags.HideInInspector;
		}

		/// <summary>
		/// Colliderに値を設定します
		/// </summary>
		void SetValues()
		{
			switch (m_ColliderConfiguration)
			{
				case TargetterCollider.Sphere:
					var sphere = (SphereCollider) m_AttachedCollider;
					sphere.radius = m_ColliderRadius;
					break;
				case TargetterCollider.Capsule:
					var capsule = (CapsuleCollider) m_AttachedCollider;
					capsule.radius = m_ColliderRadius;
					capsule.height = m_ExtraVerticalRange + m_ColliderRadius * 2;
					break;
			}
		}

		/// <summary>
		/// Colliderから情報を取得します
		/// </summary>
		void GetValues()
		{
			switch (m_ColliderConfiguration)
			{
				case TargetterCollider.Sphere:
					var sphere = (SphereCollider) m_AttachedCollider;
					m_ColliderRadius = sphere.radius;
					break;
				case TargetterCollider.Capsule:
					var capsule = (CapsuleCollider) m_AttachedCollider;
					m_ColliderRadius = capsule.radius;
					m_ExtraVerticalRange = capsule.height - m_ColliderRadius * 2;
					break;
			}
		}

		/// <summary>
		/// Colliderをキャッシュして非表示にします
		/// そこから必要な情報をすべて設定します
		/// </summary>
		void OnEnable()
		{
			m_Targetter = (Targetter) target;
			m_SerializedAttachedCollider = serializedObject.FindProperty("attachedCollider");
			m_AttachedCollider = (Collider) m_SerializedAttachedCollider.objectReferenceValue;

			if (m_AttachedCollider == null)
			{
				m_AttachedCollider = m_Targetter.GetComponent<Collider>();
				if (m_AttachedCollider == null)
				{
					switch (m_ColliderConfiguration)
					{
						case TargetterCollider.Sphere:
							m_AttachedCollider = m_Targetter.gameObject.AddComponent<SphereCollider>();
							break;
						case TargetterCollider.Capsule:
							m_AttachedCollider = m_Targetter.gameObject.AddComponent<CapsuleCollider>();
							break;
					}
					m_SerializedAttachedCollider.objectReferenceValue = m_AttachedCollider;
				}
			}
			if (m_AttachedCollider is SphereCollider)
			{
				m_ColliderConfiguration = TargetterCollider.Sphere;
			}
			else if (m_AttachedCollider is CapsuleCollider)
			{
				m_ColliderConfiguration = TargetterCollider.Capsule;
			}
			// SerializedObjectからColliderが参照されるようにします
			if (m_SerializedAttachedCollider.objectReferenceValue == null)
			{
				m_SerializedAttachedCollider.objectReferenceValue = m_AttachedCollider;
			}
			GetValues();
			m_AttachedCollider.isTrigger = true;
			m_AttachedCollider.hideFlags = HideFlags.HideInInspector;
		}
	}
}