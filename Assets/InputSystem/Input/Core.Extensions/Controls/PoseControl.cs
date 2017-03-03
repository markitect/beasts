using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class PoseAction : ActionSlot<PoseControl> {}
	public class PoseControl : InputControl<Pose>
	{
		public PoseControl() {}
		public PoseControl(string name)
		{
			this.name = name;
		}
	}
}
