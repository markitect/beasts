using System;
using System.Collections.Generic;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
	public interface IJoystickControlMapping
	{
		bool Remap(GenericControlEvent controlEvent);
	}

	[Serializable]
	public class JoystickControlMapping : IJoystickControlMapping
	{
		public int targetIndex;
		public Range fromRange;
		public Range toRange;

		public JoystickControlMapping() { }

		public JoystickControlMapping(int targetIndex)
		{
			this.targetIndex = targetIndex;
			fromRange = Range.full;
			toRange = Range.full;
		}

		public JoystickControlMapping(int targetIndex, Range fromRange, Range toRange)
		{
			this.targetIndex = targetIndex;
			this.fromRange = fromRange;
			this.toRange = toRange;
		}

		public bool Remap(GenericControlEvent controlEvent)
		{
			if (targetIndex == -1)
				return false;
			
			controlEvent.controlIndex = targetIndex;

			var floatEvent = controlEvent as GenericControlEvent<float>;
			if (floatEvent == null)
				return false;
			
			floatEvent.value = Mathf.InverseLerp(fromRange.min, fromRange.max, floatEvent.value);
			floatEvent.value = Mathf.Lerp(toRange.min, toRange.max, floatEvent.value);
			return false;
		}
	}

	[Serializable]
	public class JoystickControlSplitMapping : IJoystickControlMapping
	{
		public int targetIndexNeg;
		public Range fromRangeNeg;

		public int targetIndexPos;
		public Range fromRangePos;

		public JoystickControlSplitMapping(int targetIndexNeg, int targetIndexPos)
		{
			this.targetIndexNeg = targetIndexNeg;
			this.targetIndexPos = targetIndexPos;
			fromRangeNeg = Range.negative;
			fromRangePos = Range.positive;
		}

		public JoystickControlSplitMapping(int targetIndexNeg, Range fromRangeNeg, int targetIndexPos, Range fromRangePos)
		{
			this.targetIndexNeg = targetIndexNeg;
			this.fromRangeNeg = fromRangeNeg;
			this.targetIndexPos = targetIndexPos;
			this.fromRangePos = fromRangePos;
		}

		public bool Remap(GenericControlEvent controlEvent)
		{
			var floatEvent = controlEvent as GenericControlEvent<float>;
			if (floatEvent == null)
				return false;

			GenericControlEvent<float> eventNeg = new GenericControlEvent<float>()
			{
				device = floatEvent.device,
				controlIndex = targetIndexNeg,
				value = floatEvent.value,
				time = floatEvent.time
			};
			eventNeg.value = Mathf.InverseLerp(fromRangeNeg.min, fromRangeNeg.max, eventNeg.value);
			eventNeg.alreadyRemapped = true;
			InputSystem.ExecuteEvent(eventNeg);

			GenericControlEvent<float> eventPos = new GenericControlEvent<float>()
			{
				device = floatEvent.device,
				controlIndex = targetIndexPos,
				value = floatEvent.value,
				time = floatEvent.time
			};
			eventPos.value = Mathf.InverseLerp(fromRangePos.min, fromRangePos.max, eventPos.value);
			eventPos.alreadyRemapped = true;
			InputSystem.ExecuteEvent(eventPos);

			return true;
		}
	}
}
