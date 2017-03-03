using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	internal class RootBinding<C, T> : InputBinding<C, T>, ISerializationCallbackReceiver where C : InputControl<T>
	{
		[NonSerialized]
		private List<InputBinding<C, T>> m_Sources = new List<InputBinding<C, T>>();
		private List<InputBinding<C, T>> sources { get { return m_Sources; } set { m_Sources = value; } }
		[SerializeField]
		List<SerializationHelper.JSONSerializedElement> m_SerializableSources = new List<SerializationHelper.JSONSerializedElement>();

		// The reference control is needed since it knows how to combine multiple values into one.
		[NonSerialized]
		private InputControl<T> m_ReferenceControl;
		[SerializeField]
		private SerializationHelper.JSONSerializedElement m_SerializableReferenceControl;
		// Invoked by reflection in ActionMap.
		internal void SetReferenceControl(Type type)
		{
			m_ReferenceControl = (InputControl<T>)Activator.CreateInstance(type);
		}

		public RootBinding()
		{
			m_Sources.Add(new ControlReferenceBinding<C, T>());
		}

		public virtual void OnBeforeSerialize()
		{
			m_SerializableSources = SerializationHelper.Serialize(m_Sources);

			m_SerializableReferenceControl = SerializationHelper.SerializeObj(m_ReferenceControl);
		}

		public virtual void OnAfterDeserialize()
		{
			m_Sources = SerializationHelper.Deserialize<InputBinding<C, T>>(m_SerializableSources, new object[] {});
			m_SerializableSources = null;

			m_ReferenceControl = SerializationHelper.DeserializeObj<InputControl<T>>(m_SerializableReferenceControl, new object[] {});
			m_SerializableReferenceControl = new SerializationHelper.JSONSerializedElement();
		}

		public override void EndUpdate()
		{
			for (int i = 0; i < m_Sources.Count; i++)
				m_Sources[i].EndUpdate();
			// InputControl<T> knows how to combine multiple values into one for the specific type of T.
			value = m_ReferenceControl.GetCombinedValue(m_Sources.Select(e => e.value));
		}

		public override object Clone()
		{
			var clone = (RootBinding<C, T>)Activator.CreateInstance(GetType());
			clone.m_Sources = m_Sources.Select(x => x.Clone() as InputBinding<C, T>).ToList();
			clone.m_ReferenceControl = m_ReferenceControl;
			return clone;
		}

		public override void Initialize(IInputStateProvider stateProvider)
		{
			foreach (var control in sources)
			{
				control.Initialize(stateProvider);
			}
		}

		public override void ExtractBindingsOfType<L>(List<L> bindings)
		{
			foreach (var control in sources)
			{
				control.ExtractBindingsOfType(bindings);
			}
		}

		public override void ExtractLabeledEndBindings(string label, List<LabeledBinding> bindings)
		{
			if (sources.Count > 0)
				sources[0].ExtractLabeledEndBindings(label, bindings);
			if (sources.Count > 1)
				sources[1].ExtractLabeledEndBindings(label + " Alt", bindings);
			for (int i = 2; i < sources.Count; i++)
			{
				sources[i].ExtractLabeledEndBindings(label + " Alt " + i, bindings);
			}
		}

		public void Reset() { }

		public override string GetSourceName(ControlScheme controlScheme, bool forceStandardized)
		{
			if (sources == null || sources.Count == 0)
				return "None";

			string str = sources[0].GetSourceName(controlScheme, forceStandardized);
			for (int i = 1; i < sources.Count; i++)
				str = string.Format("{0} / {1}", str, sources[i].GetSourceName(controlScheme, forceStandardized));

			return str;
		}

		#if UNITY_EDITOR
		InputBinding<C, T> m_SelectedSource = null;

		static class Styles
		{
			public static GUIContent iconToolbarPlus =	EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
			public static GUIContent iconToolbarMinus =	EditorGUIUtility.IconContent("Toolbar Minus", "Remove from list");
			public static GUIContent bindingContent = new GUIContent("Binding");
			public const int k_Spacing = 10;
		}

		public override void OnGUI(Rect position, IControlDomainSource domainSource)
		{
			for (int i = 0; i < sources.Count; i++)
			{
				InputBinding<C, T> source = sources[i];
				position.height = ControlGUIUtility.GetControlHeight(source, Styles.bindingContent);
				DrawSourceSettings(position, i, domainSource);
				position.y += position.height + Styles.k_Spacing;
			}
			
			// Remove and add buttons
			position.width = Styles.iconToolbarMinus.image.width;
			position.height = EditorGUIUtility.singleLineHeight;
			if (GUI.Button(position, Styles.iconToolbarMinus, GUIStyle.none))
			{
				if (m_SelectedSource != null && sources.Contains(m_SelectedSource))
					sources.Remove(m_SelectedSource);
			}
			position.x += position.width;
			if (GUI.Button(position, Styles.iconToolbarPlus, GUIStyle.none))
			{
				var source = new ControlReferenceBinding<C, T>();
				sources.Add(source);
				m_SelectedSource = source;
			}
		}
		
		void DrawSourceSettings(Rect position, int bindingIndex, IControlDomainSource domainSource)
		{
			InputBinding<C, T> source = sources[bindingIndex];

			bool used = false;
			if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
			{
				m_SelectedSource = source;
				used = true;
			}
			if (m_SelectedSource == source)
				GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);

			ControlGUIUtility.ControlField(position, source, Styles.bindingContent, domainSource,
				b => sources[bindingIndex] = b);

			if (used)
				Event.current.Use();
		}

		public override float GetPropertyHeight()
		{
			float height = EditorGUIUtility.singleLineHeight;
			for (int i = 0; i < sources.Count; i++)
				height += ControlGUIUtility.GetControlHeight(sources[i], Styles.bindingContent);
			height += Styles.k_Spacing * sources.Count;
			return height;
		}
		#endif
	}
}
