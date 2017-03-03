using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class AxisAction : ActionSlot<AxisControl> {}
	public class AxisControl : InputControl<float>
	{
		public AxisControl() {}
		public AxisControl(string name)
		{
			this.name = name;
		}

		public override float GetCombinedValue(System.Collections.Generic.IEnumerable<float> values)
		{
			float value = 0;
			foreach (var current in values)
				if (Mathf.Abs(current) > Mathf.Abs(value))
					value = current;
			return value;
		}
	}
}
