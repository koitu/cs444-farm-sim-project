using System;
using UnityEngine;

using static OVRHand;
using Quaternion = System.Numerics.Quaternion;

public class GrabController : MonoBehaviour
{
	[Header("Maximum Distance")] [Range(2f, 30f)]
	// maximum distance the player can grab an object from
	public float maximumGrabDistance = 15f;
	
	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	private HandController handController;

	
	private Ray ray;
	private RaycastHit hit;
	
	private bool ray_hit;
	private Vector3 ray_end_position;

	
	private RaycastHit[] hits;
	private const int radius = 1; // 0.5 is also pretty valid
	private const int MaxHits = 512;
	private const float maxAngle = 5f;
	
	private void Start() 
	{
		hits = new RaycastHit[MaxHits];
	}


	private int _curveSamples = 256;
	
	private void DrawCurve(Vector3 begin, Vector3 end)
	{
		// TODO: something very fked about this part
		// =================================
		// float angle = Vector3.Angle(transform.forward, end - begin);
		//
		// // get point above the midpoint of begin and end in the direction of transform.forward
		// Vector3 mid = transform.forward * (((begin + end) / 2).magnitude / (float)Math.Cos(angle));
		// =================================

		Vector3 mid = new Vector3(0,0,0);
		Vector3[] points = UI.Bezier.QuadraticInterp(begin, mid, end, _curveSamples);
		// for (int i = 0; i < _curveSamples; i++)
		// {
		// 	lineRenderer.SetPosition(i, points[i]);
		// }
		lineRenderer.enabled = true;
		lineRenderer.positionCount = _curveSamples;
		lineRenderer.SetPositions(points);
	}
	
	private void activate_ray()
	{
		int numHits = Physics.CapsuleCastNonAlloc(
			transform.position,
			transform.position + (transform.forward * maximumGrabDistance),
			radius,
			transform.forward,
			hits,
			maximumGrabDistance);
		
		float minAngle = Single.PositiveInfinity;
		float curAngle;
		Vector3 hitObjectDirection;
		RaycastHit hitObject;

		// iterate over all the objects throwing away invalid ones and choosing the best fit
		for (var i = 0; i < numHits; i++)
		{
			if (hits[i].rigidbody == null)
			{
				numHits--;
				continue;
			}
			
			hitObjectDirection = (hits[i].transform.position - transform.position).normalized;

			// check if object hit is actually in the "line of sight"
			if (!Physics.Raycast(transform.position, hitObjectDirection, out hitObject, maximumGrabDistance)
			    || hitObject.collider != hits[i].collider)
			{
				numHits--;
				continue;
			}
			
			curAngle = Vector3.Angle(
				transform.forward,
				hitObjectDirection);

			// check if the angle is too great
			if (curAngle > maxAngle)
			{
				numHits--;
				continue;
			}
			
			// only retain the object will the least angle
			if (curAngle > minAngle) continue;
			
			minAngle = curAngle;
			hit = hits[i];
		}

		if (numHits == 0)
		{
			ray_end_position = transform.position + (transform.forward * maximumGrabDistance);
			
			// do nothing or draw a beam to display that we don't have anything
			lineRenderer.enabled = true;
			lineRenderer.positionCount = 2;
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, ray_end_position);
			return;
		}
		
		ray_end_position = hit.point;
		DrawCurve(transform.position, ray_end_position);
	}

	// private bool activated_grab_previous_frame = false;
	
	// private ObjectAnchor object_grasped = null;
	
	private bool moving_object = false;
	private bool holding_object = false;
	private Rigidbody object_grasped = null;
	
	private void Update()
	{
		// if (holding_object)
		// {
		//
		// 	return;
		// }
		//
		// // states (in order of priority):
		// // - holding object
		// // - object is nearby
		// // - object is being pulled in
		// // - object is locked on
		// // - laser is looking for objects
		// // - nothing
		// // note that we will not change state unless we change the state of the controller (i.e. press or release a button)
		//
		// if (handController.index_trigger_pressed() > 0.5)
		// {
		// 	// search for objects around the hand
		// 	// if there is one near enough than attach it to the hand and set holding_object = true
		// 	
		// 	// if object is close enough should be highlight or should this only be for selected far objects?
		//
		// 	return;
		// }
		
		if (handController.hand_trigger_pressed() > 0.5)
		{
			// if no object is found shoot a straight decaying laser from your controller
			// if object is found then shoot a curved continuous laser from your controller
			activate_ray();
			
			// if hit and index_trigger_pressed then cancel gravity and stuff
			// then this hand is "locked on" to the object
			//		indicate lock on with decaying laser from the object
			//		(can release the hand_trigger button now)
			//		require a flick to pull the object in
			
			// notes about the pull motion:
			// - motion should take about the same time every time (0.5 seconds?)
			//		- this means that velocity should vary based on distance
			// - object should have velocity pointing in direction of the player's hand
			//		- such that even when they move should still come at the same time?
			// - object should ahve some rotation so they are straight up when the player grabs them
			//		- after arriving to the player's hand rotation is cancelled?
			
			// feedback
			// - one vibrate when object is found by laser
			// - a click noise (and maybe some rumbles) when object is locked on
			// - some kind of feedback indicating time of flight
		}
		else
		{
			lineRenderer.enabled = false;
		}
		
		// if (holding_object)
  //       {
  //           lineRenderer.enabled = false;
  //
		// 	if (moving_object)
		// 	{
		// 		Vector3 target_location = this.transform.position + (this.transform.forward * 0.5f);
  //
		// 		if (Vector3.Distance(target_location, object_grasped.transform.position) > 0.3f)
		// 		{
		// 			object_grasped.velocity = (target_location - object_grasped.transform.position).normalized * 16f;
		// 		}
		// 		else
  //               {
  //                   object_grasped.velocity = Vector3.zero;
		// 			// doesn't work...
		// 			// object_grasped.rotation = new UnityEngine.Quaternion();
		// 			
		// 			ObjectAnchor objectAnchor = object_grasped.GetComponent<ObjectAnchor>();
		// 			if (objectAnchor)
		// 			{
		// 				objectAnchor.attach_to(handController);
		// 				moving_object = false;
		// 			}
		// 		}
		// 	}
  //
		// 	if (!activate_grab_button_pressed() && moving_object)
  //           {
  //               object_grasped.velocity = object_grasped.velocity / 2;
  //           }
  //
  //
  //           if (!activate_grab_button_pressed() && holding_object)
		// 	{
		// 		moving_object = false;
		// 		holding_object = false;
		// 		object_grasped.useGravity = true;
		// 		object_grasped.freezeRotation = false;
		// 		// object_grasped.detectCollisions = true;
		// 		
		// 		ObjectAnchor objectAnchor = object_grasped.GetComponent<ObjectAnchor>();
		// 		objectAnchor.detach_from(handController);
		// 		object_grasped = null;
		// 	}
		// 	
		// 	return;
		// }
		//
		// if (!activate_ray_button_pressed())
		// {
		// 	lineRenderer.enabled = false;
		// 	ray_hit = false;
		// }
		// else
		// {
		// 	activate_ray();
		// }
  //
		// if (ray_hit &&
		//     hit.rigidbody != null &&
		//     activate_grab_button_pressed())
		// {
		// 	// object_grasped = hit.collider.gameObject.GetComponent<ObjectAnchor>();
		// 	moving_object = true;
		// 	holding_object = true;
		// 	object_grasped = hit.rigidbody;
		// 	object_grasped.useGravity = false;
		// 	object_grasped.freezeRotation = true;
		// 	// object_grasped.detectCollisions = false;
		// }
		
		// hit.rigidbody.velocity = Vector3.zero;
	}
}
