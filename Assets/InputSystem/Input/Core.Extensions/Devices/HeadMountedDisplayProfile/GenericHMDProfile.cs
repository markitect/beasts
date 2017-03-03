#if UNITY_EDITOR
using UnityEditor;
#endif
using Assets.Utilities;
using UnityEngine.VR;

namespace UnityEngine.Experimental.Input
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class GenericHMDProfile : InputDeviceProfile
	{
		static GenericHMDProfile()
		{
			Register();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Register()
		{
			InputSystem.RegisterDeviceProfile<GenericHMDProfile>();
		}

		public GenericHMDProfile()
		{
			displayName = "HMD";
			lastResortDeviceRegex = "HMD";

			GetControlSetup(new HeadMountedDisplay());
		}

		public override InputDevice TryCreateDevice(string deviceString)
		{
			return new HeadMountedDisplay(deviceString);
		}

		public override bool Remap(InputEvent inputEvent)
		{
			var trackingEvent = inputEvent as TrackingEvent;
			if (trackingEvent != null)
			{
				switch (trackingEvent.nodeId)
				{
					case (int)VRNode.LeftEye: trackingEvent.nodeId = (int)HeadMountedDisplay.Node.LeftEye; break;
					case (int)VRNode.RightEye: trackingEvent.nodeId = (int)HeadMountedDisplay.Node.RightEye; break;
					case (int)VRNode.CenterEye: trackingEvent.nodeId = (int)HeadMountedDisplay.Node.CenterEye; break;
					case (int)VRNode.Head: trackingEvent.nodeId = (int)HeadMountedDisplay.Node.Head; break;

					////REVIEW: This is probably not the best approach. The VR system combines all tracking nodes into a single
					////        list regardless of which device they are coming from. Means we have to split at some point and route
					////        to different devices. Either in native or in managed. I went with managed for now as it can more
					////        flexibly deal with the situation.
					case (int)VRNode.LeftHand:
						trackingEvent.device = TrackedController.leftHand;
						trackingEvent.nodeId = 0;
						break;
					case (int)VRNode.RightHand:
						trackingEvent.device = TrackedController.rightHand;
						trackingEvent.nodeId = 0;
						break;
				}
			}
			return false;
		}
	}
}

