using UnityEngine;
using Assets.Utilities;

namespace UnityEngine.Experimental.Input
{
	public abstract class JoystickProfile : InputDeviceProfile
	{
		public IJoystickControlMapping[] mappings;
		public string[] nameOverrides;

		public Range defaultDeadZones = new Range(0.2f, 0.9f);

		public override bool Remap(InputEvent inputEvent)
		{
			var controlEvent = inputEvent as GenericControlEvent;
			if (controlEvent != null)
			{
				var mapping = mappings[controlEvent.controlIndex];
				if (mapping != null && mapping.Remap(controlEvent))
					return true;
			}
			return false;
		}
		
		public override string GetControlNameOverride(int controlIndex)
		{
			if (controlIndex >= nameOverrides.Length)
				return null;
			return nameOverrides[controlIndex];
		}
	}
}
