using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class QuaternionAction : ActionSlot<QuaternionControl> {}
	public class QuaternionControl : InputControl<Quaternion>
	{
		public QuaternionControl() {}
		public QuaternionControl(string name)
		{
			this.name = name;
		}

		public override Quaternion GetCombinedValue(System.Collections.Generic.IEnumerable<Quaternion> values)
		{
			// Can't really combine multiple quaternion sources.
			// We'll just return first one that is not identity.
			foreach (var current in values)
				if (current != Quaternion.identity)
					return current;
			return Quaternion.identity;
		}
	}
}
