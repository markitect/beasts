using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public abstract class InputControlProvider
	{
		private InputState m_State;
		public InputState state { get { return m_State; } }

		protected void SetControls(List<InputControl> controls)
		{
			m_State = new InputState(this, controls);
		}

		public virtual bool ProcessEvent(InputEvent inputEvent)
		{
			lastEventTime = inputEvent.time;
			return false;
		}
		
		public int controlCount
		{
			get { return m_State.controls.Count; }
		}

		public IEnumerable<InputControl> controls
		{
			get
			{
				Debug.Assert(m_State != null);
				return m_State.controls;
			}
		}
		
		public InputControl this[int index]
		{
			get { return m_State.controls[index]; }
		}
		
		public InputControl this[string controlName]
		{
			get
			{
				for (var i = 0; i < m_State.controls.Count; ++ i)
				{
					if (m_State.controls[i].name == controlName)
						return m_State.controls[i];
				}
				
				throw new KeyNotFoundException(controlName);
			}
		}

		public virtual string GetSourceName(int controlIndex)
		{
			return this[controlIndex].name;
		}

		public InputControl GetControlFromHash(int hash)
		{
			int index = GetControlIndexFromHash(hash);
			if (hash == -1)
				return null;
			return this[index];
		}

		public abstract int GetControlIndexFromHash(int hash);

		public abstract int GetHashForControlIndex(int controlIndex);

		public double lastEventTime { get; protected set; }
	}
}
