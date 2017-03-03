using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class Mouse : Pointer
	{
		public new static Mouse current { get { return InputSystem.GetCurrentDeviceOfType<Mouse>(); } }

		public Mouse() : this("Mouse") { }

		public Mouse(string displayName) : base(displayName)
		{
			cursor = new Cursor();
		}
	}
}
