using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class TouchEvent : InputEvent
	{
		public Touch touch;

		public override void Reset ()
		{
			base.Reset ();
			touch = default(Touch);
		}

		public override string ToString()
		{
			return string.Format("{0} finger={1} phase={2} pos={3} delta={4}",
				base.ToString(), touch.fingerId, touch.phase, touch.position, touch.delta);
		}
	}
}
