namespace UnityEngine.Experimental.Input
{
	public static class CommonControls
	{
		public static readonly SupportedControl leftStickLeft = SupportedControl.Get<ButtonControl>("Left Stick Left");
		public static readonly SupportedControl leftStickRight = SupportedControl.Get<ButtonControl>("Left Stick Right");
		public static readonly SupportedControl leftStickDown = SupportedControl.Get<ButtonControl>("Left Stick Down");
		public static readonly SupportedControl leftStickUp = SupportedControl.Get<ButtonControl>("Left Stick Up");
		public static readonly SupportedControl leftStickX = SupportedControl.Get<AxisControl>("Left Stick X");
		public static readonly SupportedControl leftStickY = SupportedControl.Get<AxisControl>("Left Stick Y");
		public static readonly SupportedControl leftStick = SupportedControl.Get<Vector2Control>("Left Stick");
		public static readonly SupportedControl leftStickButton = SupportedControl.Get<ButtonControl>("Left Stick Button");

		public static readonly SupportedControl rightStickLeft = SupportedControl.Get<ButtonControl>("Right Stick Left");
		public static readonly SupportedControl rightStickRight = SupportedControl.Get<ButtonControl>("Right Stick Right");
		public static readonly SupportedControl rightStickDown = SupportedControl.Get<ButtonControl>("Right Stick Down");
		public static readonly SupportedControl rightStickUp = SupportedControl.Get<ButtonControl>("Right Stick Up");
		public static readonly SupportedControl rightStickX = SupportedControl.Get<AxisControl>("Right Stick X");
		public static readonly SupportedControl rightStickY = SupportedControl.Get<AxisControl>("Right Stick Y");
		public static readonly SupportedControl rightStick = SupportedControl.Get<Vector2Control>("Right Stick");
		public static readonly SupportedControl rightStickButton = SupportedControl.Get<ButtonControl>("Right Stick Button");

		public static readonly SupportedControl dPadLeft = SupportedControl.Get<ButtonControl>("D-Pad Left");
		public static readonly SupportedControl dPadRight = SupportedControl.Get<ButtonControl>("D-Pad Right");
		public static readonly SupportedControl dPadDown = SupportedControl.Get<ButtonControl>("D-Pad Down");
		public static readonly SupportedControl dPadUp = SupportedControl.Get<ButtonControl>("D-Pad Up");
		public static readonly SupportedControl dPadX = SupportedControl.Get<AxisControl>("D-Pad X");
		public static readonly SupportedControl dPadY = SupportedControl.Get<AxisControl>("D-Pad Y");
		public static readonly SupportedControl dPad = SupportedControl.Get<Vector2Control>("D-Pad");

		public static readonly SupportedControl action1 = SupportedControl.Get<ButtonControl>("Action 1");
		public static readonly SupportedControl action2 = SupportedControl.Get<ButtonControl>("Action 2");
		public static readonly SupportedControl action3 = SupportedControl.Get<ButtonControl>("Action 3");
		public static readonly SupportedControl action4 = SupportedControl.Get<ButtonControl>("Action 4");

		public static readonly SupportedControl trigger = SupportedControl.Get<ButtonControl>("Trigger");
		public static readonly SupportedControl leftTrigger = SupportedControl.Get<ButtonControl>("Left Trigger");
		public static readonly SupportedControl rightTrigger = SupportedControl.Get<ButtonControl>("Right Trigger");

		public static readonly SupportedControl leftBumper = SupportedControl.Get<ButtonControl>("Left Bumper");
		public static readonly SupportedControl rightBumper = SupportedControl.Get<ButtonControl>("Right Bumper");

		public static readonly SupportedControl start = SupportedControl.Get<ButtonControl>("Start");
		public static readonly SupportedControl back = SupportedControl.Get<ButtonControl>("Back");
		public static readonly SupportedControl select = SupportedControl.Get<ButtonControl>("Select");
		public static readonly SupportedControl system = SupportedControl.Get<ButtonControl>("System");
		public static readonly SupportedControl options = SupportedControl.Get<ButtonControl>("Options");
		public static readonly SupportedControl pause = SupportedControl.Get<ButtonControl>("Pause");
		public static readonly SupportedControl menu = SupportedControl.Get<ButtonControl>("Menu");
		public static readonly SupportedControl share = SupportedControl.Get<ButtonControl>("Share");
		public static readonly SupportedControl home = SupportedControl.Get<ButtonControl>("Home");
		public static readonly SupportedControl view = SupportedControl.Get<ButtonControl>("View");
		public static readonly SupportedControl power = SupportedControl.Get<ButtonControl>("Power");

		public static readonly SupportedControl tiltX = SupportedControl.Get<AxisControl>("Tilt X");
		public static readonly SupportedControl tiltY = SupportedControl.Get<AxisControl>("Tilt Y");
		public static readonly SupportedControl tiltZ = SupportedControl.Get<AxisControl>("Tilt Z");

		public static readonly SupportedControl touchpadX = SupportedControl.Get<AxisControl>("Touchpad X");
		public static readonly SupportedControl touchpadY = SupportedControl.Get<AxisControl>("Touchpad Y");
		public static readonly SupportedControl touchpadTap = SupportedControl.Get<ButtonControl>("Touchpad Tap");

		public static readonly SupportedControl positionX = SupportedControl.Get<AxisControl>("Position X");
		public static readonly SupportedControl positionY = SupportedControl.Get<AxisControl>("Position Y");
		public static readonly SupportedControl positionZ = SupportedControl.Get<AxisControl>("Position Z");
		public static readonly SupportedControl position3d = SupportedControl.Get<Vector3Control>("Position");

		public static readonly SupportedControl rotation3d = SupportedControl.Get<QuaternionControl>("Rotation");

		public static readonly SupportedControl pose = SupportedControl.Get<PoseControl>("Pose");

		public static readonly SupportedControl deltaX = SupportedControl.Get<DeltaAxisControl>("Delta X");
		public static readonly SupportedControl deltaY = SupportedControl.Get<DeltaAxisControl>("Delta Y");
		public static readonly SupportedControl deltaZ = SupportedControl.Get<DeltaAxisControl>("Delta Z");
		public static readonly SupportedControl delta3d = SupportedControl.Get<Vector3Control>("Delta");

		public static readonly SupportedControl scrollWheelX = SupportedControl.Get<DeltaAxisControl>("Scroll Wheel X");
		public static readonly SupportedControl scrollWheelY = SupportedControl.Get<DeltaAxisControl>("Scroll Wheel Y");
		public static readonly SupportedControl scrollWheelZ = SupportedControl.Get<DeltaAxisControl>("Scroll Wheel Z");
		public static readonly SupportedControl scrollWheel3d = SupportedControl.Get<Vector3Control>("Scroll Wheel");

		public static readonly SupportedControl pressure = SupportedControl.Get<AxisControl>("Pressure");
		public static readonly SupportedControl tilt = SupportedControl.Get<AxisControl>("Tilt");
		public static readonly SupportedControl rotation = SupportedControl.Get<AxisControl>("Rotation");
		public static readonly SupportedControl radius = SupportedControl.Get<AxisControl>("Radius");
		public static readonly SupportedControl distance = SupportedControl.Get<AxisControl>("Distance");

		public static readonly SupportedControl leftButton = SupportedControl.Get<ButtonControl>("Left Button");
		public static readonly SupportedControl rightButton = SupportedControl.Get<ButtonControl>("Right Button");
		public static readonly SupportedControl middleButton = SupportedControl.Get<ButtonControl>("Middle Button");

		public static readonly SupportedControl forwardButton = SupportedControl.Get<ButtonControl>("Forward Button");
		public static readonly SupportedControl backButton = SupportedControl.Get<ButtonControl>("Back Button");
	}
}

