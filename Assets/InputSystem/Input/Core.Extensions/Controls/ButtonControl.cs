using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class ButtonAction : ActionSlot<ButtonControl> {}
	public class ButtonControl : InputControl<float>
	{
		public ButtonControl() {}
		public ButtonControl(string name)
		{
			this.name = name;
		}

		public bool isHeld
		{
			get { return value > 0.5f; }
		}

		public bool wasJustPressed
		{
			get { return value > 0.5f && previousValue <= 0.5f; }
		}

		public bool wasJustReleased
		{
			get { return value <= 0.5f && previousValue > 0.5f; }
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
