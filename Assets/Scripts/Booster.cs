using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour {

	float startY;
	public float hoverHeight = 3.5f;
	public float hoverForce = 20f;

	Rigidbody ship;

	// Use this for initialization
	void Start () {
		this.startY = this.transform.position.y;
		this.ship = GetComponentInParent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		var ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, hoverHeight))
		{
			var proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
			Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
			ship.AddForceAtPosition(appliedHoverForce, this.ship.ClosestPointOnBounds(this.transform.position), ForceMode.Acceleration);
		}
	}
}
