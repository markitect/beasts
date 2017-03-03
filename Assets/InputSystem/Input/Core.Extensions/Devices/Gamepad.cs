using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class Gamepad : InputDevice
	{
		public static Gamepad current { get { return InputSystem.GetCurrentDeviceOfType<Gamepad>(); } }

		public Gamepad() : this("Gamepad") { }

		public Gamepad(string displayName) : base(displayName) { }

		public override void AddStandardControls(ControlSetup setup)
		{
			leftStickLeft = (ButtonControl)setup.AddControl(CommonControls.leftStickLeft);
			leftStickRight = (ButtonControl)setup.AddControl(CommonControls.leftStickRight);
			leftStickDown = (ButtonControl)setup.AddControl(CommonControls.leftStickDown);
			leftStickUp = (ButtonControl)setup.AddControl(CommonControls.leftStickUp);
			leftStickX = (AxisControl)setup.AddControl(CommonControls.leftStickX);
			leftStickY = (AxisControl)setup.AddControl(CommonControls.leftStickY);
			leftStick = (Vector2Control)setup.AddControl(CommonControls.leftStick);
			leftStickButton = (ButtonControl)setup.AddControl(CommonControls.leftStickButton);

			rightStickLeft = (ButtonControl)setup.AddControl(CommonControls.rightStickLeft);
			rightStickRight = (ButtonControl)setup.AddControl(CommonControls.rightStickRight);
			rightStickDown = (ButtonControl)setup.AddControl(CommonControls.rightStickDown);
			rightStickUp = (ButtonControl)setup.AddControl(CommonControls.rightStickUp);
			rightStickX = (AxisControl)setup.AddControl(CommonControls.rightStickX);
			rightStickY = (AxisControl)setup.AddControl(CommonControls.rightStickY);
			rightStick = (Vector2Control)setup.AddControl(CommonControls.rightStick);
			rightStickButton = (ButtonControl)setup.AddControl(CommonControls.rightStickButton);

			dPadLeft = (ButtonControl)setup.AddControl(CommonControls.dPadLeft);
			dPadRight = (ButtonControl)setup.AddControl(CommonControls.dPadRight);
			dPadDown = (ButtonControl)setup.AddControl(CommonControls.dPadDown);
			dPadUp = (ButtonControl)setup.AddControl(CommonControls.dPadUp);
			dPadX = (AxisControl)setup.AddControl(CommonControls.dPadX);
			dPadY = (AxisControl)setup.AddControl(CommonControls.dPadY);
			dPad = (Vector2Control)setup.AddControl(CommonControls.dPad);

			action1 = (ButtonControl)setup.AddControl(CommonControls.action1);
			action2 = (ButtonControl)setup.AddControl(CommonControls.action2);
			action3 = (ButtonControl)setup.AddControl(CommonControls.action3);
			action4 = (ButtonControl)setup.AddControl(CommonControls.action4);

			leftTrigger = (ButtonControl)setup.AddControl(CommonControls.leftTrigger);
			rightTrigger = (ButtonControl)setup.AddControl(CommonControls.rightTrigger);

			leftBumper = (ButtonControl)setup.AddControl(CommonControls.leftBumper);
			rightBumper = (ButtonControl)setup.AddControl(CommonControls.rightBumper);

			vibrationLow = (AxisOutput)setup.AddControl(
				SupportedControl.Get<AxisOutput>("Vibration Low"),
				new AxisOutput("Vibration Low", e => Debug.Log("Vibration Low "+e)));
			vibrationHigh = (AxisOutput)setup.AddControl(
				SupportedControl.Get<AxisOutput>("Vibration High"),
				new AxisOutput("Vibration High", e => Debug.Log("Vibration High "+e)));
		}

		public override void PostProcessState(InputState state)
		{
			// Right now all dead zones come from the default dead zones on the device profile.
			// Maybe in the future copy deadzone over ti fields on Gamepad,
			// either a single field or per control, and make it possible to change through API.
			var joystickProfile = (JoystickProfile)profile;
			Range deadZones;
			if (joystickProfile != null)
				deadZones = joystickProfile.defaultDeadZones;
			else
				deadZones = Range.positive;

			HandleCircularDirectionalControls(
				(ButtonControl)state.controls[leftStickLeft.index],
				(ButtonControl)state.controls[leftStickRight.index],
				(ButtonControl)state.controls[leftStickDown.index],
				(ButtonControl)state.controls[leftStickUp.index],
				(AxisControl)state.controls[leftStickX.index],
				(AxisControl)state.controls[leftStickY.index],
				(Vector2Control)state.controls[leftStick.index],
				deadZones);

			HandleCircularDirectionalControls(
				(ButtonControl)state.controls[rightStickLeft.index],
				(ButtonControl)state.controls[rightStickRight.index],
				(ButtonControl)state.controls[rightStickDown.index],
				(ButtonControl)state.controls[rightStickUp.index],
				(AxisControl)state.controls[rightStickX.index],
				(AxisControl)state.controls[rightStickY.index],
				(Vector2Control)state.controls[rightStick.index],
				deadZones);

			HandleSquareDirectionalControls(
				(ButtonControl)state.controls[dPadLeft.index],
				(ButtonControl)state.controls[dPadRight.index],
				(ButtonControl)state.controls[dPadDown.index],
				(ButtonControl)state.controls[dPadUp.index],
				(AxisControl)state.controls[dPadX.index],
				(AxisControl)state.controls[dPadY.index],
				(Vector2Control)state.controls[dPad.index],
				deadZones);

			((ButtonControl)state.controls[leftTrigger.index]).value =
				GetDeadZoneAdjustedValue(((ButtonControl)state.controls[leftTrigger.index]).value, deadZones);

			((ButtonControl)state.controls[rightTrigger.index]).value =
				GetDeadZoneAdjustedValue(((ButtonControl)state.controls[rightTrigger.index]).value, deadZones);
		}

		public override void PostProcessEnabledControls(InputState state)
		{
			HandleEnabledDirectionalControls(
				state.controls[leftStickLeft.index],
				state.controls[leftStickRight.index],
				state.controls[leftStickDown.index],
				state.controls[leftStickUp.index],
				state.controls[leftStickX.index],
				state.controls[leftStickY.index],
				state.controls[leftStick.index]);

			HandleEnabledDirectionalControls(
				state.controls[rightStickLeft.index],
				state.controls[rightStickRight.index],
				state.controls[rightStickDown.index],
				state.controls[rightStickUp.index],
				state.controls[rightStickX.index],
				state.controls[rightStickY.index],
				state.controls[rightStick.index]);

			HandleEnabledDirectionalControls(
				state.controls[dPadLeft.index],
				state.controls[dPadRight.index],
				state.controls[dPadDown.index],
				state.controls[dPadUp.index],
				state.controls[dPadX.index],
				state.controls[dPadY.index],
				state.controls[dPad.index]);
		}

		void HandleCircularDirectionalControls(
			ButtonControl controlLeft,
			ButtonControl controlRight,
			ButtonControl controlDown,
			ButtonControl controlUp,
			AxisControl controlX,
			AxisControl controlY,
			Vector2Control control,
			Range deadZones
		)
		{
			// Calculate new vector.
			Vector2 vector = new Vector2(-controlLeft.value + controlRight.value, -controlDown.value + controlUp.value);
			float magnitude = vector.magnitude;
			float newMagnitude = GetDeadZoneAdjustedValue(magnitude, deadZones);
			if (newMagnitude == 0)
				vector = Vector2.zero;
			else
				vector *= (newMagnitude / magnitude);

			// Set control values.
			control.value = vector;
			controlX.value = vector.x;
			controlY.value = vector.y;
			controlLeft.value = Mathf.Max(0, -vector.x);
			controlRight.value = Mathf.Max(0, vector.x);
			controlDown.value = Mathf.Max(0, -vector.y);
			controlUp.value = Mathf.Max(0, vector.y);
		}

		void HandleSquareDirectionalControls(
			ButtonControl controlLeft,
			ButtonControl controlRight,
			ButtonControl controlDown,
			ButtonControl controlUp,
			AxisControl controlX,
			AxisControl controlY,
			Vector2Control control,
			Range deadZones
		)
		{
			// Calculate new vector.
			Vector2 vector = new Vector2(-controlLeft.value + controlRight.value, -controlDown.value + controlUp.value);
			vector.x = GetDeadZoneAdjustedValue(vector.x, deadZones);
			vector.y = GetDeadZoneAdjustedValue(vector.y, deadZones);

			// Set control values.
			control.value = vector;
			controlX.value = vector.x;
			controlY.value = vector.y;
			controlLeft.value = Mathf.Max(0, -vector.x);
			controlRight.value = Mathf.Max(0, vector.x);
			controlDown.value = Mathf.Max(0, -vector.y);
			controlUp.value = Mathf.Max(0, vector.y);
		}

		float GetDeadZoneAdjustedValue(float value, Range deadZones)
		{
			float absValue = Mathf.Abs(value);
			if (absValue < deadZones.min)
				return 0;
			if (absValue > deadZones.max)
				return Mathf.Sign(value);
			return Mathf.Sign(value) * ((absValue - deadZones.min) / (deadZones.max - deadZones.min));
		}

		void HandleEnabledDirectionalControls(
			InputControl controlLeft,
			InputControl controlRight,
			InputControl controlDown,
			InputControl controlUp,
			InputControl controlX,
			InputControl controlY,
			InputControl control)
		{
			if (control.enabled)
			{
				controlX.enabled = true;
				controlY.enabled = true;
			}
			if (controlX.enabled)
			{
				controlLeft.enabled = true;
				controlRight.enabled = true;
			}
			if (controlY.enabled)
			{
				controlDown.enabled = true;
				controlUp.enabled = true;
			}
		}

		public ButtonControl leftStickLeft { get; private set; }
		public ButtonControl leftStickRight { get; private set; }
		public ButtonControl leftStickDown { get; private set; }
		public ButtonControl leftStickUp { get; private set; }
		public AxisControl leftStickX { get; private set; }
		public AxisControl leftStickY { get; private set; }
		public Vector2Control leftStick { get; private set; }
		public ButtonControl leftStickButton { get; private set; }

		public ButtonControl rightStickLeft { get; private set; }
		public ButtonControl rightStickRight { get; private set; }
		public ButtonControl rightStickDown { get; private set; }
		public ButtonControl rightStickUp { get; private set; }
		public AxisControl rightStickX { get; private set; }
		public AxisControl rightStickY { get; private set; }
		public Vector2Control rightStick { get; private set; }
		public ButtonControl rightStickButton { get; private set; }

		public ButtonControl dPadLeft { get; private set; }
		public ButtonControl dPadRight { get; private set; }
		public ButtonControl dPadDown { get; private set; }
		public ButtonControl dPadUp { get; private set; }
		public AxisControl dPadX { get; private set; }
		public AxisControl dPadY { get; private set; }
		public Vector2Control dPad { get; private set; }

		public ButtonControl action1 { get; private set; }
		public ButtonControl action2 { get; private set; }
		public ButtonControl action3 { get; private set; }
		public ButtonControl action4 { get; private set; }

		public ButtonControl leftTrigger { get; private set; }
		public ButtonControl rightTrigger { get; private set; }

		public ButtonControl leftBumper { get; private set; }
		public ButtonControl rightBumper { get; private set; }

		public AxisOutput vibrationLow { get; private set; }
		public AxisOutput vibrationHigh { get; private set; }
	}
}
