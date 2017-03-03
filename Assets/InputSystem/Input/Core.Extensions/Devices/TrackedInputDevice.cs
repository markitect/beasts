
namespace UnityEngine.Experimental.Input
{
	public class TrackedInputDevice : InputDevice
	{
		public TrackedInputDevice(string displayName)
			: base(displayName)
		{
		}

		public override void AddStandardControls(ControlSetup setup)
		{
			position = (Vector3Control)setup.AddControl(CommonControls.position3d);
			rotation = (QuaternionControl)setup.AddControl(CommonControls.rotation3d);
			pose = (PoseControl)setup.AddControl(CommonControls.pose);
		}

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			
			var consumed = false;

			var trackingEvent = inputEvent as TrackingEvent;
			if (trackingEvent != null && trackingEvent.nodeId == 0)
			{
				Pose localPose = new Pose();
				localPose.rotation = trackingEvent.localRotation;
				localPose.translation = trackingEvent.localPosition;

				consumed |= intoState.SetCurrentValue(position.index, trackingEvent.localPosition);
				consumed |= intoState.SetCurrentValue(rotation.index, trackingEvent.localRotation);
				consumed |= intoState.SetCurrentValue(pose.index, localPose);
			}

			if (!consumed && inputEvent.GetType().IsAssignableFrom(typeof(GenericControlEvent)))
				consumed = base.ProcessEventIntoState(inputEvent, intoState);

			return consumed;
		}

		public Vector3Control position { get; private set; }
		public QuaternionControl rotation { get; private set; }

		public PoseControl pose { get; private set; }
	}
}

