using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	public class AxisOututAction : ActionSlot<AxisOutput> {}
	public class AxisOutput : OutputControl<float>
	{
		private Action<float> m_Action;

		public AxisOutput() {}
		public AxisOutput(string name, Action<float> action)
		{
			this.name = name;
			m_Action = action;
		}

		public override float value
		{
			set
			{
				if (value == base.value)
					return;
				base.value = value;
				m_Action.Invoke(base.value);
			}
		}
	}
}
