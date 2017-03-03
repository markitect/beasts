using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class DeviceSlot
	{
		public static readonly int kInvalidKey = -1;

		public int key
		{
			get
			{
				return m_Key;
			}
			set
			{
				m_Key = value;
			}
		}

		public SerializableType type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
			}
		}

		public int tagIndex
		{
			get
			{
				return m_TagIndex;
			}
			set
			{
				m_TagIndex = value;
			}
		}

		[SerializeField]
		private int m_Key = kInvalidKey;

		[SerializeField]
		private SerializableType m_Type;

		[SerializeField]
		private int m_TagIndex = -1;

		[SerializeField]
		private List<int> m_SortedCachedUsedControlHashes;

		public DeviceSlot Clone()
		{
			var clone = new DeviceSlot();
			clone.m_Key = m_Key;
			clone.m_TagIndex = m_TagIndex;
			clone.m_Type = m_Type;
			clone.m_SortedCachedUsedControlHashes = new List<int>(m_SortedCachedUsedControlHashes);

			return clone;
		}
		public override string ToString()
		{
			if (type == null || type.value == null)
				return "Invalid Device Slot";
			return type.Name;
		}

		public string GetTagIndexAsString()
		{
			if(type == null || type.value == null)
				return "Invalid Device Slot";

			string[] tagNames = null;  
			tagNames = InputDeviceUtility.GetDeviceTags(type.value);
			if (tagNames != null)
			{
				if(tagIndex >= 0 && tagIndex < tagNames.Length)
				{
					return tagNames[tagIndex];
				}
			}
	
			return "Invalid Device Slot";
		}

		public bool IsDeviceCompatible(InputDevice device)
		{
			if (tagIndex != -1 && tagIndex != device.tagIndex)
				return false;

			// If a device has been setup but no bindings using it, match by type instead.
			if (m_SortedCachedUsedControlHashes.Count == 0 && !type.value.IsInstanceOfType(device))
				return false;

			// Match by supported controls.
			int supportScore = device.GetSupportScoreForSupportedControlHashes(m_SortedCachedUsedControlHashes);
			if (supportScore < 0)
				return false;

			return true;
		}

		public void SetSortedCachedUsedControlHashes(List<int> sortedHashes)
		{
			m_SortedCachedUsedControlHashes = sortedHashes;
		}

		#if UNITY_EDITOR
		public void OnGUI(Rect rect, GUIContent label, Type baseType = null)
		{
			if (baseType == null)
				baseType = typeof(InputDevice);

			string[] tagNames = null;
			Vector2 tagMaxSize = Vector2.zero;
			if (type != null && type.value != null)
			{
				tagNames = InputDeviceUtility.GetDeviceTags(type.value);
				if (tagNames != null)
				{
					GUIContent content = new GUIContent();
					for (var j = 0; j < tagNames.Length; j++)
					{
						content.text = tagNames[j];
						Vector2 size = EditorStyles.popup.CalcSize(content);
						tagMaxSize = Vector2.Max(size, tagMaxSize);
					}
				}
			}

			rect.width -= tagMaxSize.x; // Adjust width to leave room for tag
			EditorGUI.BeginChangeCheck();
			Type t = TypeGUI.TypeField(rect, label, baseType, type);
			if (EditorGUI.EndChangeCheck())
				type = t;
			if (tagNames != null)
			{
				int oldIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;

				EditorGUI.BeginChangeCheck();
				// We want to have the ability to unset a tag after specifying one, so add an "Any" option
				var popupTags = new string[tagNames.Length + 1];
				popupTags[0] = "Any";
				tagNames.CopyTo(popupTags, 1);
				int newTagIndex = tagIndex + 1;
				rect.x += rect.width;
				rect.width = tagMaxSize.x;
				newTagIndex = EditorGUI.Popup(
					rect,
					newTagIndex,
					popupTags);
				if (EditorGUI.EndChangeCheck())
					tagIndex = newTagIndex - 1;

				EditorGUI.indentLevel = oldIndent;
			}
			else
			{
				// if we're no longer showing tags, reset the tag field so that we can still search on the current
				// tag selection of the device slot.
				tagIndex = -1;
			}
		}
		#endif
	}
}
