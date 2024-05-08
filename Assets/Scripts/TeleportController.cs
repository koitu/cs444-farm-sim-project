using UnityEngine;

public class TeleportController : MonoBehaviour
{
	[Header("Maximum Distance")]
	[Range(2f, 30f)]
	// maximum distance the player can teleport to
	public float maximumTeleportationDistance = 15f; // TODO

	[Header("Marker")]
	// Store the refence to the marker prefab used to highlight the targeted point
	public GameObject markerPrefab;

	[Header("Fader")]
	public OVRScreenFade screenFader;

	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	private HandController handController;

	// Retrieve the character controller used later to move the player in the environment
	private CharacterController _characterController;

	void Start()
	{
		_characterController = FindObjectOfType<CharacterController>();
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

		if (handController.near_button_pressed())
		{
            activate_ray();

            if (handController.index_trigger_pressed() > 0.5 && Time.time - this.lastTeleport > 0.2f)
            {
                if (!ray_hit)
                {
                    return;
                }

				this.screenFader.FadeOut();
				_characterController.Move(ray_end_position - this.transform.position);
				//character_controller.transform.position = new Vector3(ray_end_position.x, ray_end_position.y + 1.5f, ray_end_position.z);
				this.lastTeleport = Time.time;
				Debug.LogWarning("SHOULD BE TELEPORTING XDDLOL " + _characterController.transform.position);
				this.screenFader.FadeIn();
            }
		}

		bool activated_ray = handController.near_button_pressed();
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
