using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	// Must be different from Gamepad as the standardized controls for Gamepads don't
	// work for joysticks.
	public class Joystick : InputDevice
	{
		public Joystick() : this("Joystick") { }

		public Joystick(string displayName) : base(displayName) { }

		public override void AddStandardControls (ControlSetup setup)
		{
			
		}
	}
}
