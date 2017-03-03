using System;
using Assets.Utilities;

namespace UnityEngine.Experimental.Input
{
	public abstract class InputDeviceProfile
	{
		public string[] matchingDevices;
		public string[] matchingDeviceRegexes;
		public string lastResortDeviceRegex;
		public string neverMatchDeviceRegex { get; protected set; }

		////REVIEW: do these still make sense or are the #ifs good enough?
		public Version minUnityVersion;
		public Version maxUnityVersion;

		public string displayName { get; protected set; }
		
		////TODO: instead of bools, we should turn that into enums (same for ProcessEventIntoState); it's hard to remember otherwise what true/false means
		public virtual bool Remap(InputEvent inputEvent)
		{
			return false;
		}

		public void AddMatchingDevice(string deviceString)
		{
			ArrayHelpers.AppendUnique(ref matchingDevices, deviceString);
		}

		public void AddMatchingDeviceRegex(string regex)
		{
			ArrayHelpers.AppendUnique(ref matchingDeviceRegexes, regex);
		}
		
		public virtual string GetControlNameOverride(int controlIndex)
		{
			return null;
		}

		// NOTE: This method is allowed to return null which means the profile opts
		//       for the device to be ignored.
		public abstract InputDevice TryCreateDevice(string deviceString);

		public virtual ControlSetup GetControlSetup(InputDevice device)
		{
			return new ControlSetup(device);
		}
	}
}
