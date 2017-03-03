using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public abstract class InputDevice : InputControlProvider, IInputStateProvider
	{
		private InputDeviceProfile m_Profile;
		private PlayerDeviceAssignment m_Assignment = null;
		protected Dictionary<int, int> m_SupportedControlToControlIndex = new Dictionary<int, int>();
		private List<int> m_SortedCachedUsedControlHashes;

		// For serialization only.
		protected InputDevice()
		{
		}

		protected InputDevice(string displayName)
		{
			this.displayName = displayName;
		}

		public void SetControls(ControlSetup setup)
		{
			SetControls(setup.controls);
			m_SupportedControlToControlIndex = setup.supportedControlIndices;
			m_SortedCachedUsedControlHashes = m_SupportedControlToControlIndex.Keys.Distinct().OrderBy(e => e).ToList();
		}

		////REVIEW: right now the devices don't check whether the event was really meant for them; they go purely by the
		////  type of event they receive. should they check more closely?
		
		public override sealed bool ProcessEvent(InputEvent inputEvent)
		{
			// If event was used, set time, but never consume event.
			// Devices don't consume events.
			if (ProcessEventIntoState(inputEvent, state))
				lastEventTime = inputEvent.time;
			return false;
		}

		public virtual bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			GenericControlEvent controlEvent = inputEvent as GenericControlEvent;
			if (controlEvent == null)
				return false;

			var control = intoState.controls[controlEvent.controlIndex];
			if (!control.enabled)
				return false;
			controlEvent.CopyValueToControl(control);
			return true;
		}

		public virtual void BeginUpdate()
		{
			state.BeginUpdate();
		}

		public void EndUpdate()
		{
			PostProcessState(state);
		}

		////REVIEW: why are these public?
		public virtual void PostProcessState(InputState intoState) { }

		public virtual void PostProcessEnabledControls(InputState intoState) { }

		public virtual bool RemapEvent(InputEvent inputEvent)
		{
			if (profile != null)
				return profile.Remap(inputEvent);
			return false;
		}
		
		private void SetNameOverrides()
		{
			if (profile == null)
				return;
			
			// Assign control override names
			for (int i = 0; i < controlCount; i++) {
				string nameOverride = profile.GetControlNameOverride(i);
				if (nameOverride != null)
					this[i].name = nameOverride;
			}
		}

		public InputState GetDeviceStateForDeviceSlotKey(int deviceKey)
		{
			// For composite bindings on InputDevices the returned state is always
			// the state of the InputDevice itself. We don't need to look at the deviceKey.
			return state;
		}

		public int GetOrAddDeviceSlotKey(InputDevice device)
		{
			return DeviceSlot.kInvalidKey;
		}

		public bool connected { get; internal set; }

		public InputDeviceProfile profile
		{
			get { return m_Profile; }
		}

		// The controls the device assumes are present and may have shortcut properties for.
		public abstract void AddStandardControls(ControlSetup setup);

		public void SetupWithoutProfile()
		{
			ControlSetup setup = new ControlSetup(this);
			SetControls(setup);
		}

		public virtual void SetupFromProfile(InputDeviceProfile profile)
		{
			if (profile != null)
			{
				m_Profile = profile;
				SetControls(profile.GetControlSetup(this));
			}
		}

		// Some input providers need an identifier tag when there are
		// multiple devices of the same type (e.g. left and right hands).
		public virtual int tagIndex
		{
			get { return -1; } // -1 tag means unset or "Any"
		}

		[SerializeField]
		private string m_DisplayName;
		public string displayName
		{
			get { return m_DisplayName; }
			private set { m_DisplayName = value; }
		}

		public PlayerDeviceAssignment assignment
		{
			get
			{
				return m_Assignment;
			}
			set
			{
				m_Assignment = value;
			}
		}

		public override string ToString ()
		{
			return (displayName ?? GetType().Name);
		}

		public InputControl GetControl(SupportedControl supportedControl)
		{
			return GetControlFromHash(supportedControl.hash);
		}

		public override int GetControlIndexFromHash(int hash)
		{
			int controlIndex = -1;
			m_SupportedControlToControlIndex.TryGetValue(hash, out controlIndex);
			return controlIndex;
		}

		public override int GetHashForControlIndex(int controlIndex)
		{
			foreach (var kvp in m_SupportedControlToControlIndex)
			{
				if (kvp.Value == controlIndex)
					return kvp.Key;
			}
			return -1;
		}

		protected List<InputControl> GetControlList(int size)
		{
			List<InputControl> list = new List<InputControl>(size);
			for (int i = 0; i < size; i++)
				list.Add(null);
			return list;
		}

		public int GetSupportScoreForSupportedControlHashes(List<int> neededHashes)
		{
			//Debug.Log("Needed "+string.Join(", ", neededHashes.Select(e => InputSystem.GetSupportedControl(e).standardName).ToArray())+" Provided "+string.Join(", ", m_SortedCachedUsedControlHashes.Select(e => InputSystem.GetSupportedControl(e).standardName).ToArray()));
			int currentNeededIndex = 0;
			int currentProvidedIndex = 0;
			int score = 0;
			bool finished = false;
			while (!finished)
			{
				if (currentNeededIndex >= neededHashes.Count || currentProvidedIndex >= m_SortedCachedUsedControlHashes.Count)
				{
					score -= (neededHashes.Count - currentNeededIndex);
					break;
				}

				int currentNeeded = neededHashes[currentNeededIndex];
				int currentProvided = m_SortedCachedUsedControlHashes[currentProvidedIndex];
				if (currentNeeded == currentProvided)
				{
					currentNeededIndex++;
					currentProvidedIndex++;
					continue;
				}
				if (currentNeeded < currentProvided)
				{
					currentNeededIndex++;
					score--;
					continue;
				}

				currentProvidedIndex++;
			}
			return score;
		}
		
		[SerializeField]
		internal int nativeDeviceId;
	}
}
