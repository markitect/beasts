using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class GenericMouseProfile : InputDeviceProfile
	{
		static GenericMouseProfile()
		{
			Register();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Register()
		{
			InputSystem.RegisterDeviceProfile<GenericMouseProfile>();
		}

		public GenericMouseProfile()
		{
			displayName = "Mouse";

			// This profile isn't very specific so we put us in the fallback position in case
			// someone wants to match a specific keyboard by device name.
			lastResortDeviceRegex = "type:\\[Mouse\\]";
			neverMatchDeviceRegex = "interface:\\[HID\\]"; // Don't handle mice as HID devices.
		}

		public override bool Remap(InputEvent inputEvent)
		{
			var controlEvent = inputEvent as GenericControlEvent;
			if (controlEvent != null)
			{
				// Map from pointer control enumeration in native code.
				switch (controlEvent.controlIndex)
				{
					case 11: controlEvent.controlIndex = (int)PointerControl.LeftButton; break;
					case 12: controlEvent.controlIndex = (int)PointerControl.RightButton; break;
					case 13: controlEvent.controlIndex = (int)PointerControl.MiddleButton; break;
				}
			}
			return false;
		}

		public override InputDevice TryCreateDevice(string deviceString)
		{
			return new Mouse(deviceString);
		}
	}
}

