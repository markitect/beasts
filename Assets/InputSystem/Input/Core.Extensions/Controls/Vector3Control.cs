using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class Vector3Action : ActionSlot<Vector3Control> {}
	public class Vector3Control : InputControl<Vector3>
	{
		public Vector3Control() {}
		public Vector3Control(string name)
		{
			this.name = name;
		}

		public override Vector3 GetCombinedValue(System.Collections.Generic.IEnumerable<Vector3> values)
		{
			Vector3 value = Vector3.zero;
			foreach (var current in values)
				if (current.sqrMagnitude > value.sqrMagnitude)
					value = current;
			return value;
		}
	}
}
