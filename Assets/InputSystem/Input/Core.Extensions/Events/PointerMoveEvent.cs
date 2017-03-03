using System;
using System.Collections.Generic;
using UnityEngine;

////TODO: PointerClickEvent (important as unlike GenericControlEvent it tells the position of the click)

namespace UnityEngine.Experimental.Input
{
	public class PointerMoveEvent : PointerEvent
	{
		public Vector3 delta { get; set; }

		public override void Reset ()
		{
			base.Reset ();
			delta = default(Vector3);
		}
	}
}
