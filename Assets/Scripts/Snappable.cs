using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snappable : MonoBehaviour
{
	private bool snapped = false;
	public bool beingDragged = false;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!snapped)
		{
			if (collision.gameObject.layer == LayerMask.NameToLayer("SnapTarget"))
			{
				var snaps = collision.gameObject.GetComponentsInChildren<Transform>();

				if (snaps.Any())
				{
					var snapPos = FindClosest(snaps);
					this.transform.position = snapPos.position + (Vector3.up*this.GetComponent<Collider>().bounds.extents.y*1.1f);
					this.gameObject.AddComponent<FixedJoint>().connectedBody = snapPos.transform.parent.GetComponent<Rigidbody>();
					this.snapped = true;
					this.GetComponent<Rigidbody>().freezeRotation = true;
				}
			}
		}
	}

	private Transform FindClosest(Transform[] positions)
	{
		var closestPos = positions[0];
		var shortestSqrMag = Vector3.SqrMagnitude(closestPos.position - this.transform.position);

		for (var i = 1; i < positions.Length; ++i)
		{
			var sqrMag = Vector3.SqrMagnitude(positions[i].position - this.transform.position);
			if (sqrMag < shortestSqrMag)
			{
				closestPos = positions[i];
				shortestSqrMag = sqrMag;
			}
		}

		return closestPos;
	}
}
