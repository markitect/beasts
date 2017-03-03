using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Assets.Utilities;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class ControlScheme : ISerializationCallbackReceiver, IControlDomainSource
	{
		[SerializeField]
		private string m_Name;
		public string name { get { return m_Name; } set { m_Name = value; } }

		[SerializeField]
		private List<DeviceSlot> m_DeviceSlots = new List<DeviceSlot>();
		public List<DeviceSlot> deviceSlots { get { return m_DeviceSlots; } set { m_DeviceSlots = value; } }

		[SerializeField]
		private ActionMap m_ActionMap;
		public ActionMap actionMap { get { return m_ActionMap; } }
		
		[NonSerialized]
		private List<InputBinding> m_Bindings = new List<InputBinding> ();
		public List<InputBinding> bindings { get { return m_Bindings; } set { m_Bindings = value; } }
		[SerializeField]
		List<SerializationHelper.JSONSerializedElement> m_SerializableBindings = new List<SerializationHelper.JSONSerializedElement>();

		public bool customized { get; internal set; }

		public virtual void OnBeforeSerialize()
		{
			m_SerializableBindings = SerializationHelper.Serialize(m_Bindings);
		}

		public virtual void OnAfterDeserialize()
		{
			m_Bindings = SerializationHelper.Deserialize<InputBinding>(m_SerializableBindings, new object[] {});
			m_SerializableBindings = null;
		}

		public ControlScheme()
		{
		}
		
		public ControlScheme(string name, ActionMap actionMap)
		{
			m_Name = name;
			m_ActionMap = actionMap;
		}

		public virtual ControlScheme Clone()
		{
			var clone = (ControlScheme) Activator.CreateInstance(GetType());
			clone.m_Name = m_Name;
			clone.m_DeviceSlots = m_DeviceSlots.Select(x => x.Clone()).ToList();
			clone.m_ActionMap = m_ActionMap;
			clone.m_Bindings = m_Bindings.Select(x => x == null ? null : x.Clone() as InputBinding).ToList();
			// Don't clone customized flag.
			return clone;
		}

		public int GetDeviceKey(InputDevice device)
		{
			foreach (var deviceSlot in m_DeviceSlots)
			{
				if (device.GetType().IsInstanceOfType(deviceSlot.type.value) &&
					(device.tagIndex == -1 || device.tagIndex == deviceSlot.tagIndex))
					return deviceSlot.key;
			}

			return DeviceSlot.kInvalidKey;
		}

		public DeviceSlot GetDeviceSlot(int key)
		{
			foreach (var deviceSlot in m_DeviceSlots)
			{
				if (deviceSlot.key == key)
					return deviceSlot;
			}

			return null;
		}

		public void ExtractDeviceTypesAndControlHashes(Dictionary<int, List<int>> controlIndicesPerDeviceType)
		{
			List<IEndBinding> endBindings = new List<IEndBinding>();
			ExtractBindingsOfType<IEndBinding>(endBindings);
			foreach (var binding in endBindings)
				binding.ExtractDeviceTypesAndControlHashes(controlIndicesPerDeviceType);
		}

		public void ExtractBindingsOfType<L>(List<L> bindingsList)
		{
			foreach (var binding in bindings)
				if (binding != null)
					binding.ExtractBindingsOfType(bindingsList);
		}

		public void ExtractLabeledEndBindings(List<LabeledBinding> endBindings)
		{
			for (int i = 0; i < bindings.Count; i++)
				if (bindings[i] != null)
					bindings[i].ExtractLabeledEndBindings(actionMap.actions[i].name, endBindings);
		}

		public void Initialize(IInputStateProvider stateProvider)
		{
			for (int i = 0; i < bindings.Count; i++)
				if (bindings[i] != null)
					bindings[i].Initialize(stateProvider);
		}

		public List<DomainEntry> GetDomainEntries()
		{
			return deviceSlots
				.Select(e => e == null ?
					new DomainEntry() { name = string.Empty, hash = -1 } :
					new DomainEntry() { name = e.ToString(), hash = e.key })
				.ToList();
		}

		public List<DomainEntry> GetControlEntriesOfType(int domainId, Type controlType)
		{
			return InputDeviceUtility.GetDeviceControlEntriesOfType(GetDeviceSlot(domainId), controlType);
		}

		public void UpdateUsedControlHashes()
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeUsedControlIndices = new Dictionary<int, List<int>>();
			ExtractDeviceTypesAndControlHashes(perDeviceTypeUsedControlIndices);
			
			foreach (var deviceSlot in deviceSlots)
			{
				List<int> hashes;
				if (perDeviceTypeUsedControlIndices.TryGetValue(deviceSlot.key, out hashes))
				{
					hashes = hashes.Distinct().ToList();
					hashes.Sort();
					deviceSlot.SetSortedCachedUsedControlHashes(hashes);
				}
			}
		}
	}
}
