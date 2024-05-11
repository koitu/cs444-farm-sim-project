using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

using static OVRHand;
using Quaternion = System.Numerics.Quaternion;

// TODO: make RayVisualizer a parent file
// TODO: make RayInteraction and RayTeleportation files that inherits from the parent

namespace Archive
{
	public class VisualizeRay : MonoBehaviour
	{
		// Store the hand type to know which button should be pressed
		public enum HandType : int { LeftHand, RightHand };
		[Header( "Hand Properties" )]
		public HandType handType;

		protected HandControllerOld handController;

		[Header("Maximum Distance")] [Range(2f, 30f)]
		// maximum distance the player can teleport to
		public float maximumTeleportationDistance = 15f;
		
		[SerializeField]
		LineRenderer lineRenderer;

		private void Start() 
		{
			this.handController = this.GetComponent<HandControllerOld>();
		}

		// check if the ray should be activated
		private bool activate_ray_button_pressed()
		{
			if (handType == HandType.LeftHand)
			{ 
				// Check if the left index finger is pressing
				return OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0.5;
			}
			else
			{
				// Check if the right index finger is pressing
				return OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.5;
			}
		}

		private bool activate_grab_button_pressed()
		{
			if (handType == HandType.LeftHand)
			{ 
				// Check if the left middle finger is gripping
				return OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) > 0.5;
			}
			else
			{
				// Check if the right middle finger is gripping
				return OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.5;
			}
		}

		private Ray ray;
		private RaycastHit hit;
		
		private bool ray_hit;
		private Vector3 ray_end_position;
		
		private void activate_ray()
		{
			// send out the ray
			ray_hit = Physics.Raycast(
				this.transform.position,
				this.transform.forward,
				out hit,
				100);

			// check if the ray has collided with an object within the maximum distance
			if (ray_hit && hit.distance <= maximumTeleportationDistance)
			{
				// if ray does hit something change the color and draw the ray
				ray_end_position = hit.point;
			}
			else
			{
				// if ray does not hit anything draw a ray that is of length 100
				ray_end_position = this.transform.position + (this.transform.forward * 100);
			}
			
			// add an option to make this a curve instead
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(0, this.transform.position);
			lineRenderer.SetPosition(1, ray_end_position);
		}

		// private bool activated_grab_previous_frame = false;
		
		// private ObjectAnchor object_grasped = null;
		
		private bool moving_object = false;
		private bool holding_object = false;
		private Rigidbody object_grasped = null;
		
		private void Update()
		{
			if (holding_object)
			{
				lineRenderer.enabled = false;

				if (moving_object)
				{
					Vector3 target_location = this.transform.position + (this.transform.forward * 0.5f);

					if (Vector3.Distance(target_location, object_grasped.transform.position) > 0.3f)
					{
						object_grasped.velocity = (target_location - object_grasped.transform.position).normalized * 16f;
					}
					else
					{
						object_grasped.velocity = Vector3.zero;
						// doesn't work...
						// object_grasped.rotation = new UnityEngine.Quaternion();
						
						ObjectAnchor objectAnchor = object_grasped.GetComponent<ObjectAnchor>();
						if (objectAnchor)
						{
							objectAnchor.attach_to(handController);
							moving_object = false;
						}
					}
				}

				if (!activate_grab_button_pressed() && moving_object)
				{
					object_grasped.velocity = object_grasped.velocity / 2;
				}


				if (!activate_grab_button_pressed() && holding_object)
				{
					moving_object = false;
					holding_object = false;
					object_grasped.useGravity = true;
					object_grasped.freezeRotation = false;
					// object_grasped.detectCollisions = true;
					
					ObjectAnchor objectAnchor = object_grasped.GetComponent<ObjectAnchor>();
					objectAnchor.detach_from(handController);
					object_grasped = null;
				}
				
				return;
			}
			
			if (!activate_ray_button_pressed())
			{
				lineRenderer.enabled = false;
				ray_hit = false;
			}
			else
			{
				activate_ray();
			}

			if (ray_hit &&
				hit.rigidbody != null &&
				activate_grab_button_pressed())
			{
				// object_grasped = hit.collider.gameObject.GetComponent<ObjectAnchor>();
				moving_object = true;
				holding_object = true;
				object_grasped = hit.rigidbody;
				object_grasped.useGravity = false;
				object_grasped.freezeRotation = true;
				// object_grasped.detectCollisions = false;
			}
		}
	}
}
