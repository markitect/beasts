using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public abstract class GamepadProfile : JoystickProfile
	{
		public override InputDevice TryCreateDevice(string deviceString)
		{
			return new Gamepad(deviceString);
		}
	}
}

