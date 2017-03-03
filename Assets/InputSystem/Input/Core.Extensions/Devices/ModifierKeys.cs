using System;

namespace UnityEngine.Experimental.Input
{
	[Flags]
	public enum ModifierKeys
	{
		LeftShift = 1 << 0,
		RightShift = 1 << 1,
		LeftAlt = 1 << 2,
		RightAlt = 1 << 3,
		LeftControl = 1 << 4,
		RightControl = 1 << 5,

		LeftCommand = 1 << 6,
		RightCommand = 1 << 7,
		LeftMeta = 1 << 6,
		RightMeta = 1 << 7,
		LeftWindows = 1 << 6,
		RightWindows = 1 << 7,

		Shift = LeftShift | RightShift,
		Alt = LeftAlt | RightAlt,
		Control = LeftControl | RightControl,
		Command = LeftCommand | RightCommand,
		Meta = LeftMeta | RightMeta,
		Windows = LeftWindows | RightWindows
	}
}

