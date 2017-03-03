using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	////REVIEW: the naming of this is confusing with InputControl
	public enum PointerControl
	{
		Position,
		PositionX,
		PositionY,
		PositionZ,

		Delta,
		DeltaX,
		DeltaY,
		DeltaZ,

		Pressure,
		Tilt,
		Rotation,
		Radius,
		Distance,

		LeftButton,
		RightButton,
		MiddleButton,

		ScrollWheel,
		ScrollWheelX,
		ScrollWheelY,
		ScrollWheelZ,

		ForwardButton,
		BackButton,
	}

	public static class PointerControlConstants
	{
		public const int ControlCount = (int)PointerControl.BackButton + 1;
	}
}
