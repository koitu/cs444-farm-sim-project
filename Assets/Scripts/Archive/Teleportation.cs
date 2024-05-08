using System.Collections;
using UnityEngine;

namespace Archive
{
	public class Teleportation : MonoBehaviour
	{
		// Store the hand type to know which button should be pressed
		public enum HandType : int { LeftHand, RightHand };
		[Header("Hand Properties")]
		public HandType handType;

		[Header("Maximum Distance")]
		[Range(2f, 30f)]
		// maximum distance the player can teleport to
		public float maximumTeleportationDistance = 15f; // TODO

		[Header("Marker")]
		// Store the refence to the marker prefab used to highlight the targeted point
		public GameObject markerPrefab;

		[Header("Fader")]
		public OVRScreenFade screenFader;

		// Retrieve the character controller used later to move the player in the environment
		protected CharacterController character_controller;

		[SerializeField]
		LineRenderer lineRenderer;

		void Start()
		{
			character_controller = FindObjectOfType<CharacterController>();
		}

		// check if the ray should be activated
		private bool activate_ray_button_pressed()
		{
			if (handType == HandType.LeftHand)
			{
				// Check if the left index finger is pressing A
				return OVRInput.Get(OVRInput.RawButton.A);
			}
			else
			{
				// Check if the right index finger is pressing A
				return OVRInput.Get(OVRInput.RawButton.A);
			}
		}

		private bool activate_tp_button_pressed()
		{
			if (handType == HandType.LeftHand)
			{
				// Check if the left front finger is gripping
				return OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0.5;
			}
			else
			{
				// Check if the right front finger is gripping
				return OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.5;
			}
		}

		private Ray ray;
		private RaycastHit hit;

		private bool ray_hit;
		private Vector3 ray_end_position;

		private GameObject marker_prefab_instanciated;

		// Keep track of the teleportation state to prevent continuous teleportation
		protected float lastTeleport = 0f;
		protected bool lastActivatedRay;

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
				if (marker_prefab_instanciated == null)
				{
					marker_prefab_instanciated = GameObject.Instantiate(markerPrefab, this.transform);
				}
				marker_prefab_instanciated.transform.position = ray_end_position;
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

		private void Update()
		{

			if (activate_ray_button_pressed())
			{
				activate_ray();

				if (activate_tp_button_pressed() && Time.time - this.lastTeleport > 0.2f)
				{
					if (!ray_hit)
					{
						return;
					}

					this.screenFader.FadeOut();
					character_controller.Move(ray_end_position - this.transform.position);
					//character_controller.transform.position = new Vector3(ray_end_position.x, ray_end_position.y + 1.5f, ray_end_position.z);
					this.lastTeleport = Time.time;
					Debug.LogWarning("SHOULD BE TELEPORTING XDDLOL " + character_controller.transform.position);
					this.screenFader.FadeIn();
				}
			}

			bool activated_ray = this.activate_ray_button_pressed();
			if (activated_ray == this.lastActivatedRay) return;
			this.lastActivatedRay = activated_ray;

			if (!activated_ray)
			{
				lineRenderer.enabled = false;
				ray_hit = false;

				if (marker_prefab_instanciated != null) Destroy(marker_prefab_instanciated);
				marker_prefab_instanciated = null;
			}

		}
	}
}
