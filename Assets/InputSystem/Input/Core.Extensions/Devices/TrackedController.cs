using System.Linq;

namespace UnityEngine.Experimental.Input
{
	// A combination of gamepad-style controls with motion tracking.
	// We mandate it has at least a "fire" button in addition to tracking.
	// Example: Oculus Touch, OpenVR controllers
	public class TrackedController : TrackedInputDevice
	{
		internal enum Tag
		{
			Left,
			Right
		}

		public TrackedController()
			: this("Tracked Controller") {}
		////REVIEW: this fixed assignment probably won't be good enough; seems like we have to support controllers changing roles on the fly
		public TrackedController(string displayName, int tagIndex = -1)
			: base(displayName)
		{
			m_TagIndex = tagIndex;
		}

		public override void AddStandardControls(ControlSetup setup)
		{
			base.AddStandardControls(setup);
			trigger = (ButtonControl)setup.AddControl(CommonControls.trigger);
		}

		public ButtonControl trigger { get; private set; }

        

		[SerializeField]
		private int m_TagIndex;
		public override int tagIndex
		{
			get { return m_TagIndex; }
		}

		private static string[] s_Tags = new[] { "Left", "Right" };
		public static string[] Tags
		{
			get { return s_Tags; }
		}

		////TODO: implement speedier lookups rather than crawling through all devices looking for left and right
		public static TrackedController leftHand
		{
			get { return (TrackedController)InputSystem.devices.FirstOrDefault(d => d is TrackedController && d.tagIndex == 0); }
		}
		public static TrackedController rightHand
		{
			get { return (TrackedController)InputSystem.devices.FirstOrDefault(d => d is TrackedController && d.tagIndex == 1); }
		}
	}
}

