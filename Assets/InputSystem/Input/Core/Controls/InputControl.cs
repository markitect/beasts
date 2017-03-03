using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public abstract class InputControl : ICloneable
	{
		protected bool m_Enabled;
		public bool enabled { get { return m_Enabled; } set { m_Enabled = value; } }

		public InputControlProvider provider { get; internal set; }
		public int index { get; internal set; }
		// For an InputControl in an ActionMapInput the name is the action name
		// and the source name is the name of the controls it's bound to.
		public string name { get; internal set; }
		public string sourceName { get { return provider.GetSourceName(index); } }

		public abstract object valueObject { get; }
		public abstract bool changedValue { get; }
		public abstract bool isDefaultValue { get; }

		public abstract void Reset();
		public abstract void CopyValueFromControl(object control);
		public abstract void AdvanceFrame();
		public abstract object Clone();
	}

	public abstract class InputControl<T> : InputControl, IValueProvider<T>
	{
		protected T m_ValueDynamic;
		protected T m_ValueFixed;
		public T value
		{
			get
			{
				return Time.inFixedTimeStep ? m_ValueFixed : m_ValueDynamic;
			}
			set
			{
				if (Time.inFixedTimeStep)
					m_ValueFixed = value;
				else
					m_ValueDynamic = value;
			}
		}

		private T m_PreviousValueDynamic;
		private T m_PreviousValueFixed;
		public T previousValue
		{
			get { return Time.inFixedTimeStep ? m_PreviousValueFixed : m_PreviousValueDynamic; }
		}

		private T m_DefaultValue;
		public T defaultValue { get { return m_DefaultValue; } }

		public override object valueObject { get { return value; } }
		public override bool changedValue { get { return !value.Equals(previousValue); } }
		public override bool isDefaultValue { get { return value.Equals(m_DefaultValue); } }

		public virtual void SetValue(T newValue)
		{
			m_ValueDynamic = newValue;
			m_ValueFixed = newValue;
		}

		public override void AdvanceFrame()
		{
			if (Time.inFixedTimeStep)
				m_PreviousValueFixed = m_ValueFixed;
			else
				m_PreviousValueDynamic = m_ValueDynamic;
		}

		public override void Reset()
		{
			m_ValueFixed = m_DefaultValue;
			m_ValueDynamic = m_DefaultValue;
			m_PreviousValueFixed = m_DefaultValue;
			m_PreviousValueDynamic = m_DefaultValue;
		}

		public override void CopyValueFromControl(object control)
		{
			m_ValueFixed = ((IValueProvider<T>)control).value;
			m_ValueDynamic = m_ValueFixed;
		}

		public override object Clone()
		{
			var clone = (InputControl<T>)Activator.CreateInstance(GetType());
			clone.m_ValueFixed = m_ValueFixed;
			clone.m_ValueDynamic = m_ValueDynamic;
			clone.m_PreviousValueFixed = m_PreviousValueFixed;
			clone.m_PreviousValueDynamic = m_PreviousValueDynamic;
			clone.m_DefaultValue = m_DefaultValue;
			clone.m_Enabled = m_Enabled;
			clone.provider = provider;
			clone.index = index;
			clone.name = name;
			return clone;
		}

		public virtual T GetCombinedValue(IEnumerable<T> values)
		{
			foreach (var value in values)
				return value;
			return defaultValue;
		}
	}
}
