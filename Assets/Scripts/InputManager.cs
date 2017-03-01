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


	// Use this for initialization
	void Start () {
		InputTracking.Recenter();
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.head.transform.localPosition = InputTracking.GetLocalPosition(VRNode.Head);
		this.head.transform.localRotation = InputTracking.GetLocalRotation(VRNode.Head);
		this.left.transform.localPosition = InputTracking.GetLocalPosition(VRNode.LeftHand);
		this.left.transform.localRotation = InputTracking.GetLocalRotation(VRNode.LeftHand);
		this.right.transform.localPosition = InputTracking.GetLocalPosition(VRNode.RightHand);
		this.right.transform.localRotation = InputTracking.GetLocalRotation(VRNode.RightHand);
	}
}
