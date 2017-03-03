namespace UnityEngine.Experimental.Input
{
	public class InputEvent
	{
		public double time { get; set; }
		public InputDevice device { get; set; }

		public virtual void Reset()
		{
			time = 0;
			device = null;
		}
	}
}
