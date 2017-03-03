using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal.Input;

////REVIEW: Given that native device IDs are stored on devices, why can't we just have InputDeviceManager do the lookups?

namespace UnityEngine.Experimental.Input
{
	// Listens for device discoveries reported by the native runtime and maps
	// those reports into InputDevice instances. Keeps a persistent list of
	// native device information.
#if UNITY_EDITOR
	[Serializable]
#endif
	internal class NativeInputDeviceManager : INativeInputDeviceManager
	{
		[NonSerialized]
		private bool m_IsInitialized;

		// Not serialized (otherwise we'd end up with two copies of the managerse)
		// so we need to re-establish these connections after every reload.
		private InputDeviceManager m_DeviceManager;
		private InputDeviceProfileManager m_ProfileManager;

		// Binds a NativeInputDeviceInfo record reported by native
		// together with the InputDevice created from it.
		[Serializable]
		private struct NativeDeviceRecord
		{
			[NonSerialized]
			public InputDevice device; // Nulled out after every reload.
			////REVIEW: In the player, we don't really need anything from here except the device ID
			public NativeInputDeviceInfo deviceInfo;
		};

		// Persistent record of the native devices we have.
		[SerializeField]
		private List<NativeDeviceRecord> m_NativeDevices = new List<NativeDeviceRecord>();

		public void Initialize(InputDeviceManager deviceManager, InputDeviceProfileManager profileManager)
		{
			if (m_IsInitialized)
				return;

			m_DeviceManager = deviceManager;
			m_ProfileManager = profileManager;

			// Reconnect our NativeDeviceRecords to the InputDevices in the manager.
			// The device manager survives a reload, we don't. But all the information
			// we need is in the native device IDs stored on the devices and persisted
			// across reloads.
			foreach (var device in m_DeviceManager.devices)
			{
				if (device.nativeDeviceId == 0)
					continue;

				for (var i = 0; i < m_NativeDevices.Count; ++i)
				{
					if (m_NativeDevices[i].deviceInfo.deviceId != device.nativeDeviceId)
						continue;

					m_NativeDevices[i] = new NativeDeviceRecord
					{
						device = device,
						deviceInfo = m_NativeDevices[i].deviceInfo
					};
					break;
				}
			}

			// Hook into notifications for when the native runtime discovers new devices.
			NativeInputSystem.onDeviceDiscovered += CreateNativeInputDevice;

			m_IsInitialized = true;
		}

		public void Uninitialize()
		{
			if (!m_IsInitialized)
				return;

			m_DeviceManager = null;
			m_ProfileManager = null;

			NativeInputSystem.onDeviceDiscovered -= CreateNativeInputDevice;

			m_IsInitialized = false;
		}

		public InputDevice FindInputDeviceByNativeDeviceId(int nativeDeviceId)
		{
			////TODO: probably want a faster lookup
			foreach (var record in m_NativeDevices)
				if (record.deviceInfo.deviceId == nativeDeviceId)
					return record.device;
			return null;
		}

		// When we reset, we want to create a new set of InputDevices for native devices in the
		// system. Use the old manager's list to create the devices from.
		public void RecreateNativeDevicesFrom(NativeInputDeviceManager oldManager)
		{
			foreach (var record in oldManager.m_NativeDevices)
				CreateNativeInputDevice(record.deviceInfo);
		}

		struct NativeDeviceDescriptor
		{
			public string @interface;
			public string type;
			public string product;
			public string manufacturer;
			public string version;
		}

		// Called whenever the native runtime discovers a new device.
		// All device registrations are sent before events are sent.
		private void CreateNativeInputDevice(NativeInputDeviceInfo deviceInfo)
		{
			var descriptor = JsonUtility.FromJson<NativeDeviceDescriptor>(deviceInfo.deviceDescriptor);
			var deviceString = string.Format("product:[{0}] manufacturer:[{1}] interface:[{2}] type:[{3}] version:[{4}]",
				descriptor.product, descriptor.manufacturer, descriptor.@interface, descriptor.type, descriptor.version);

			// Try to find a profile for the device. If we have none, we simply
			// ignore the device entirely.
			var profile = m_ProfileManager.FindProfileByDeviceString(deviceString);
			if (profile != null)
			{
				var device = profile.TryCreateDevice(deviceString);
				if (device != null)
				{
					device.SetupFromProfile(profile);

					// Associate the device with the native device ID.
					device.nativeDeviceId = deviceInfo.deviceId;
					m_NativeDevices.Add(
						new NativeDeviceRecord
						{
							device = device,
							deviceInfo = deviceInfo
						});

					// And add it into the system.
					m_DeviceManager.RegisterDevice(device);
				}
			}
			else
			{
				////TODO: disable the device in native
				//NativeInputSystem.SetDeviceConfiguration(deviceInfo.deviceId, "{enabled:false}");
			}
		}
	}
}

