using UnityEngine;

public class TeleportController : MonoBehaviour
{
	[Header("Maximum Distance")] [Range(2f, 30f)]
	// maximum distance the player can teleport to
	public float maximumTeleportationDistance = 15f; // TODO

	[Header("UI")]
	// Store the reference to the marker prefab used to highlight the targeted point
	public GameObject markerPrefab;

	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	public ScreenFader screenFader;
	
	[Header("Properties")]
	[SerializeField]
	private HandController handController;

	// Retrieve the character controller used later to move the player in the environment
	private CharacterController _characterController;

	void Start()
	{
		_characterController = FindObjectOfType<CharacterController>();
	}

	// private Ray _ray;
	
	private RaycastHit _hit;
	private bool _hitValid;
	private Vector3 _rayEndPosition;

	private bool _markerCreated;
	private GameObject _marker;

	// Keep track of the teleportation state to prevent continuous teleportation
	private float _sinceLastTeleport = 0f;

	private void activate_ray()
	{
		// TODO: add an option to make this a curve instead
		// send out the ray
		_hitValid = Physics.Raycast(
			this.transform.position,
			this.transform.forward,
			out _hit,
			maximumTeleportationDistance);

		// check if the ray has collided with an object within the maximum distance
		if (_hitValid && _hit.distance <= maximumTeleportationDistance)
		{
			// if ray does hit something change the color and draw the ray
			_rayEndPosition = _hit.point;
			
			if (!_markerCreated)
			{
				_markerCreated = true;
				_marker = Instantiate(markerPrefab, transform);
			}
			_marker.transform.position = _rayEndPosition;
		}
		else
		{
			// if ray does not hit anything draw a straight ray
			_rayEndPosition = transform.position + (transform.forward * maximumTeleportationDistance);
			
			if (_markerCreated)
			{
				_markerCreated = false;
				Destroy(_marker);
			}
		}
	}

	private void Update()
	{
		if (handController.near_button_pressed())
		{
            activate_ray();
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, _rayEndPosition);

            if (handController.index_trigger_pressed() > 0.5 &&
                _sinceLastTeleport > 0.5f &&
                _hitValid)
            {
				screenFader.FadeToBlack();
				
				_sinceLastTeleport = 0f;
				_characterController.Move(_rayEndPosition - transform.position);
				//character_controller.transform.position = new Vector3(ray_end_position.x, ray_end_position.y + 1.5f, ray_end_position.z);
				// Debug.LogWarning("SHOULD BE TELEPORTING XDDLOL " + _characterController.transform.position);
				
				screenFader.FadeToClear();
            }
		}
		else
		{
			if (_markerCreated)
			{
				_markerCreated = false;
				Destroy(_marker);
			}

			lineRenderer.enabled = false;
		}

		_sinceLastTeleport += Time.deltaTime;
	}
}
