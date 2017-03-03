using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
	[CreateAssetMenu()]
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class ActionMap : ScriptableObject, IControlDomainSource
	{
		public static readonly string kDefaultNamespace = "UnityEngine.Experimental.Input";

		[FormerlySerializedAs("entries")]
		[SerializeField]
		private List<InputAction> m_Actions = new List<InputAction>();
		public List<InputAction> actions { get { return m_Actions; } set { m_Actions = value; } }
		
		[SerializeField]
		private List<ControlScheme> m_ControlSchemes = new List<ControlScheme>();
		public List<ControlScheme> controlSchemes
		{
			get { return m_ControlSchemes; }
			set { m_ControlSchemes = value; }
		}

		public Type mapType
		{
			get
			{
				if ( m_CachedMapType == null )
				{
					if (m_MapTypeName == null)
						return null;
					m_CachedMapType = Type.GetType( m_MapTypeName );
				}
				return m_CachedMapType;
			}
			set
			{
				m_CachedMapType = value;
				m_MapTypeName = m_CachedMapType.AssemblyQualifiedName;
			}
		}
		[SerializeField]
		private string m_MapTypeName;
		private Type m_CachedMapType;
		public void SetMapTypeName(string name)
		{
			m_MapTypeName = name;
		}

		public Type customActionMapType {
			get
			{
				Type t = null;

				string typeString = string.Format(
					"{0}.{1}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
					string.IsNullOrEmpty(m_CustomNamespace) ? kDefaultNamespace : m_CustomNamespace,
					name);
				try
				{
					t = Type.GetType(typeString);
				}
				catch { }
				if (t != null)
					return t;

				typeString = string.Format(
					"{0}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
					name);
				try
				{
					t = Type.GetType(typeString);
				}
				catch { }
				if (t != null)
					return t;

				return null;
			}
		}

		[SerializeField]
		private string m_CustomNamespace;
		public string customNamespace
		{
			get
			{
				return m_CustomNamespace;
			}
			set
			{
				m_CustomNamespace = value;
			}
		}

		public void RestoreCustomizations(string customizations)
		{
			var customizedControlSchemes = JsonUtility.FromJson<List<ControlScheme>>(customizations);
			foreach (var customizedScheme in customizedControlSchemes)
			{
				// See if it replaces an existing scheme.
				var replacesExisting = false;
				for (var i = 0; i < controlSchemes.Count; ++i)
				{
					if (String.Compare(controlSchemes[i].name, customizedScheme.name, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0)
					{
						// Yes, so get rid of current scheme.
						controlSchemes[i] = customizedScheme;
						replacesExisting = true;
						break;
					}
				}

				if (!replacesExisting)
				{
					// No, so add as new scheme.
					controlSchemes.Add(customizedScheme);
				}
			}
		}

		public void EnforceBindingsTypeConsistency()
		{
			for (int actionIndex = 0; actionIndex < actions.Count; actionIndex++)
			{
				var action = actions[actionIndex];
				Type controlType = action.controlType;
				Type bindingType = null;
				if (controlType != null)
				{
					// InputControl > InputControl<T>
					// We know the selected type is RootBinding<T> or a derived type.
					// We want to find out what type the T is.
					// Getting first generic argument doesn't work if the selected type
					// is a non-generic class that derives from InputControl<T> (with a specific T)
					// rather than being InputControl<T> itself.
					// So we need to go down to the InputControl<T> base type first.
					Type currentType = controlType;
					while (currentType.BaseType != typeof(InputControl))
					{
						currentType = currentType.BaseType;
						if (currentType == typeof(object))
							throw new Exception("Selected Control Type does not derive from InputControl");
					}

					Type genericArgumentType = currentType.GetGenericArguments()[0];
					bindingType = typeof(RootBinding<,>).MakeGenericType(new System.Type[] { controlType, genericArgumentType });
				}

				// Scheme bindings.
				for (int schemeIndex = 0; schemeIndex < controlSchemes.Count; schemeIndex++)
				{
					ControlScheme scheme = controlSchemes[schemeIndex];
					if (scheme.bindings.Count <= actionIndex)
					{
						if (bindingType == null || action.combined)
							scheme.bindings.Add(null);
						else
							scheme.bindings.Add(CreateBinding(bindingType, controlType));
					}
					else if (((scheme.bindings[actionIndex] == null) != (bindingType == null || action.combined)) ||
						(scheme.bindings[actionIndex] != null && scheme.bindings[actionIndex].GetType() != bindingType))
					{
						if (bindingType == null || action.combined)
							scheme.bindings[actionIndex] = null;
						else
							scheme.bindings[actionIndex] = CreateBinding(bindingType, controlType);
					}
				}

				// Self binding.
				if (((action.selfBinding == null) != (bindingType == null || !action.combined)) ||
					(action.selfBinding != null && action.selfBinding.GetType() != bindingType))
				{
					if (bindingType == null || !action.combined)
						action.selfBinding = null;
					else
						action.selfBinding = CreateBinding(bindingType, controlType);
				}
			}
		}

		InputBinding CreateBinding(Type bindingType, Type controlType)
		{
			var binding = (InputBinding)Activator.CreateInstance(bindingType);
			bindingType
				.GetMethod("SetReferenceControl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				.Invoke(binding, new object[] { controlType });
			return binding;
		}

		public void ExtractCombinedBindingsOfType<L>(List<L> bindingsList)
		{
			foreach (var action in actions)
				if (action.combined && action.selfBinding != null)
					action.selfBinding.ExtractBindingsOfType(bindingsList);
		}

		public List<DomainEntry> GetDomainEntries()
		{
			return new List<DomainEntry>() { new DomainEntry() { name = "Self", hash = 0 } };
		}

		public List<DomainEntry> GetControlEntriesOfType(int domainId, Type controlType)
		{
			return actions
				.Where(e => !e.combined && e.controlType == controlType)
				.Select(e => new DomainEntry() { name = e.name, hash = e.actionIndex })
				.ToList();
		}
	}
}
