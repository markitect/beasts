using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class ControlSetup
	{
		public List<InputControl> controls = new List<InputControl>();
		public Dictionary<int, int> supportedControlIndices = new Dictionary<int, int>();
		public Dictionary<int, IJoystickControlMapping> mappings = new Dictionary<int, IJoystickControlMapping>();

		Type m_DeviceType;
		bool m_DefaultAdditionsAsStandardized;

		public ControlSetup(InputDevice device)
		{
			m_DeviceType = device.GetType();
			m_DefaultAdditionsAsStandardized = true;
			device.AddStandardControls(this);
			m_DefaultAdditionsAsStandardized = false;
		}

		public InputControl AddControl(SupportedControl supportedControl)
		{
			return AddControl(supportedControl, null, m_DefaultAdditionsAsStandardized);
		}

		public InputControl AddControl(SupportedControl supportedControl, InputControl control)
		{
			if (!control.GetType().IsAssignableFrom(supportedControl.controlType.value))
				throw new Exception("Control type does not match type of SupportedControl.");
			return AddControl(supportedControl, control, m_DefaultAdditionsAsStandardized);
		}

		InputControl AddControl(SupportedControl supportedControl, InputControl control, bool standardized)
		{
			int index = -1;
			if (supportedControlIndices.TryGetValue(supportedControl.hash, out index))
				return controls[index];

			if (control == null)
				control = Activator.CreateInstance(supportedControl.controlType.value) as InputControl;
			if (control.name == null)
				control.name = supportedControl.standardName;

			index = controls.Count;
			supportedControlIndices[supportedControl.hash] = index;
			control.index = index;
			controls.Add(control);

			InputSystem.RegisterControl(supportedControl, m_DeviceType, standardized);

			return control;
		}

		public InputControl GetControl(SupportedControl supportedControl)
		{
			return controls[supportedControlIndices[supportedControl.hash]];
		}

		public void Mapping(int sourceIndex, SupportedControl control)
		{
			int index = GetControl(control).index;
			mappings[sourceIndex] = new JoystickControlMapping(index);
		}

		public void Mapping(int sourceIndex, SupportedControl control, Range fromRange, Range toRange)
		{
			int index = GetControl(control).index;
			mappings[sourceIndex] = new JoystickControlMapping(index, fromRange, toRange);
		}

		public void SplitMapping(int sourceIndex, SupportedControl negative, SupportedControl positive)
		{
			int negativeIndex = GetControl(negative).index;
			int positiveIndex = GetControl(positive).index;
			mappings[sourceIndex] = new JoystickControlSplitMapping(negativeIndex, positiveIndex);
		}

		public IJoystickControlMapping[] CreateMappingsArray()
		{
			int highestIndex = -1;
			foreach (var index in mappings.Keys)
				highestIndex = Mathf.Max(highestIndex, index);
			var array = new IJoystickControlMapping[highestIndex + 1];
			foreach (var kvp in mappings)
				array[kvp.Key] = kvp.Value;
			return array;
		}
	}
}
