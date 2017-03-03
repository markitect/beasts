#if UNITY_EDITOR
using UnityEditor;
#endif
using Assets.Utilities;

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

namespace UnityEngine.Experimental.Input
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class Xbox360MacProfile : GamepadProfile
	{
		static Xbox360MacProfile()
		{
			Register();
		}

		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Register()
		{
			InputSystem.RegisterDeviceProfile<Xbox360MacProfile>();
		}

		public Xbox360MacProfile()
		{
			displayName = "Xbox 360 Controller";
			matchingDeviceRegexes = new[]
			{
				"Microsoft.+360 Controller",
				"Mad Catz, Inc. Mad Catz FPS Pro GamePad",
				"Microsoft Corporation Controller",
				"Microsoft.+Xbox.+Controller", ////REVIEW: this will match Xbox One controllers, too
			};

			lastResortDeviceRegex = "360.+Gamepad"; ////REVIEW: or just "gamepad"?

			ControlSetup setup = GetControlSetup(new Gamepad());

			// Based on "Xbox 360 Controller Driver" from https://github.com/360Controller/360Controller/.
			// X and Y labeling on the controller are inverted with "X" meaning vertical and "Y" meaning horizontal.

			// Setup mapping.
			setup.SplitMapping(3, CommonControls.leftStickLeft, CommonControls.leftStickRight);
			setup.SplitMapping(4, CommonControls.leftStickUp, CommonControls.leftStickDown);
			setup.Mapping(19, CommonControls.leftStickButton);

			setup.SplitMapping( 6, CommonControls.rightStickLeft, CommonControls.rightStickRight);
			setup.SplitMapping( 7, CommonControls.rightStickUp, CommonControls.rightStickDown);
			setup.Mapping(20, CommonControls.rightStickButton);

			setup.Mapping(15, CommonControls.dPadLeft);
			setup.Mapping(16, CommonControls.dPadRight);
			setup.Mapping(14, CommonControls.dPadDown);
			setup.Mapping(13, CommonControls.dPadUp);

			setup.Mapping(24, CommonControls.action1);
			setup.Mapping(25, CommonControls.action2);
			setup.Mapping(26, CommonControls.action3);
			setup.Mapping(27, CommonControls.action4);

			setup.Mapping(28, CommonControls.leftTrigger, Range.full, Range.positive);
			setup.Mapping(29, CommonControls.rightTrigger, Range.full, Range.positive);

			setup.Mapping(21, CommonControls.leftBumper);
			setup.Mapping(22, CommonControls.rightBumper);

			setup.Mapping(18, CommonControls.back);
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
