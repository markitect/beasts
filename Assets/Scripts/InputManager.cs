using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR;

public class InputManager : MonoBehaviour
{

	private Snappable selectedObject;

	public GameObject head;
	public GameObject left;
	public GameObject right;

	public bool legacyTracking;

	private List<VRNodeState> nodeStates = new List<VRNodeState>();
	private Dictionary<ulong, string> nodeNames = new Dictionary<ulong, string>();


	// Use this for initialization
	void Start () {
		//InputTracking.Recenter();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (this.legacyTracking)
		{
			InputTracking.GetNodeStates(nodeStates);

			foreach (var nodeState in nodeStates)
			{
				var name = string.Empty;
				nodeNames.TryGetValue(nodeState.uniqueID, out name);
				Debug.LogError(nodeState.nodeType + " : " + name);
				Vector3 position;
				Quaternion rotation;
				Vector3 velocity;
				Quaternion angularVelocity;
				Vector3 acceleration;
				Quaternion angularAcceleration;

				if (nodeState.TryGetPosition(out position))
				{
					Debug.LogError("Position: " + position.ToString("N4"));
				}
				if (nodeState.TryGetRotation(out rotation))
				{
					Debug.LogError("Orientation: " + rotation.ToString("N4"));
				}
				if (nodeState.TryGetVelocity(out velocity))
				{
					Debug.LogError("Velocity: " + velocity.ToString("N4"));
				}
				if (nodeState.TryGetAngularVelocity(out angularVelocity))
				{
					Debug.LogError("Angular Velocity: " + angularVelocity.ToString("N4"));
				}
				if (nodeState.TryGetAcceleration(out acceleration))
				{
					Debug.LogError("Acceleration: " + acceleration.ToString("N4"));
				}
				if (nodeState.TryGetAngularAcceleration(out angularAcceleration))
				{
					Debug.LogError("Angular Acceleration: " + angularAcceleration.ToString("N4"));
				}
			}

			this.head.transform.localPosition = InputTracking.GetLocalPosition(VRNode.Head) - (Vector3.up * 1f);
			this.head.transform.localRotation = InputTracking.GetLocalRotation(VRNode.Head);
			this.left.transform.localPosition = InputTracking.GetLocalPosition(VRNode.LeftHand);// - (Vector3.up * .5f); ;
			this.left.transform.localRotation = InputTracking.GetLocalRotation(VRNode.LeftHand);
			this.right.transform.localPosition = InputTracking.GetLocalPosition(VRNode.RightHand);// - (Vector3.up * .5f); ;
			this.right.transform.localRotation = InputTracking.GetLocalRotation(VRNode.RightHand);
		}
	}
}
