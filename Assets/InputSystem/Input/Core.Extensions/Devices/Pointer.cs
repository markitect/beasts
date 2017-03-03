using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	/// <summary>
	///     A device that can point at and click on things.
	/// </summary>
	public class Pointer : InputDevice
	{
		public static Pointer current { get { return InputSystem.GetCurrentDeviceOfType<Pointer>(); } }

		public Pointer() : this("Pointer") { }
		
		public Pointer(string displayName) : base(displayName) { }

		public override void AddStandardControls(ControlSetup setup)
		{
			setup.AddControl(CommonControls.position3d);
			setup.AddControl(CommonControls.positionX);
			setup.AddControl(CommonControls.positionY);
			setup.AddControl(CommonControls.positionZ);

			setup.AddControl(CommonControls.delta3d);
			setup.AddControl(CommonControls.deltaX);
			setup.AddControl(CommonControls.deltaY);
			setup.AddControl(CommonControls.deltaZ);

			setup.AddControl(CommonControls.pressure);
			setup.AddControl(CommonControls.tilt);
			setup.AddControl(CommonControls.rotation);
			setup.AddControl(CommonControls.radius);
			setup.AddControl(CommonControls.distance);

			setup.AddControl(CommonControls.leftButton);
			setup.AddControl(CommonControls.rightButton);
			setup.AddControl(CommonControls.middleButton);

			setup.AddControl(CommonControls.scrollWheel3d);
			setup.AddControl(CommonControls.scrollWheelX);
			setup.AddControl(CommonControls.scrollWheelY);
			setup.AddControl(CommonControls.scrollWheelZ);

			setup.AddControl(CommonControls.forwardButton);
			setup.AddControl(CommonControls.backButton);
		}

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			if (base.ProcessEventIntoState(inputEvent, intoState))
				return true;

			var consumed = false;

			var moveEvent = inputEvent as PointerMoveEvent;
			if (moveEvent != null)
			{
				consumed |= intoState.SetCurrentValue((int)PointerControl.PositionX, moveEvent.position.x);
				consumed |= intoState.SetCurrentValue((int)PointerControl.PositionY, moveEvent.position.y);
				consumed |= intoState.SetCurrentValue((int)PointerControl.PositionZ, moveEvent.position.z);

				consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaX, moveEvent.delta.x);
				consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaY, moveEvent.delta.y);
				consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaZ, moveEvent.delta.z);

				return consumed;
			}

			var floatEvent = inputEvent as GenericControlEvent<float>;
			if (floatEvent != null)
			{
				switch ((PointerControl)floatEvent.controlIndex)
				{
				case PointerControl.LeftButton:
					consumed |= intoState.SetCurrentValue((int)PointerControl.LeftButton, floatEvent.value);
					break;
				case PointerControl.MiddleButton:
					consumed |= intoState.SetCurrentValue((int)PointerControl.MiddleButton, floatEvent.value);
					break;
				case PointerControl.RightButton:
					consumed |= intoState.SetCurrentValue((int)PointerControl.RightButton, floatEvent.value);
					break;
				}
			}

			return false;
		}

		public override void PostProcessState(InputState state)
		{
			Vector3 value = new Vector3(
				((AxisControl)state.controls[(int)PointerControl.PositionX]).value,
				((AxisControl)state.controls[(int)PointerControl.PositionY]).value,
				((AxisControl)state.controls[(int)PointerControl.PositionZ]).value
			);
			((Vector3Control)state.controls[(int)PointerControl.Position]).SetValue(value);

			// TODO: Handle other combined controls.
		}
		
		public AxisControl horizontal { get { return positionX; } }
		public AxisControl vertical { get { return positionY;  } }

		public AxisControl horizontalDelta { get { return deltaX; } }
		public AxisControl verticalDelta { get { return deltaY;  } }

		public AxisControl positionX { get { return (AxisControl) this[(int) PointerControl.PositionX]; } }
		public AxisControl positionY { get { return (AxisControl) this[(int) PointerControl.PositionY]; } }
		public AxisControl positionZ { get { return (AxisControl) this[(int) PointerControl.PositionZ]; } }

		public AxisControl deltaX { get { return (AxisControl) this[(int) PointerControl.DeltaX]; } }
		public AxisControl deltaY { get { return (AxisControl) this[(int) PointerControl.DeltaY]; } }
		public AxisControl deltaZ { get { return (AxisControl) this[(int) PointerControl.DeltaZ]; } }

		public ButtonControl leftButton { get { return (ButtonControl) this[(int) PointerControl.LeftButton]; } }
		public ButtonControl rightButton { get { return (ButtonControl) this[(int) PointerControl.RightButton]; } }

		////REVIEW: why do these return values instead of controls?
		public Vector3 position
		{
			get { return ((Vector3Control)this[(int)PointerControl.Position]).value; }
		}

		public float pressure
		{
			get { return ((AxisControl)this[(int)PointerControl.Pressure]).value; }
		}

		////REVIEW: okay, maybe the concept of a per-pointer cursor is bogus after all...
		public Cursor cursor { get; protected set; }
	}

}
