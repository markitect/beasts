using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Input;

[CustomEditor(typeof(TrackedAlignment))]
public class TrackedAlignmentEditor : Editor, IControlDomainSource
{
	static class Styles
	{
		public static GUIContent deviceLabel = new GUIContent("Device Type");
		public static GUIContent controlLabel = new GUIContent("Control");
	}

	TrackedAlignment m_TrackedAlignment;

	SerializedProperty m_Source;
	SerializedProperty m_PlayerInput;
	SerializedProperty m_Action;

	void OnEnable()
	{
		m_TrackedAlignment = target as TrackedAlignment;

		m_Source = serializedObject.FindProperty("m_Source");
		m_PlayerInput = serializedObject.FindProperty("m_PlayerInput");
		m_Action = serializedObject.FindProperty("m_Action");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(m_Source);

		EditorGUI.indentLevel++;
		if (m_TrackedAlignment.source == TrackedAlignment.Source.Device)
		{
			m_TrackedAlignment.deviceSlot.OnGUI(
				EditorGUILayout.GetControlRect(),
				Styles.deviceLabel,
				typeof(TrackedInputDevice));

			float height = ControlGUIUtility.GetControlHeight(m_TrackedAlignment.binding, Styles.controlLabel);
			Rect position = EditorGUILayout.GetControlRect(true, height);
			ControlGUIUtility.ControlField(position, m_TrackedAlignment.binding, Styles.controlLabel, this,
				b => m_TrackedAlignment.binding = b as ControlReferenceBinding<Vector3Control, Vector3>);
		}
		else
		{
			EditorGUILayout.PropertyField(m_PlayerInput);
			EditorGUILayout.PropertyField(m_Action);
		}
		EditorGUI.indentLevel--;

		serializedObject.ApplyModifiedProperties();
	}

	public List<DomainEntry> GetDomainEntries()
	{
		return new List<DomainEntry>() { new DomainEntry() { name = "", hash = 0 } };
	}

	public List<DomainEntry> GetControlEntriesOfType(int domainId, Type controlType)
	{
		return InputDeviceUtility.GetDeviceControlEntriesOfType(m_TrackedAlignment.deviceSlot, typeof(Vector3Control));
	}
}
