using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class VirtualJoystick : Joystick
	{
		public enum VirtualJoystickControl
		{
			LeftStickX,
			LeftStickY,
			LeftStick, // Composite

			RightStickX,
			RightStickY,
			RightStick, // Composite

			Action1,
			Action2,
			Action3,
			Action4,
		}

		public static VirtualJoystick current { get { return InputSystem.GetCurrentDeviceOfType<VirtualJoystick>(); } }

		public VirtualJoystick() : this("Virtual Joystick") { }

		public VirtualJoystick(string displayName) : base(displayName) { }

		public override void AddStandardControls(ControlSetup setup)
		{
			setup.AddControl(CommonControls.leftStickX);
			setup.AddControl(CommonControls.leftStickY);
			setup.AddControl(CommonControls.leftStick);

			setup.AddControl(CommonControls.rightStickX);
			setup.AddControl(CommonControls.rightStickY);
			setup.AddControl(CommonControls.rightStick);

			setup.AddControl(CommonControls.action1);
			setup.AddControl(CommonControls.action2);
			setup.AddControl(CommonControls.action3);
			setup.AddControl(CommonControls.action4);
		}

		public AxisControl leftStickX { get { return (AxisControl)this[(int)VirtualJoystickControl.LeftStickX]; } }
		public AxisControl leftStickY { get { return (AxisControl)this[(int)VirtualJoystickControl.LeftStickY]; } }
		public Vector2Control leftStick { get { return (Vector2Control)this[(int)VirtualJoystickControl.LeftStick]; } }

		public AxisControl rightStickX { get { return (AxisControl)this[(int)VirtualJoystickControl.RightStickX]; } }
		public AxisControl rightStickY { get { return (AxisControl)this[(int)VirtualJoystickControl.RightStickY]; } }
		public Vector2Control rightStick { get { return (Vector2Control)this[(int)VirtualJoystickControl.RightStick]; } }

		public ButtonControl action1 { get { return (ButtonControl)this[(int)VirtualJoystickControl.Action1]; } }
		public ButtonControl action2 { get { return (ButtonControl)this[(int)VirtualJoystickControl.Action2]; } }
		public ButtonControl action3 { get { return (ButtonControl)this[(int)VirtualJoystickControl.Action3]; } }
		public ButtonControl action4 { get { return (ButtonControl)this[(int)VirtualJoystickControl.Action4]; } }

		public override void PostProcessState(InputState state)
		{
			((Vector2Control)state.controls[leftStick.index]).value = new Vector2(
				((AxisControl)state.controls[leftStickX.index]).value,
				((AxisControl)state.controls[leftStickY.index]).value);
		}
				

		public void SetValue<T>(InputControl<T> control, T value)
		{
			T currentValue = control.value;
			if (value.Equals(currentValue))
				return;
			
			var inputEvent = InputSystem.CreateEvent<GenericControlEvent<T>>();
			inputEvent.device = this;
			inputEvent.controlIndex = control.index;
			inputEvent.value = value;
			InputSystem.QueueEvent(inputEvent);
		}
	}
}
