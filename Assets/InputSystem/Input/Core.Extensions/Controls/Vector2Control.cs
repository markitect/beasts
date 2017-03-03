using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class Vector2Action : ActionSlot<Vector2Control> {}
	public class Vector2Control : InputControl<Vector2>
	{
		public Vector2Control() {}
		public Vector2Control(string name)
		{
			this.name = name;
		}

		public override Vector2 GetCombinedValue(System.Collections.Generic.IEnumerable<Vector2> values)
		{
			Vector2 value = Vector2.zero;
			foreach (var current in values)
				if (current.sqrMagnitude > value.sqrMagnitude)
					value = current;
			return value;
		}
	}
}
