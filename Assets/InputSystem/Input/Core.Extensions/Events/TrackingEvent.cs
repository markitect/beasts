using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public class TrackingEvent : InputEvent
	{
		public int nodeId { get; set; }
		public Vector3 localPosition { get; set; }
		public Quaternion localRotation { get; set; }

		public override void Reset ()
		{
			base.Reset ();
			nodeId = default(int);
			localPosition = default(Vector3);
			localRotation = default(Quaternion);
		}
	}
}

