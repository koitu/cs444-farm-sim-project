using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnchor : MonoBehaviour
{

	[Header("Grasping Properties")]
	public float graspingRadius = 0.1f;

	[Header("Object Properties")]
	public bool throwable;
	
	public bool grabbed = false;

	// Store initial transform parent
	protected Transform initial_transform_parent;
	protected Rigidbody rigidbody;


	protected Vector3 currVelocity;
	protected Vector3 lastPosition;

	void Start()
	{
		this.rigidbody = this.GetComponent<Rigidbody>();
		this.initial_transform_parent = transform.parent;
	}

	private void Update()
	{
		if (this.grabbed)
		{
			this.calcVelocity();
		}
	}
	protected void calcVelocity()
	{
		float speed = Vector3.Distance(lastPosition, this.transform.position) / Time.deltaTime;
		this.currVelocity = Vector3.Normalize(this.transform.position - lastPosition) * speed * 1.3f;

		//update previous positions
		lastPosition = this.transform.position;
	}

	void OnDrawGizmos()
	{
		// Draw a blue sphere at the transform's position with the radius specified
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, graspingRadius);
	}

	// Store the hand controller this object will be attached to
	protected Archive.HandControllerOld hand_controller = null;

	public void attach_to(Archive.HandControllerOld hand_controller)
	{
		if (this.rigidbody) this.rigidbody.isKinematic = true;
		this.grabbed = true;
		this.lastPosition = this.transform.position;

		// Store the hand controller in memory
		this.hand_controller = hand_controller;

		// Set the object to be placed in the hand controller referential
		transform.SetParent(hand_controller.transform);
	}

	public void detach_from(Archive.HandControllerOld hand_controller)
	{
		// Make sure that the right hand controller ask for the release
		if (this.hand_controller != hand_controller) return;

		// Detach the hand controller
		this.hand_controller = null;

		// Set the object to be placed in the original transform parent
		transform.SetParent(initial_transform_parent);

		if (this.rigidbody) this.rigidbody.isKinematic = false;
		this.grabbed = false;
		if (this.throwable) this.rigidbody.velocity = this.currVelocity;
	}

	public bool is_available() { return hand_controller == null; }

	public float get_grasping_radius() { return graspingRadius; }
}
