#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

namespace UnityEngine.Experimental.Input
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class Xbox360WinProfile : GamepadProfile
	{
		static Xbox360WinProfile()
		{
			Register();
		}

		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Register()
		{
			InputSystem.RegisterDeviceProfile<Xbox360WinProfile>();
		}

		public Xbox360WinProfile()
		{
			////REVIEW: these probably require adjustments; I'm pretty sure the new code won't report most if not all controllers with those exact names
			AddMatchingDevice("AIRFLO             ");
			AddMatchingDevice("AxisPad");
			AddMatchingDevice("Controller (Afterglow Gamepad for Xbox 360)");
			AddMatchingDevice("Controller (Batarang wired controller (XBOX))");
			AddMatchingDevice("Controller (Gamepad for Xbox 360)");
			AddMatchingDevice("Controller (GPX Gamepad)");
			AddMatchingDevice("Controller (Infinity Controller 360)");
			AddMatchingDevice("Controller (Mad Catz FPS Pro GamePad)");
			AddMatchingDevice("Controller (MadCatz Call of Duty GamePad)");
			AddMatchingDevice("Controller (MadCatz GamePad)");
			AddMatchingDevice("Controller (MLG GamePad for Xbox 360)");
			AddMatchingDevice("Controller (Razer Sabertooth Elite)");
			AddMatchingDevice("Controller (Rock Candy Gamepad for Xbox 360)");
			AddMatchingDevice("Controller (SL-6566)");
			AddMatchingDevice("Controller (Xbox 360 For Windows)");
			AddMatchingDevice("Controller (Xbox 360 Wireless Receiver for Windows)");
			AddMatchingDevice("Controller (Xbox Airflo wired controller)");
			AddMatchingDevice("Controller (XEOX Gamepad)");
			AddMatchingDevice("Cyborg V.3 Rumble Pad");
			AddMatchingDevice("Generic USB Joystick ");
			AddMatchingDevice("MadCatz GamePad (Controller)");
			AddMatchingDevice("Saitek P990 Dual Analog Pad");
			AddMatchingDevice("SL-6566 (Controller)");
			AddMatchingDevice("USB Gamepad ");
			AddMatchingDevice("WingMan RumblePad");
			AddMatchingDevice("XBOX 360 For Windows (Controller)");
			AddMatchingDevice("XEOX Gamepad (Controller)");
			AddMatchingDevice("XEQX Gamepad SL-6556-BK");
			
			lastResortDeviceRegex = "360|xbox|catz";

			ControlSetup setup = GetControlSetup(new Gamepad());

			// Setup mapping.
			setup.SplitMapping(0, CommonControls.leftStickLeft, CommonControls.leftStickRight);
			setup.SplitMapping(1, CommonControls.leftStickUp, CommonControls.leftStickDown);
			setup.Mapping(18, CommonControls.leftStickButton);

			setup.SplitMapping( 3, CommonControls.rightStickLeft, CommonControls.rightStickRight);
			setup.SplitMapping( 4, CommonControls.rightStickUp, CommonControls.rightStickDown);
			setup.Mapping(19, CommonControls.rightStickButton);

			setup.SplitMapping( 5, CommonControls.dPadLeft, CommonControls.dPadRight);
			setup.SplitMapping( 6, CommonControls.dPadDown, CommonControls.dPadUp);

			setup.Mapping(10, CommonControls.action1);
			setup.Mapping(11, CommonControls.action2);
			setup.Mapping(12, CommonControls.action3);
			setup.Mapping(13, CommonControls.action4);

			setup.SplitMapping( 2, CommonControls.rightTrigger, CommonControls.leftTrigger);
			setup.Mapping( 8, CommonControls.leftTrigger);
			setup.Mapping( 9, CommonControls.rightTrigger);

			setup.Mapping(14, CommonControls.leftBumper);
			setup.Mapping(15, CommonControls.rightBumper);

			setup.Mapping(16, CommonControls.back);
			setup.Mapping(17, CommonControls.start);

			mappings = setup.CreateMappingsArray();
		}

		public override ControlSetup GetControlSetup(InputDevice device)
		{
			ControlSetup setup = new ControlSetup(device);

			setup.AddControl(CommonControls.back);
			setup.AddControl(CommonControls.start);

			// Section for control name overrides.
			setup.GetControl(CommonControls.action1).name = "A";
			setup.GetControl(CommonControls.action2).name = "B";
			setup.GetControl(CommonControls.action3).name = "X";
			setup.GetControl(CommonControls.action4).name = "Y";

			return setup;
		}
	}
}

#endif
