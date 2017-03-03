using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class KeyEvent : InputEvent
	{
		public KeyCode rawKey { get; set; }
		public KeyCode localizedKey { get; set; }
		public bool isPress { get; private set; }
		public bool isRelease { get; private set; }
		public bool isRepeat { get; private set; }

		public override void Reset ()
		{
			base.Reset ();
			rawKey = default(KeyCode);
			localizedKey = default(KeyCode);
			isPress = false;
			isRelease = false;
			isRepeat = false;
		}
	}
}
