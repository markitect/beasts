using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class Grab : MonoBehaviour
{
	private GameObject highlightedObject;

	// Use this for initialization
	void Start ()
	{
		var names = Input.GetJoystickNames();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.highlightedObject != null && Input.GetButton("Fire1"))
		{
			this.highlightedObject.transform.parent = this.transform;
			this.highlightedObject.GetComponent<Rigidbody>().isKinematic = true;
		}

		if (this.highlightedObject != null && Input.GetButtonUp("Fire1"))
		{
			this.highlightedObject.transform.parent = null;
			this.highlightedObject.GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log("Trigger enter!");
		other.gameObject.GetComponent<Renderer>().material.color = Color.red;
		this.highlightedObject = other.gameObject;
	}

	void OnTriggerStay(Collider other)
	{
		Debug.Log("Trigger stay!");
		other.gameObject.GetComponent<Renderer>().material.color = Color.red;
		this.highlightedObject = other.gameObject;
	}

	void OnTriggerExit(Collider other)
	{
		Debug.Log("Trigger exit!");
		other.gameObject.GetComponent<Renderer>().material.color = Color.white;
		this.highlightedObject = null;
	}
}
