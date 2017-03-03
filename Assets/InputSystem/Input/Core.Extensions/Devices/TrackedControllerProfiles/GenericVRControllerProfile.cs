using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Assets.Utilities;
using UnityEngine.VR;

namespace UnityEngine.Experimental.Input
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class GenericVRControllerProfile : InputDeviceProfile
	{
		static GenericVRControllerProfile()
		{
			Register();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Register()
		{
			InputSystem.RegisterDeviceProfile<GenericVRControllerProfile>();
		}

		public GenericVRControllerProfile()
		{
			displayName = "VR Controller";
			lastResortDeviceRegex = "VR.*[Cc]ontroller";
		}

		public override InputDevice TryCreateDevice(string deviceString)
		{
			var tagIndex = -1;
			if (deviceString.IndexOf("Left", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				tagIndex = (int)TrackedController.Tag.Left;
			}
			else if (deviceString.IndexOf("Right", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				tagIndex = (int)TrackedController.Tag.Right;
			}
			var name = InputDeviceUtility.GetElementFromDeviceString("product", deviceString) ?? displayName;
			return new TrackedController(name, tagIndex);
		}
	}
}

