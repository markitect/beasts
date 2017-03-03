using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
	public class ControlGUIUtility
	{
		#if UNITY_EDITOR
		public static void ControlField<C, T>(Rect position, InputBinding<C, T> binding, GUIContent label, IControlDomainSource domainSource, Action<InputBinding<C, T>> action) where C : InputControl<T>
		{
			position.height = EditorGUIUtility.singleLineHeight;

			Rect buttonPosition = EditorGUI.PrefixLabel(position, label);

			if (EditorGUI.DropdownButton(buttonPosition, new GUIContent(GetName(binding, domainSource)), FocusType.Keyboard))
			{
				GenericMenu menu = GetMenu(binding, domainSource, action);
				menu.DropDown(buttonPosition);
			}

			if (binding != null && !(binding is ControlReferenceBinding<C, T>))
			{
				EditorGUI.indentLevel++;
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				binding.OnGUI(position, domainSource);
				EditorGUI.indentLevel--;
			}
		}

		static string GetName<C, T>(InputBinding<C, T> binding, IControlDomainSource domainSource) where C : InputControl<T>
		{
			if (binding == null || domainSource == null)
				return null;

			var reference = binding as ControlReferenceBinding<C, T>;
			if (reference != null)
			{
				int domainIndex = domainSource.GetDomainEntries().FindIndex(e => e.hash == reference.deviceKey);
				if (domainIndex < 0)
					return null;
				var domainEntries = domainSource.GetDomainEntries();
				if (domainIndex >= domainEntries.Count)
					return null;
				string domainName = domainEntries[domainIndex].name;
				if (!string.IsNullOrEmpty(domainName))
					domainName = domainName + "/";
				return domainName + InputSystem.GetSupportedControl(reference.controlHash).standardName;
			}

			return NicifyBindingName(binding.GetType().Name);
		}

		static GenericMenu GetMenu<C, T>(InputBinding<C, T> binding, IControlDomainSource domainSource, Action<InputBinding<C, T>> action) where C : InputControl<T>
		{
			GenericMenu menu = new GenericMenu();

			Type[] derivedTypes = null;
			string[] derivedNames = null;
			Dictionary<Type, int> indicesOfDerivedTypes = null;
			TypeGUI.GetDerivedTypesInfo(typeof(InputBinding<C, T>), out derivedTypes, out derivedNames, out indicesOfDerivedTypes);


			Type bindingType = typeof(ControlReferenceBinding<C, T>);
			Type existingType = binding == null ? null : binding.GetType();

			var reference = binding as ControlReferenceBinding<C, T>;

			// Add control references for devices.
			bool hasReferences = false;
			if (derivedTypes.Contains(bindingType))
			{
				hasReferences = true;
				List<DomainEntry> domainEntries = domainSource.GetDomainEntries();
				for (int i = 0; i < domainEntries.Count; i++)
				{
					int domainHash = domainEntries[i].hash;
					List<DomainEntry> controlEntries = domainSource.GetControlEntriesOfType(domainHash, typeof(C));

					bool showFlatList = (domainEntries.Count <= 1 && controlEntries.Count <= 20);
					string prefix = showFlatList ? string.Empty : domainEntries[i].name + "/";

					bool nonStandardizedSectionStart = false;
					for (int j = 0; j < controlEntries.Count; j++)
					{
						bool selected = (reference != null
							&& reference.deviceKey == domainHash
							&& reference.controlHash == controlEntries[j].hash);

						if (!nonStandardizedSectionStart && !controlEntries[j].standardized)
						{
							nonStandardizedSectionStart = true;
							menu.AddSeparator(prefix);
						}

						GUIContent name = new GUIContent(prefix + controlEntries[j].name);
						int index = j; // See "close over the loop variable".
						menu.AddItem(name, selected,
							() => {
								var newReference = new ControlReferenceBinding<C, T>();
								newReference.deviceKey = domainHash;
								newReference.controlHash = controlEntries[index].hash;
								action(newReference);
							});
					}
				}
			}

			if (derivedTypes.Length <= (hasReferences ? 1 : 0))
				return menu;

			menu.AddSeparator("");

			// Add other control types.
			for (int i = 0; i < derivedTypes.Length; i++)
			{
				if (derivedTypes[i] != bindingType)
				{
					bool selected = (existingType == derivedTypes[i]);
					string name = NicifyBindingName(derivedNames[i]);
					int index = i; // See "close over the loop variable".
					menu.AddItem(new GUIContent(name), selected,
						() => {
							var newBinding = Activator.CreateInstance(derivedTypes[index]) as InputBinding<C, T>;
							action(newBinding);
						});
				}
			}

			return menu;
		}

		static string NicifyBindingName(string bindingTypeName)
		{
			return ObjectNames.NicifyVariableName(bindingTypeName.Replace("Binding", string.Empty));
		}

		public static float GetControlHeight<C, T>(InputBinding<C, T> control, GUIContent label) where C : InputControl<T>
		{
			if (control == null || control is ControlReferenceBinding<C, T>)
				return EditorGUIUtility.singleLineHeight;
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + control.GetPropertyHeight();
		}
		#endif
	}
}
