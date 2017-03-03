using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Input
{
	public class HeadMountedDisplay : TrackedInputDevice
	{
		enum Control
		{
			// Same indices as Position and Rotation in TrackedInputDevice.
			// these are standard controls
			HeadPosition,
			HeadRotation,
			HeadPose,

			LeftEyePosition,
			LeftEyeRotation,
			RightEyePosition,
			RightEyeRotation,
			CenterEyePosition,
			CenterEyeRotation,

			LeftEyePose,
			RightEyePose,
			CenterEyePose,
		};

		internal enum Node
		{			
			LeftEye = 1,
			RightEye = 2,
			CenterEye = 3,
			Head = 6,
			LeftHand = 4,
			RightHand = 5,
		}

		public HeadMountedDisplay()
			: this("HMD") {}

		public HeadMountedDisplay(string displayName)
			: base(displayName)
		{
		}

		public override void AddStandardControls(ControlSetup setup)
		{
			base.AddStandardControls(setup);

			setup.AddControl(SupportedControl.Get<Vector3Control>("Left Eye Position"));
			setup.AddControl(SupportedControl.Get<QuaternionControl>("Left Eye Rotation"));
			setup.AddControl(SupportedControl.Get<Vector3Control>("Right Eye Position"));
			setup.AddControl(SupportedControl.Get<QuaternionControl>("Right Eye Rotation"));
			setup.AddControl(SupportedControl.Get<Vector3Control>("Center Eye Position"));
			setup.AddControl(SupportedControl.Get<QuaternionControl>("Center Eye Rotation"));

			setup.AddControl(SupportedControl.Get<PoseControl>("Left Eye Pose"));
			setup.AddControl(SupportedControl.Get<PoseControl>("Right Eye Pose"));
			setup.AddControl(SupportedControl.Get<PoseControl>("Center Eye Pose"));

		}

		public static HeadMountedDisplay current { get { return InputSystem.GetCurrentDeviceOfType<HeadMountedDisplay>(); } }

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{		
			var consumed = false;

			var trackingEvent = inputEvent as TrackingEvent;
			if (trackingEvent != null)
			{

				Pose pose = new Pose();
				pose.rotation = trackingEvent.localRotation;
				pose.translation = trackingEvent.localPosition;

				switch (trackingEvent.nodeId)
				{
					// Head is handled by the base class.
					case (int)Node.Head:
						consumed |= intoState.SetCurrentValue((int)Control.HeadPosition, trackingEvent.localPosition);
						consumed |= intoState.SetCurrentValue((int)Control.HeadRotation, trackingEvent.localRotation);
						consumed |= intoState.SetCurrentValue((int)Control.HeadPose, pose);
						break;
					case (int)Node.CenterEye:
						consumed |= intoState.SetCurrentValue((int)Control.CenterEyePosition, trackingEvent.localPosition);
						consumed |= intoState.SetCurrentValue((int)Control.CenterEyeRotation, trackingEvent.localRotation);
						consumed |= intoState.SetCurrentValue((int)Control.CenterEyePose, pose);
						break;
					case (int)Node.LeftEye:
						consumed |= intoState.SetCurrentValue((int)Control.LeftEyePosition, trackingEvent.localPosition);
						consumed |= intoState.SetCurrentValue((int)Control.LeftEyeRotation, trackingEvent.localRotation);
						consumed |= intoState.SetCurrentValue((int)Control.LeftEyePose, pose);
						break;
					case (int)Node.RightEye:
						consumed |= intoState.SetCurrentValue((int)Control.RightEyePosition, trackingEvent.localPosition);
						consumed |= intoState.SetCurrentValue((int)Control.RightEyeRotation, trackingEvent.localRotation);
						consumed |= intoState.SetCurrentValue((int)Control.RightEyePose, pose);
						break;
				}
			}

			if (!consumed)
				consumed = base.ProcessEventIntoState(inputEvent, intoState);

			return consumed;
		}

		public Vector3Control headPosition
		{
			get { return position; }
		}

		public QuaternionControl headRotation
		{
			get { return rotation; }
		}

		public Vector3Control leftEyePosition
		{
			get { return (Vector3Control)this[(int)Control.LeftEyePosition]; }
		}

		public QuaternionControl leftEyeRotation
		{
			get { return (QuaternionControl)this[(int)Control.LeftEyeRotation]; }
		}

		public Vector3Control rightEyePosition
		{
			get { return (Vector3Control)this[(int)Control.RightEyePosition]; }
		}

		public QuaternionControl rightEyeRotation
		{
			get { return (QuaternionControl)this[(int)Control.RightEyeRotation]; }
		}

		public Vector3Control centerEyePosition
		{
			get { return (Vector3Control)this[(int)Control.CenterEyePosition]; }
		}

		public QuaternionControl centerEyeRotation
		{
			get { return (QuaternionControl)this[(int)Control.CenterEyeRotation]; }
		}

		public PoseControl centerEyePose
		{
			get { return (PoseControl)this[(int)Control.CenterEyePose]; }
		}

		public PoseControl leftEyePose
		{
			get { return (PoseControl)this[(int)Control.LeftEyePose]; }
		}

		public PoseControl rightEyePose
		{
			get { return (PoseControl)this[(int)Control.RightEyePose]; }
		}

		public PoseControl headPose
		{
			get { return (PoseControl)this[(int)Control.HeadPose]; }
		}
	}
}

