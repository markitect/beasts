using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	/*
	Things to test for action map / control schemes.

	- When pressing e.g. mouse button or gamepad trigger in one action map creates a new action map
	  based on the same device, the new action map should not immediately have wasJustPressed be true.
	  Hence the state in the newly created control scheme should be initialized to the state
	  of the devices it's based on.
	
	- When pressing e.g. mouse button or gamepad trigger and it causes a switch in control scheme
	  within an existing action map, the new control scheme *should* immediately have wasJustPressed be true.
	  Hence the state in the newly created control scheme should not be initialized to the state
	  of the devices it's based on.

	*/
	public class ActionMapInput : InputControlProvider, IInputStateProvider
	{
		private ActionMap m_ActionMap;
		public ActionMap actionMap { get { return m_ActionMap; } }

		private ControlScheme m_ControlScheme;
		public ControlScheme controlScheme { get { return m_ControlScheme; } }

		private List<ControlScheme> m_ControlSchemes;
		public List<ControlScheme> controlSchemes
		{
			get { return m_ControlSchemes; }
			set
			{
				int currentIndex = (m_ControlSchemes == null ? 0 : m_ControlSchemes.IndexOf(m_ControlScheme));
				m_ControlSchemes = value;
				m_ControlScheme = m_ControlSchemes[currentIndex];
			}
		}

		private List<InputState> m_DeviceStates = new List<InputState>();
		private List<InputState> deviceStates { get { return m_DeviceStates; } }

		private InputBinding[] m_SelfBindings;

		bool m_Active;
		public bool active {
			get {
				return m_Active;
			}
			set {
				if (m_Active == value)
					return;

				m_Active = value;
				if (onStatusChange != null)
					onStatusChange.Invoke();
			}
		}

		public void ResetControlSchemes()
		{
			// Note: Using property intentially here to have setter logic invoked.
			controlSchemes = m_ActionMap.controlSchemes.Select(e => e.Clone()).ToList();
		}

		// Control whether this ActionMapInput will attempt to reinitialize with applicable devices
		// in order to process events.
		public bool autoReinitialize { get; set; }

		public bool blockSubsequent { get; set; }

		public delegate void ChangeEvent();
		public static ChangeEvent onStatusChange;

		public static ActionMapInput Create(ActionMap actionMap)
		{
			ActionMapInput map =
				(ActionMapInput)Activator.CreateInstance(actionMap.customActionMapType, new object[] { actionMap });
			return map;
		}

		protected ActionMapInput(ActionMap actionMap)
		{
			autoReinitialize = true;
			m_ActionMap = actionMap;
			ResetControlSchemes();

			// Create list of controls from ActionMap.
			var controls = new List<InputControl>();
			foreach (var action in actionMap.actions)
			{
				var control = (InputControl)Activator.CreateInstance(action.controlType);
				control.name = action.name;
				controls.Add(control);
			}
			SetControls(controls);

			m_SelfBindings = new InputBinding[actionMap.actions.Count];
			for (int i = 0; i < actionMap.actions.Count; i++)
			{
				if (actionMap.actions[i].combined)
				{
					m_SelfBindings[i] = (InputBinding)(actionMap.actions[i].selfBinding.Clone());
					m_SelfBindings[i].Initialize(this);
				}
			}
		}

		/// <summary>
		/// Find the best control scheme for the available devices and initialize the action map input.
		/// 
		/// It's important to note that an action map that can use either of two devices should have two of the same device
		/// listed in the control scheme. Otherwise, if the ActionMapInput is initialized with those two devices, the first
		/// device state found will override the other's device state. This becomes apparent when GetDeviceStateForDeviceSlot
		/// is called. 
		///
		/// Ex. Having a left and right VR controller where the action map can accept either controller's trigger button 
		/// would cause issues if only one device was listed in the action map. Usually, this shows up as a ping-ponging
		/// issue where the ActionMapInput keeps getting re-initialized and binds different devices.
		/// </summary>
		/// <param name="availableDevices">Available devices in the system</param>
		/// <param name="requiredDevice">Required device for scheme</param>
		/// <returns></returns>
		public bool TryInitializeWithDevices(IEnumerable<InputDevice> availableDevices,
			IEnumerable<InputDevice> requiredDevices = null,
			int requiredControlSchemeIndex = -1)
		{
			int bestScheme = -1;
			List<InputDevice> bestFoundDevices = null;
			double mostRecentTime = -1;

			int firstScheme = 0;
			int lastScheme = controlSchemes.Count - 1;
			if (requiredControlSchemeIndex >= 0)
			{
				if (requiredControlSchemeIndex > lastScheme)
					return false;
				firstScheme = lastScheme = requiredControlSchemeIndex;
			}

			List<InputDevice> foundDevices = new List<InputDevice>();
			for (int scheme = firstScheme; scheme <= lastScheme; scheme++)
			{
				double timeForScheme = -1;
				foundDevices.Clear();
				var deviceSlots = controlSchemes[scheme].deviceSlots;
				bool matchesAll = true;
				foreach (var deviceSlot in deviceSlots)
				{
					InputDevice foundDevice = null;
					double foundDeviceTime = -1;
					foreach (var device in availableDevices)
					{
						if (!deviceSlot.IsDeviceCompatible(device))
							continue;
						bool required = (requiredDevices != null && requiredDevices.Contains(device));
						if (required || device.lastEventTime > foundDeviceTime)
						{
							foundDevice = device;
							foundDeviceTime = device.lastEventTime;
							if (required)
								break;
						}
					}
					if (foundDevice != null)
					{
						foundDevices.Add(foundDevice);
						timeForScheme = Math.Max(timeForScheme, foundDeviceTime);
					}
					else
					{
						matchesAll = false;
						break;
					}
				}

				// Don't switch schemes in the case where we require a specific device for an event that is getting processed.
				if (matchesAll && requiredDevices != null && requiredDevices.Any())
				{
					foreach (var device in requiredDevices)
					{
						if (!foundDevices.Contains(device))
						{
							matchesAll = false;
							break;
						}
					}
				}

				if (!matchesAll)
					continue;

				// If we reach this point we know that control scheme both matches required and matches all.
				if (timeForScheme > mostRecentTime)
				{
					bestScheme = scheme;
					bestFoundDevices = new List<InputDevice>(foundDevices);
					mostRecentTime = timeForScheme;
				}
			}

			if (bestScheme == -1)
				return false;
			
			ControlScheme matchingControlScheme = controlSchemes[bestScheme];
			Assign(matchingControlScheme, bestFoundDevices);
			return true;
		}

		private void Assign(ControlScheme controlScheme, List<InputDevice> devices)
		{
			m_ControlScheme = controlScheme;

			// Create state for every device.
			var deviceStates = new List<InputState>();
			foreach (var device in devices)
			{
				deviceStates.Add(new InputState(device, GetClonedControlsList(device)));
			}
			m_DeviceStates = deviceStates;
			m_ControlScheme.Initialize(this);
			RefreshBindings();

			if (onStatusChange != null)
				onStatusChange.Invoke();
		}

		private List<InputControl> GetClonedControlsList(InputControlProvider provider)
		{
			List<InputControl> controls = new List<InputControl>();
			for (int i = 0; i < provider.controlCount; i++)
				if (provider[i] == null)
					controls.Add(null);
				else
					controls.Add((InputControl)provider[i].Clone());
			return controls;
		}

		public void SendControlResetEvents()
		{
			for (int i = 0; i < m_DeviceStates.Count; i++)
			{
				var state = m_DeviceStates[i];
				for (int j = 0; j < state.count; j++)
				{
					if (blockSubsequent || (state.controls[j] != null && state.controls[j].enabled))
					{
						var evt = GetControlResetEventForControl(state.controls[j]);
						if (evt == null)
							continue;
						evt.device = state.controlProvider as InputDevice;
						InputSystem.consumers.ProcessEvent(evt);
					}
				}
			}
		}

		private GenericControlEvent GetControlResetEventForControl(InputControl control)
		{
			Type genericType = control.GetType();
			while (genericType.BaseType != typeof(InputControl))
				genericType = genericType.BaseType;
			if (genericType.GetGenericTypeDefinition() != typeof(InputControl<>))
				return null;
			Type genericArgumentType = genericType.GetGenericArguments()[0];
			Type eventType = typeof(GenericControlEvent<>).MakeGenericType(new System.Type[] { genericArgumentType });
			GenericControlEvent evt = (GenericControlEvent)Activator.CreateInstance(eventType);
			evt.controlIndex = control.index;
			evt.CopyDefaultValueFromControl(control);
			evt.time = Time.time;
			return evt;
		}

		public bool CurrentlyUsesDevice(InputDevice device)
		{
			foreach (var deviceState in deviceStates)
				if (deviceState.controlProvider == device)
					return true;
			return false;
		}

		public double GetLastDeviceInputTime()
		{
			double time = 0;
			foreach (var deviceState in deviceStates)
				time = Math.Max(time, deviceState.controlProvider.lastEventTime);
			return time;
		}

		public override bool ProcessEvent(InputEvent inputEvent)
		{
			var consumed = false;
			
			// Update device state (if event actually goes to one of the devices we talk to).
			foreach (var deviceState in deviceStates)
			{
				////FIXME: should refer to proper type
				var device = (InputDevice)deviceState.controlProvider;
				
				// Skip state if event is not meant for device associated with it.
				if (device != inputEvent.device)
					continue;
				
				// Give device a stab at converting the event into state.
				if (device.ProcessEventIntoState(inputEvent, deviceState))
				{
					consumed = true;
					break;
				}
			}
			
			if (!consumed)
				return false;
			
			return true;
		}

		public void Reset(bool initToDeviceState = true)
		{
			if (initToDeviceState)
			{
				foreach (var deviceState in deviceStates)
					deviceState.InitToDevice();
				
				ExtractCurrentValuesFromSources();

				// Copy current values into prev values.
				state.BeginUpdate();
			}
			else
			{
				foreach (var deviceState in deviceStates)
					deviceState.Reset();
				state.Reset();
			}
		}

		public List<InputDevice> GetCurrentlyUsedDevices()
		{
			List<InputDevice> list = new List<InputDevice>();
			for (int i = 0; i < deviceStates.Count; i++)
				list.Add(deviceStates[i].controlProvider as InputDevice);
			return list;
		}

		public InputState GetDeviceStateForDeviceSlotKey(int deviceKey)
		{
			// If deviceKey is 0, return own state instead.
			// This is used by combined bindings which create actions from other actions.
			if (deviceKey == 0)
				return state;

			// Otherwise find relevant device state.
			int deviceSlotIndex = -1;
			for (int i = 0; i < controlScheme.deviceSlots.Count; i++)
				if (controlScheme.deviceSlots[i].key == deviceKey)
					deviceSlotIndex = i;
			if (deviceSlotIndex == -1)
				return null;
			return deviceStates[deviceSlotIndex];
		}

		public int GetOrAddDeviceSlotKey(InputDevice device)
		{
			foreach (var slot in controlScheme.deviceSlots)
				if (slot.IsDeviceCompatible(device))
					return slot.key;
			// TODO: Add new device state if not already present.
			return DeviceSlot.kInvalidKey;
		}

		public void BeginUpdate()
		{
			state.BeginUpdate();
			foreach (var deviceState in deviceStates)
				deviceState.BeginUpdate();
		}
		
		public void EndUpdate()
		{
			foreach (var deviceState in deviceStates)
				(deviceState.controlProvider as InputDevice).PostProcessState(deviceState);
			ExtractCurrentValuesFromSources();
		}

		private void ExtractCurrentValuesFromSources()
		{
			// Fill state that is bound to controls.
			for (var i = 0; i < actionMap.actions.Count; i++)
			{
				InputAction action = actionMap.actions[i];
				if (action.combined)
					continue;
				var binding = controlScheme.bindings[i];
				if (binding != null)
				{
					binding.EndUpdate();
					state.controls[i].CopyValueFromControl(binding);
				}
			}
			// Fill state that is combined from other state.
			// This must be done in a separate pass afterwards,
			// since it relies on the other state having already been filled out.
			for (var i = 0; i < actionMap.actions.Count; i++)
			{
				InputAction action = actionMap.actions[i];
				if (!action.combined)
					continue;
				var binding = m_SelfBindings[i];
				if (binding != null)
				{
					binding.EndUpdate();
					state.controls[i].CopyValueFromControl(binding);
				}
			}
		}

		public override string GetSourceName(int controlIndex)
		{
			return controlScheme.bindings[controlIndex].GetSourceName(null, false);
		}

		////REVIEW: the binding may come from anywhere; method assumes we get passed some state we actually own
		public bool BindControl(IEndBinding binding, InputControl control, bool restrictToExistingDevices)
		{
			if (restrictToExistingDevices)
			{
				bool existingDevice = false;
				for (int i = 0; i < m_DeviceStates.Count; i++)
				{
					if (control.provider == m_DeviceStates[i].controlProvider)
					{
						existingDevice = true;
						break;
					}
				}
				if (!existingDevice)
					return false;
			}
			
			if (!binding.TryBindControl(control, this))
				return false;
			m_ControlScheme.customized = true;
			RefreshBindings();
			return true;
		}

		private void RefreshBindings()
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeUsedControlIndices = new Dictionary<int, List<int>>();
			controlScheme.ExtractDeviceTypesAndControlHashes(perDeviceTypeUsedControlIndices);
			
			for (int slotIndex = 0; slotIndex < controlScheme.deviceSlots.Count; slotIndex++)
			{
				DeviceSlot slot = controlScheme.deviceSlots[slotIndex];
				InputState state = deviceStates[slotIndex];
				List<int> indices;
				if (perDeviceTypeUsedControlIndices.TryGetValue(slot.key, out indices))
				{
					state.SetUsedControls(indices.Select(e => state.controlProvider.GetControlIndexFromHash(e)).ToList());
					(state.controlProvider as InputDevice).PostProcessEnabledControls(state);
				}
				else
				{
					state.SetAllControlsEnabled(false);
				}
			}
		}

		public override int GetControlIndexFromHash(int hash)
		{
			return hash;
		}

		public override int GetHashForControlIndex(int controlIndex)
		{
			return controlIndex;
		}
	}

	[Serializable]
	public class ActionMapSlot
	{
		public ActionMap actionMap;
		public bool active = true;
		public bool blockSubsequent;
	}
}
