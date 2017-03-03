using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
	public abstract class InputBinding
	{
		public abstract void ExtractBindingsOfType<L>(List<L> bindings);
		public abstract void ExtractLabeledEndBindings(string label, List<LabeledBinding> endBindings);
		public abstract void Initialize(IInputStateProvider stateProvider);
		public abstract object Clone();
		public abstract string GetSourceName(ControlScheme controlScheme, bool forceStandardized);
		public abstract void EndUpdate();

		#if UNITY_EDITOR
		public abstract void OnGUI(Rect position, IControlDomainSource domainSource);
		public abstract float GetPropertyHeight();
		#endif
	}

	public abstract class InputBinding<C, T> : InputBinding, IValueProvider<T> where C : InputControl<T>
	{
		private T m_Value;
		public T value { get { return m_Value; } protected set { m_Value = value; } }
	}

	public interface IEndBinding
	{
		void ExtractDeviceTypesAndControlHashes(Dictionary<int, List<int>> controlIndicesPerDeviceType);
		bool TryBindControl(InputControl control, IInputStateProvider stateProvider);
		string GetSourceName(ControlScheme controlScheme, bool forceStandardized);
	}

	public struct LabeledBinding
	{
		public string label;
		public IEndBinding binding;

		public LabeledBinding(string label, IEndBinding binding)
		{
			this.label = label;
			this.binding = binding;
		}
	}
}
