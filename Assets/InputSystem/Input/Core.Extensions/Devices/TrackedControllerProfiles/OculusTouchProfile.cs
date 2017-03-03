#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class OculusTouchProfile : GenericVRControllerProfile
	{
		enum Control
		{
			Position,
			Rotation,

			Trigger,
			TriggerTouch,
			TriggerNearTouch,

			HandTrigger,

			Button1, // A (right) or X (left)
			Button1Touch,

			Button2, // B (right) or Y (left)
			Button2Touch,

			ThumbRestTouch,
			// Near touches on thumbsticks and thumb rests come out as the same flag in the
			// Oculus API so we can't differentiate between the two.
			ThumbNearTouch,

			Start,

			StickPress,
			StickTouch,
			StickX,
			StickY,
			StickLeft,
			StickRight,
			StickUp,
			StickDown,
			Stick
		}

		static OculusTouchProfile()
		{
			Register();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Register()
		{
			InputSystem.RegisterDeviceProfile<OculusTouchProfile>();
		}

		public OculusTouchProfile()
		{
			displayName = "Oculus Touch";
			matchingDeviceRegexes = new[] {
				"Oculus.*Touch.*Controller"
			};
			lastResortDeviceRegex = null; // Clear from GenericVRControllerProfile.

			GetControlSetup(new TrackedController());
		}

		public override bool Remap(InputEvent inputEvent)
		{
			const int kMaxNumAxes = 28;

			var controlEvent = inputEvent as GenericControlEvent<float>;
			if (controlEvent != null)
			{
				switch (controlEvent.controlIndex)
				{
					// Indices in the VR module enumerate both left and right independently. We want them
					// to go to the same indices on the left and right controller devices.

					case kMaxNumAxes + 0: controlEvent.controlIndex = (int)Control.Button1; return false;
					case kMaxNumAxes + 1: controlEvent.controlIndex = (int)Control.Button2; return false;
					case kMaxNumAxes + 2: controlEvent.controlIndex = (int)Control.Button1; return false;
					case kMaxNumAxes + 3: controlEvent.controlIndex = (int)Control.Button2; return false;
					case kMaxNumAxes + 10: controlEvent.controlIndex = (int)Control.Button1Touch; return false;
					case kMaxNumAxes + 11: controlEvent.controlIndex = (int)Control.Button2Touch; return false;
					case kMaxNumAxes + 12: controlEvent.controlIndex = (int)Control.Button1Touch; return false;
					case kMaxNumAxes + 13: controlEvent.controlIndex = (int)Control.Button2Touch; return false;

					case kMaxNumAxes + 7: controlEvent.controlIndex = (int)Control.Start; return false;
					case kMaxNumAxes + 8: controlEvent.controlIndex = (int)Control.StickPress; return false;
					case kMaxNumAxes + 9: controlEvent.controlIndex = (int)Control.StickPress; return false;

					case 0: controlEvent.controlIndex = (int)Control.StickX; return false;
					case 1: controlEvent.controlIndex = (int)Control.StickY; return false;
					case 3: controlEvent.controlIndex = (int)Control.StickX; return false;
					case 4: controlEvent.controlIndex = (int)Control.StickY; return false;

					case 8: controlEvent.controlIndex = (int)Control.Trigger; return false;
					case 9: controlEvent.controlIndex = (int)Control.Trigger; return false;

					case kMaxNumAxes + 14: controlEvent.controlIndex = (int)Control.TriggerTouch; return false;
					case kMaxNumAxes + 15: controlEvent.controlIndex = (int)Control.TriggerTouch; return false;

					case 12: controlEvent.controlIndex = (int)Control.TriggerNearTouch; return false;
					case 13: controlEvent.controlIndex = (int)Control.TriggerNearTouch; return false;

					case 10: controlEvent.controlIndex = (int)Control.HandTrigger; return false;
					case 11: controlEvent.controlIndex = (int)Control.HandTrigger; return false;

					case kMaxNumAxes + 16: controlEvent.controlIndex = (int)Control.StickTouch; return false;
					case kMaxNumAxes + 17: controlEvent.controlIndex = (int)Control.StickTouch; return false;

					case 14: controlEvent.controlIndex = (int)Control.ThumbNearTouch; return false;
					case 15: controlEvent.controlIndex = (int)Control.ThumbNearTouch; return false;

					case kMaxNumAxes + 18: controlEvent.controlIndex = (int)Control.ThumbRestTouch; return false;
					case kMaxNumAxes + 19: controlEvent.controlIndex = (int)Control.ThumbRestTouch; return false;
				}
			}

			// Swallow any unrecognized events. This also gets rids of index 2
			// which combines left and right index trigger values into a single axis.
			return true;
		}

		public override ControlSetup GetControlSetup(InputDevice device)
		{
			ControlSetup setup = new ControlSetup(device);

			setup.AddControl(SupportedControl.Get<ButtonControl>("Trigger Touch"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Trigger Near Touch"));

			setup.AddControl(SupportedControl.Get<AxisControl>("Hand Trigger"));

			setup.AddControl(SupportedControl.Get<ButtonControl>("Button 1"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Button 1 Touch"));

			setup.AddControl(SupportedControl.Get<ButtonControl>("Button 2"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Button 2 Touch"));

			setup.AddControl(SupportedControl.Get<ButtonControl>("Thumb Rest Touch"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Thumb Rest Near Touch"));

			setup.AddControl(CommonControls.start);

			////TODO: split mappings
			setup.AddControl(SupportedControl.Get<ButtonControl>("Stick Press"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Stick Touch"));
			setup.AddControl(SupportedControl.Get<AxisControl>("Stick X"));
			setup.AddControl(SupportedControl.Get<AxisControl>("Stick Y"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Stick Left"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Stick Right"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Stick Up"));
			setup.AddControl(SupportedControl.Get<ButtonControl>("Stick Down"));
			setup.AddControl(SupportedControl.Get<Vector2Control>("Stick"));

			return setup;
		}
	}
}

