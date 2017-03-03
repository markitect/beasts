using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
#if UNITY_EDITOR
	[Serializable]
#endif
	internal class InputDeviceManager : IInputHandler
#if UNITY_EDITOR // This class is internal so switching around the API like this is okay.
		, ISerializationCallbackReceiver
#endif
	{
		private List<InputDevice> m_InputDevices = new List<InputDevice>();
		public IEnumerable<InputDevice> devices { get { return m_InputDevices; } }

		private Dictionary<Type, List<InputDevice>> m_DevicesByType = new Dictionary<Type, List<InputDevice>>();
		private Dictionary<Type, InputDevice> m_MostRecentDeviceOfType = new Dictionary<Type, InputDevice>();

		public void RegisterDevice(InputDevice inputDevice)
		{
			if (m_InputDevices.Contains(inputDevice))
				return;

			m_InputDevices.Add(inputDevice);

			var deviceType = inputDevice.GetType();
			RegisterDeviceByTypes(deviceType, inputDevice);
		}

		private void RegisterDeviceByTypes(Type deviceType, InputDevice inputDevice)
		{
			List<InputDevice> list;
			if (!m_DevicesByType.TryGetValue(deviceType, out list))
			{
				list = new List<InputDevice>();
				m_DevicesByType[deviceType] = list;
			}
			list.Add(inputDevice);
			m_MostRecentDeviceOfType[deviceType] = inputDevice;

			var baseType = deviceType.BaseType;
			if (baseType != typeof(InputDevice))
				RegisterDeviceByTypes(baseType, inputDevice);
		}

		public InputDevice GetCurrentDeviceOfType(Type deviceType)
		{
			InputDevice device;
			if (m_MostRecentDeviceOfType.TryGetValue(deviceType, out device))
				return device;
			return null;
		}

		public TDevice GetCurrentDeviceOfType<TDevice>() where TDevice : InputDevice
		{
			return (TDevice)GetCurrentDeviceOfType(typeof(TDevice));
		}

		public int GetDeviceCountOfType(Type deviceType)
		{
			List<InputDevice> list;
			if (!m_DevicesByType.TryGetValue(deviceType, out list))
				return 0;

			return list.Count;
		}

		public InputDevice LookupDevice(Type deviceType, int deviceIndex)
		{
			List<InputDevice> list;
			if (!m_DevicesByType.TryGetValue(deviceType, out list) || deviceIndex >= list.Count)
				return null;

			return list[deviceIndex];
		}

		public int LookupDeviceIndex(InputDevice inputDevice)
		{
			List<InputDevice> list;
			if (!m_DevicesByType.TryGetValue(inputDevice.GetType(), out list))
				return -1;

			return list.IndexOf(inputDevice);
		}

		public bool RemapEvent(InputEvent inputEvent)
		{
			if (inputEvent.device == null)
				return false;

			GenericControlEvent genericEvent = inputEvent as GenericControlEvent;
			if (genericEvent != null && genericEvent.alreadyRemapped)
				return false;

			return inputEvent.device.RemapEvent(inputEvent);
		}

		public bool ProcessEvent(InputEvent inputEvent)
		{
			InputDevice device = inputEvent.device;

			// TODO: Ignore if disconnected.
			if (device != null)
			{
				var consumed = device.ProcessEvent(inputEvent);
				MakeMostRecentDevice(device);
				return consumed;
			}

			return false;
		}

		public void BeginUpdate()
		{
			foreach (var device in devices)
				device.BeginUpdate();
		}

		public void EndUpdate()
		{
			foreach (var device in devices)
				device.EndUpdate();
		}

		void MakeMostRecentDevice(InputDevice device)
		{
			for (var type = device.GetType(); type != typeof(InputDevice); type = type.BaseType)
				m_MostRecentDeviceOfType[type] = device;
		}

// Support for surviving domain reloads.
// We can't leave serialization of InputDevices to Unity directly as it doesn't
// support polymorphism. Instead, we work around it here by serializing snapshots
// of each individual device manually.
#if UNITY_EDITOR
		[Serializable]
		private struct SerializedDeviceState
		{
			public string deviceTypeName;
			////TODO: this should really use the binary serializer but we don't expose that in the public API ATM
			public string deviceState;
			public bool isMostRecentDevice;
		}

		[Serializable]
		private class SerializedState
		{
			public List<SerializedDeviceState> deviceStates = new List<SerializedDeviceState>();
		}

		[SerializeField]
		private SerializedState m_SerializedState;

		public void OnBeforeSerialize()
		{
			m_SerializedState = new SerializedState();
			foreach (var device in m_InputDevices)
			{
				var deviceTypeName = device.GetType().AssemblyQualifiedName;
				var deviceState = JsonUtility.ToJson(device);
				var isMostRecentDevice = (m_MostRecentDeviceOfType[device.GetType()] == device);

				m_SerializedState.deviceStates.Add(
					new SerializedDeviceState
					{
						deviceTypeName = deviceTypeName,
						deviceState = deviceState,
						isMostRecentDevice = isMostRecentDevice
					}
				);
			}
		}

		public void OnAfterDeserialize()
		{
			m_InputDevices = new List<InputDevice>();

			if (m_SerializedState != null)
			{
				m_InputDevices.Capacity = m_SerializedState.deviceStates.Count;
				foreach (var deviceState in m_SerializedState.deviceStates)
				{
					var deviceType = Type.GetType(deviceState.deviceTypeName);
					if (deviceType == null)
					{
						Debug.LogError(string.Format("Cannot find type from name for input device class '{0}'", deviceState.deviceTypeName));
					}
					else if (!typeof(InputDevice).IsAssignableFrom(deviceType))
					{
						Debug.LogError(string.Format("Type '{0}' is not derived from InputDevice", deviceState.deviceTypeName));
					}
					else
					{
						var device = (InputDevice) Activator.CreateInstance(deviceType);
						JsonUtility.FromJsonOverwrite(deviceState.deviceState, device);
						m_InputDevices.Add(device);

						if (deviceState.isMostRecentDevice)
							MakeMostRecentDevice(device);
					}
				}
				m_SerializedState = null;
			}
		}
#endif
	}
}
