using UnityEngine;
using UnityEngine.Experimental.Input;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace UnityEngine.Experimental.Input
{
	public struct TypePair
	{
		public Type deviceType;
		public Type controlType;
		public TypePair(Type device, Type control) { deviceType = device; controlType = control; } 
	}

	public static class InputDeviceUtility
	{
		static Dictionary<Type, InputDevice> s_DeviceInstances = new Dictionary<Type, InputDevice>();
		static Dictionary<TypePair, List<DomainEntry>> s_DeviceControlEntriesOfType =
			new Dictionary<TypePair, List<DomainEntry>>();
		
		static string[] s_DeviceNames = null;
		static Type[] s_DeviceTypes = null;
		static Dictionary<Type, int> s_IndicesOfDevices = null;
		static List<DomainEntry> s_EmptyList = new List<DomainEntry>();

		public static InputDevice GetDevice(Type type)
		{
			InputDevice device = null;
			if (!s_DeviceInstances.TryGetValue(type, out device))
			{
				device = (InputDevice)System.Activator.CreateInstance(type);
				device.SetupWithoutProfile();
				s_DeviceInstances[type] = device;
			}
			return device;
		}

		public static string GetDeviceSlotName(DeviceSlot deviceSlot)
		{
			string deviceName = "No-Device";

			if (deviceSlot != null && (Type)deviceSlot.type != null)
			{
				if (deviceSlot.tagIndex == -1)
					deviceName = deviceSlot.type.Name;
				else
				{
					string[] tags = GetDeviceTags(deviceSlot.type);
					deviceName = string.Format("{0}.{1}", deviceSlot.type.Name, tags[deviceSlot.tagIndex]);
				}
			}
			return deviceName;
		}

		static void InitializeDeviceControlInfoOfType(DeviceSlot deviceSlot, TypePair key)
		{
			List<DomainEntry> entries = new List<DomainEntry>();

			var dict = InputSystem.GetSupportedControlsForDeviceType(deviceSlot.type);
			foreach (var kvp in dict)
			{
				if (key.controlType.IsAssignableFrom(kvp.Key.controlType.value))
					entries.Add(new DomainEntry()
					{
						name = kvp.Key.standardName,
						hash = kvp.Key.hash,
						standardized = kvp.Value
					});
			}

			s_DeviceControlEntriesOfType[key] = entries;
		}

		public static List<DomainEntry> GetDeviceControlEntriesOfType(DeviceSlot deviceSlot, Type controlType)
		{
			if (deviceSlot == null || deviceSlot.type == null || controlType == null)
				return s_EmptyList;
			var key = new TypePair(deviceSlot.type, controlType);
			List<DomainEntry> entries;
			if (!s_DeviceControlEntriesOfType.TryGetValue(key, out entries))
			{
				InitializeDeviceControlInfoOfType(deviceSlot, key);
				return s_DeviceControlEntriesOfType[key];
			}
			return entries;
		}
		
		static void InitDevices()
		{
			if (s_DeviceTypes != null)
				return;
			
			s_DeviceTypes = (
				from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetExportedTypes()
				where assemblyType.IsSubclassOf(typeof(InputDevice))
				select assemblyType
			).OrderBy(e => GetInheritancePath(e)).ToArray();
			
			s_DeviceNames = s_DeviceTypes.Select(e => string.Empty.PadLeft(GetInheritanceDepth(e) * 3) + e.Name).ToArray();
			
			s_IndicesOfDevices = new Dictionary<Type, int>();
			for (int i = 0; i < s_DeviceTypes.Length; i++)
				s_IndicesOfDevices[s_DeviceTypes[i]] = i;
		}
		
		public static string[] GetDeviceNames()
		{
			InitDevices();
			return s_DeviceNames;
		}
		
		public static int GetDeviceIndex(Type type)
		{
			InitDevices();
			return (type == null ? -1 : s_IndicesOfDevices[type]);
		}
		
		public static Type GetDeviceType(int index)
		{
			InitDevices();
			return s_DeviceTypes[index];
		}

		public static string[] GetDeviceTags(Type type)
		{
			string[] tags = null;
			PropertyInfo info = type.GetProperty("Tags", BindingFlags.Static | BindingFlags.Public);
			if (info != null)
			{
				tags = (string[])info.GetValue(null, null);
			}
			return tags;
		}
		
		static string GetInheritancePath(Type type)
		{
			if (type.BaseType == typeof(InputDevice))
				return type.Name;
			return GetInheritancePath(type.BaseType) + "/" + type.Name;
		}
		
		static int GetInheritanceDepth(Type type)
		{
			if (type.BaseType == typeof(InputDevice))
				return 0;
			return GetInheritanceDepth(type.BaseType) + 1;
		}

		public static string GetElementFromDeviceString(string elementName, string deviceString)
		{
			var startIndex = deviceString.IndexOf(elementName + ':');
			if (startIndex == -1)
				return "";

			startIndex += elementName.Length + 1;
			if (startIndex >= deviceString.Length || deviceString[startIndex] != '[')
				return "";

			++startIndex;
			var closingBracketIndex = deviceString.IndexOf("]", startIndex);

			return deviceString.Substring(startIndex, closingBracketIndex - startIndex);
		}
	}
}
