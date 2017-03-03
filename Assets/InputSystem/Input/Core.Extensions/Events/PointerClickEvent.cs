using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class PointerClickEvent : PointerEvent
	{
		public int clickCount { get; set; }

		public override void Reset ()
		{
			base.Reset ();
			clickCount = 0;
		}
	}
}
