using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	////REVIEW: after the refactor, InputState now really is an InputControlProvider itself
	public class InputState
	{
		private List<InputControl> m_Controls = new List<InputControl>();
		public  List<InputControl> controls { get { return m_Controls; } }

		public InputState(InputControlProvider controlProvider, List<InputControl> controls)
			: this(controlProvider, controls, null) { }

		public InputState(InputControlProvider controlProvider, List<InputControl> controls, List<int> usedControlIndices)
		{
			this.controlProvider = controlProvider;
			m_Controls = controls;
			for (var i = 0; i < m_Controls.Count; i++)
			{
				if (m_Controls[i] != null)
				{
					m_Controls[i].index = i;
					m_Controls[i].provider = controlProvider;
				}
			}
			SetUsedControls(usedControlIndices);
		}
		
		public void SetUsedControls(List<int> usedControlIndices)
		{
			if (usedControlIndices == null)
			{
				SetAllControlsEnabled(true);
			}
			else
			{
				SetAllControlsEnabled(false);
				for (var i = 0; i < usedControlIndices.Count; i++)
					if (m_Controls[usedControlIndices[i]] != null)
						m_Controls[usedControlIndices[i]].enabled = true;
			}
		}

		public bool SetCurrentValue<T>(int index, T value)
		{
			if (index < 0 || index >= m_Controls.Count)
				throw new ArgumentOutOfRangeException("index",
					string.Format("Control index {0} is out of range; state has {1} entries", index, m_Controls.Count));

			if (!controls[index].enabled)
				return false;

			var control = m_Controls[index] as InputControl<T>;
			if (control == null)
				throw new Exception(string.Format(
					"Control index {0} is of type {1} but was attempted to be set with value type {2}.",
					index, m_Controls[index].GetType().Name, value.GetType().Name
				));

			control.SetValue(value);
			return true;
		}

		public void SetAllControlsEnabled(bool enabled)
		{
			for (var i = 0; i < m_Controls.Count; ++ i)
			{
				if (m_Controls[i] != null)
					m_Controls[i].enabled = enabled;
			}
		}

		public void InitToDevice()
		{
			if (controlProvider.state == this)
				return;
			
			List<InputControl> referenceControls = controlProvider.state.controls;
			for (int i = 0; i < m_Controls.Count; i++)
			{
				if (m_Controls[i] == null)
					continue;
				if (m_Controls[i].enabled)
					m_Controls[i].CopyValueFromControl(referenceControls[i]);
				else
					m_Controls[i].Reset();
			}
		}
		
		public void Reset()
		{
			for (int i = 0; i < m_Controls.Count; i++)
				if (m_Controls[i] != null)
					m_Controls[i].Reset();
		}

		internal void BeginUpdate()
		{
			var stateCount = m_Controls.Count;
			for (var index = 0; index < stateCount; ++index)
			{
				if (m_Controls[index] == null || !m_Controls[index].enabled)
					continue;

				if (InputSystem.listeningForBinding)
				{
					if (m_Controls[index].changedValue && !m_Controls[index].isDefaultValue)
						InputSystem.RegisterBinding(controlProvider[index]);
				}
				
				m_Controls[index].AdvanceFrame();
			}
		}

		public InputControlProvider controlProvider { get; set; }

		public int count
		{
			get { return m_Controls.Count; }
		}
	}
}
