using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public abstract class OutputControl<T> : InputControl, IValueProvider<T>
	{
		protected T m_Value;
		public virtual T value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = value;
			}
		}

		private T m_DefaultValue;
		public T defaultValue { get { return m_DefaultValue; } }

		public override object valueObject { get { return value; } }
		public override bool changedValue { get { return false; } }
		public override bool isDefaultValue { get { return value.Equals(m_DefaultValue); } }

		public override void AdvanceFrame() { }

		public override void Reset()
		{
			m_Value = m_DefaultValue;
		}

		public override void CopyValueFromControl(object control)
		{
			m_Value = ((IValueProvider<T>)control).value;
		}

		public override object Clone()
		{
			var clone = (OutputControl<T>)Activator.CreateInstance(GetType());
			clone.m_Value = m_Value;
			clone.m_DefaultValue = m_DefaultValue;
			clone.m_Enabled = m_Enabled;
			clone.provider = provider;
			clone.index = index;
			clone.name = name;
			return clone;
		}
	}
}
