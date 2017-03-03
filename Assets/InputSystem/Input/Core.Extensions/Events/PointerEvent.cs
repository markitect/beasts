using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class PointerEvent : InputEvent
	{
		public int pointerId { get; set; }
		public Vector3 position { get; set; }
		public float pressure { get; set; }
		public float tilt { get; set; }
		public float rotation { get; set; }
		public Vector3 radius { get; set; }
		public float distance { get; set; }
		public int displayIndex { get; set; }

		public override void Reset ()
		{
			base.Reset ();
			pointerId = 0;
			position = default(Vector3);
			pressure = 0;
			tilt = 0;
			rotation = 0;
			radius = default(Vector3);
			distance = 0;
			displayIndex = 0;
		}

		public override string ToString()
		{
			return string.Format("({0}, pos:{1})", base.ToString(), position);
		}
	}
}
