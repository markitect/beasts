using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{

	private Snappable selectedObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				this.selectedObject = hit.transform.gameObject.GetComponent<Snappable>();
			}
		}

		if (this.selectedObject != null && Input.GetMouseButton(0))
		{
			this.selectedObject.beingDragged = true;

			var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 10f, 1 << LayerMask.NameToLayer("SnapTarget"));
			if(hits.Any())
			{
				var closestHit = FindClosest(hits);
				this.selectedObject.transform.position = closestHit.position;
			}
			else
			{
				this.selectedObject.transform.position = Camera.main.ScreenPointToRay(Input.mousePosition).origin +
				                                         (Camera.main.ScreenPointToRay(Input.mousePosition).direction*5f);
			}
		}
		else if(this.selectedObject != null)
		{
			this.selectedObject.beingDragged = false;
			this.selectedObject = null;
		}
	}

	private Transform FindClosest(RaycastHit[] positions)
	{
		var closestPos = positions[0];
		var shortestSqrMag = Vector3.SqrMagnitude(closestPos.transform.position - this.transform.position);

		for (var i = 1; i < positions.Length; ++i)
		{
			var sqrMag = Vector3.SqrMagnitude(positions[i].transform.position - this.transform.position);
			if (sqrMag < shortestSqrMag)
			{
				closestPos = positions[i];
				shortestSqrMag = sqrMag;
			}
		}

		return closestPos.transform;
	}
}
