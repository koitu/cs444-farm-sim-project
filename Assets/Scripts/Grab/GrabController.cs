using System;
using UnityEngine;

using Quaternion = UnityEngine.Quaternion;

namespace Grab
{
	public class GrabController : MonoBehaviour
	{
		// Config Begin ========================
		
		[Header("Maximum Grab Distance")] [Range(2f, 30f)]
		// maximum distance the player can grab an object from
		public float maximumGrabDistance = 15f;
		
		// [Header("Non-match line appearance")]
		[SerializeField]
		// used when we have not made a match
		public LineRenderer lookingLine;
		
		// [Header("Match line appearance")]
		[SerializeField]
		// used when we found an object
		public LineRenderer matchingLine;
		
		// [Header("Locked line appearance")]
		[SerializeField]
		// used when we lock onto an object
		public LineRenderer lockedLine;

		[SerializeField]
		// controller button presses helper
		private HandController handController;
		
		// Config End ========================
		
		
		// Layer Mask Begin ======================== 

		// layer applied to all grabbables
		public const string grabbableLayerName = "Grabbable";
		
		// layer applied to items that are currently being grabbed
		public const string grabbingLayerName = "Grabbing";
		
		[HideInInspector]
		// the layers that can be grabbed
		public LayerMask grabbableLayers;

		[HideInInspector]
		// the layers that can be range grabbed
		public LayerMask rangeGrabbableLayers;
		
		// Layer Mask End ======================== 
		
		
		// sphere overlap parameters (near grab)
		private Collider[] _sphereOverlapHits;
		private const int SphereOverlapMaxHits = 64;
		private const float SphereOverlapRadius = 0.5f; // we will also check the object's grasping radius
		private bool _colValid;
		private Collider _col;
		
		// sphere cast parameters (range grab)
		private RaycastHit[] _sphereCastHits;
		private const int SphereCastMaxHits = 512;
		private const float SphereCastRadius = 1.5f;
		private const float SphereCastMaxAngle = 10f;
		private bool _hitValid;
		private RaycastHit _hit;
		
		// other parameters
		private const int CurveSamples = 256;  // amount of samples to use when drawing curve
		private const float PullingConstTime = 0.8f;  // approx time for object to be pulled in
		public float velocityThreshold = 2;  // velocity threshold of user to activate pull
		
		private void Start()
		{
			// if image is being held we only want it be grabbable by the other hand
			// also the range grab should not interact with the held object
			grabbableLayers = LayerMask.GetMask(grabbableLayerName, grabbingLayerName);
			rangeGrabbableLayers = LayerMask.GetMask(grabbableLayerName);
			
			_sphereCastHits = new RaycastHit[SphereCastMaxHits];
			_sphereOverlapHits = new Collider[SphereOverlapMaxHits];
		}

		
		private void RangeSearchForMatches()
		{
			int hitNum = Physics.SphereCastNonAlloc(
				transform.position,
				SphereCastRadius,
				transform.forward,
				_sphereCastHits,
				maximumGrabDistance,
				rangeGrabbableLayers);

			float bestAngle = float.MaxValue;
			_hitValid = false;
			RaycastHit bestHit = default;

			for (int i = 0; i < hitNum; i++)
			{
				// every grabbable object *must* have a rigidbody
				RaycastHit cur = _sphereCastHits[i];
				Vector3 hitDirection = (cur.transform.position - transform.position).normalized;

				if (IsOccluded(transform.position, hitDirection, cur.collider) &&
				    IsOccluded(transform.position + (transform.up * 0.3f), hitDirection, cur.collider)) continue;

				float angle = Vector3.Angle(transform.forward, hitDirection);
				if (SphereCastMaxAngle < angle || bestAngle < angle) continue;

				bestAngle = angle;
				_hitValid = true;
				bestHit = cur;
			}

			if (_hitValid)
			{
				_hit = bestHit;
			}
			
			// make sure to clear previous results
			Array.Clear(_sphereCastHits, 0, hitNum);
		}
		
		private void NearSearchForMatches()
		{
			int colNum = Physics.OverlapSphereNonAlloc(
				transform.position,
				SphereOverlapRadius,
				_sphereOverlapHits,
				grabbableLayers);
			
			float bestDst = float.MaxValue;
			_colValid = false;
			Collider bestCol = default;

			for (int i = 0; i < colNum; i++)
			{
				Collider cur = _sphereOverlapHits[i];
				float dst = Vector3.Distance(transform.position, cur.transform.position);

				Grabbable obj = cur.gameObject.GetComponent<Grabbable>();
				if (obj.get_grasping_radius() < dst || bestDst < dst) continue;

				_colValid = true;
				bestCol = cur;
				bestDst = dst;
			}

			if (_colValid)
			{
				_col = bestCol;
			}
		}
		
		private void PullingSearchForMatch()
		{
			int colNum = Physics.OverlapSphereNonAlloc(
				transform.position,
				SphereOverlapRadius,
				_sphereOverlapHits,
				grabbableLayers);
			
			_colValid = false;

			for (int i = 0; i < colNum; i++)
			{
				Collider cur = _sphereOverlapHits[i];
				
				if (cur.attachedRigidbody != _grabbable.body) continue;
				
				float dst = Vector3.Distance(transform.position, _grabbable.transform.position);
				if (_grabbable.get_grasping_radius() < dst) break;

				_colValid = true;
			}
		}
		
		
		// // Compute the velocity for the object to come to you (constant angle with variable speed and time)
		// // Half Life Alyx distance grab: https://www.youtube.com/watch?v=WU23Uj1oeh8
		// public Vector3 PullInitialVelocity()
		// {
		//  Vector3 diff = transform.position - _grabbable.transform.position;
		// 	Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);
		// 	float diffXZLength = diffXZ.magnitude;
		// 	float diffYLength = diff.y;
		//
		// 	float angleInRadian = jumpAngleInDegree * Mathf.Deg2Rad;
		//
		// 	float jumpSpeed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(diffXZLength, 2)/
		// 	                             (2 * Mathf.Cos(angleInRadian) * Mathf.Cos(angleInRadian) * 
		// 	                              (diffXZ.magnitude * Mathf.Tan(angleInRadian) - diffYLength)));
		//
		// 	Vector3 jumpVelocityVector = diffXZ.normalized * (Mathf.Cos(angleInRadian) * jumpSpeed) +
		// 	                             Vector3.up * (Mathf.Sin(angleInRadian) * jumpSpeed);
		//
		// 	return jumpVelocityVector;
		// }

		// compute the velocity for an object to come to you (constant time with variable speed and angle)
		public Vector3 PullInitialVelocity()
		{
			Vector3 d = transform.position - _grabbable.transform.position;
			Vector3 dXZ = new Vector3(d.x, 0, d.z);
			float dXZLen = dXZ.magnitude;
			float dYLen = d.y;

			float angle = Mathf.Atan2(
				2 * dYLen - Physics.gravity.y * Mathf.Pow(PullingConstTime, 2),
				2 * dXZLen);

			Debug.LogWarningFormat("angle {0}", angle);
			float speed = dXZLen / (PullingConstTime * Mathf.Cos(angle));

			return dXZ.normalized * (Mathf.Cos(angle) * speed) +
			       Vector3.up * (Mathf.Sin(angle) * speed);
		}

		public Vector3 PullInitialRotation()
		{
			Quaternion q = Quaternion.FromToRotation(_grabbable.transform.up, transform.up);
			q.ToAngleAxis(out float angle, out Vector3 axis);

			// the angular velocity vector of the rigidbody measured in radians per second:
			// _grabbable.body.angularVelocity
			return axis.normalized * (angle * Mathf.Deg2Rad) / (PullingConstTime - 0.1f);
		}

		enum GrabState
		{
			Holding, // currently holding an object
			Pulling, // pulling in an object
			Locked, // locked onto an object
			Looking, // looking for an object
			Nothing
		}

		private GrabState _grabState = GrabState.Nothing;
		private Grabbable _grabbable;  // the object we are currently interacting with
		private float _pullingLockoutTime;
		private bool _holdingIdxTrigger;
		private bool _holdingHandTrigger;

		private Vector3 _prevPosition;
		private Vector3 _prevVelocity;
		private Quaternion _prevRotation;
		
		private void Update()
		{
			// notes about the pull motion:
			// - motion should take about the same time every time (0.5 seconds?)
			//		- this means that velocity should vary based on distance
			// - object should have velocity pointing in direction of the player's hand
			//		- such that even when they move should still come at the same time?
			// - object should have some rotation so they are straight up when the player grabs them
			//		- after arriving to the player's hand rotation is cancelled?
			
			// feedback
			// - one vibrate when object is found by laser
			// - a click noise (and maybe some rumbles) when object is locked on
			// - some kind of feedback indicating time of flight
			Vector3 velocity = (transform.position - _prevPosition) / Time.deltaTime;
			
			switch (_grabState)
			{
				case GrabState.Holding:
					// if holding an object then continue to hold it
					if (handController.index_trigger_pressed() > 0.5) break;
					
					// if no longer holding an object then release it
					_grabbable.ReleaseObject(this);
					_grabState = GrabState.Nothing;
					break;
				
				case GrabState.Pulling:
					// maintain current state
					if (handController.index_trigger_pressed() > 0.5)
					{
						if (Vector3.Distance(transform.position, _grabbable.transform.position) < 0.1f)
						{
							_grabbable.body.useGravity = true;
							_grabbable.body.freezeRotation = false;
							_grabbable.GrabObject(this);
							_grabState = GrabState.Holding;
						}
						else if (_pullingLockoutTime < 0.1f)
						{
							_grabbable.body.useGravity = false;
							_grabbable.body.freezeRotation = true;
							_grabbable.body.velocity = _grabbable.body.velocity.magnitude *
							                           (transform.position - _grabbable.transform.position).normalized;
						}
						else
						{
							_grabbable.body.velocity += (velocity - _prevVelocity) / _pullingLockoutTime;
						}
					}
					else
					{
						_grabState = GrabState.Nothing;
					}
					
					_pullingLockoutTime -= Time.deltaTime;
					break;
					
				//  // alternate pulling method that requires the player to grab the object out of the air
				// case GrabState.Pulling:
				// 	// // TODO: if direction of velocity is somewhat pointed to us (i.e. within some angle) then allow some course correction
				// 	// // we also need to consider the effect of gravity...
				// 	// Vector3 angleOfAttack = (_grabbable.transform.position - transform.position).normalized;
				// 	// float angle = Vector3.Angle(angleOfAttack, _grabbable.body.velocity.normalized);
				// 	
				// 	// the only thing that changes over the course of the flight is the vertical velocity
				// 	// we can use PullInitialVelcotiy and modify the y velocity based on how much time has passed
				// 	// to get the velocity that it would be needed to take to get to the hand (however we are already partially on a different trajecotiry)
				//
				// 	// upgrade to holding: press the index trigger near the object we are pulling in
				// 	if (handController.index_trigger_pressed() > 0.5)
				// 	{
				// 		// not changing state so need to check for holding
				// 		if (_holdingIdxTrigger) break;
				// 		_holdingIdxTrigger = true;
				//
				// 		// only check for the object we are pulling in
				// 		PullingSearchForMatch();
				// 		if (_colValid)
				// 		{
				// 			_grabbable.GrabObject(this);
				// 			_grabState = GrabState.Holding;
				// 			break;
				// 		}
				// 	}
				// 	else
				// 	{
				// 		_holdingIdxTrigger = false;
				// 	}
				//
				// 	// maintain current state until lockout time is over
				// 	if (_pullingLockoutTime < 0)
				// 	{
				// 		_grabState = GrabState.Nothing;
				// 		break;
				// 	}
				// 	_pullingLockoutTime -= Time.deltaTime;
				// 	break;

				case GrabState.Locked:
					// upgrade to pulling: make a large enough movement
					if (velocity.magnitude > velocityThreshold)
					{
						_pullingLockoutTime = PullingConstTime;  // time for object to be pulled
						_grabbable.body.isKinematic = false;
						_grabbable.body.velocity = PullInitialVelocity() + velocity;
						_grabbable.body.angularVelocity = PullInitialRotation();
						Debug.LogWarningFormat("velocity {0} rotation {1}", PullInitialVelocity(), PullInitialRotation());
						_grabbable.UnHighlight();
						_grabState = GrabState.Pulling;
						ClearDrawing();
						break;
					}
					
					// maintain current state
					if (handController.index_trigger_pressed() > 0.5)
					{
						DrawLocked();
					}
					else
					{
						_grabbable.body.isKinematic = false;
						_grabbable.UnHighlight();
						_grabState = GrabState.Nothing;
						ClearDrawing();
					}
					break;
				
				case GrabState.Looking:
					// upgrade to locked: press index trigger while beam found a match
					if (handController.index_trigger_pressed() > 0.5)
					{
						// not changing state so need to check for holding
						if (!_holdingIdxTrigger)
						{
							_holdingIdxTrigger = true;
							
							// check range search for matches, if so then lock on
							if (_hitValid)
							{
								_grabbable = _hit.rigidbody.gameObject.GetComponent<Grabbable>();
								_grabbable.body.isKinematic = true;
								_grabbable.Highlight();
								_grabState = GrabState.Locked;
								DrawLocked();
								break;
							}
						}
					}
					else
					{
						_holdingIdxTrigger = false;
					}

					// maintain current state
					if (handController.hand_trigger_pressed() > 0.5)
					{
						RangeSearchForMatches();
						if (_hitValid)
						{
							DrawMatch();
						}
						else
						{
							DrawLooking();
						}
					}
					else
					{
						_grabState = GrabState.Nothing;
						ClearDrawing();
					}
					break;
				
				case GrabState.Nothing:
					// upgrade to holding: press index trigger while near object that can be grabbed
					if (handController.index_trigger_pressed() > 0.5)
					{	
						// not changing state so need to check for holding
						if (!_holdingIdxTrigger)
						{
							_holdingIdxTrigger = true;

							// check the nearby area for anything to grab, if so grab it
							NearSearchForMatches();
							if (_colValid)
							{
								_grabbable = _col.gameObject.GetComponent<Grabbable>();
								_grabbable.GrabObject(this);
								_grabState = GrabState.Holding;
								ClearDrawing();
								break;
							}
						}
					}
					else
					{
						_holdingIdxTrigger = false;
					}
					
					// upgrade to looking: press hand trigger
					if (handController.hand_trigger_pressed() > 0.5)
					{
						RangeSearchForMatches();
						if (_hitValid)
						{
							DrawMatch();
						}
						else
						{
							DrawLooking();
						}
						_grabState = GrabState.Looking;
					}
					break;
			}

			_prevPosition = transform.position;
			_prevVelocity = velocity;
			_prevRotation = transform.rotation;
		}
		

		private bool IsOccluded(Vector3 start, Vector3 direction, Collider target)
		{
			// range grab should not be occluded by held items
			return !Physics.Raycast(
				       start,
				       direction,
				       out RaycastHit hit,
				       maximumGrabDistance,
				       Physics.DefaultRaycastLayers | rangeGrabbableLayers) ||
			       hit.collider != target;
		}
		
		
		private void ClearDrawing()
		{
			lookingLine.enabled = false;
			matchingLine.enabled = false;
			lockedLine.enabled = false;
		}
		
		
		private void DrawLooking()
		{
			matchingLine.enabled = false;
			lockedLine.enabled = false;

			bool occulsion = Physics.Raycast(
				transform.position,
				transform.forward,
				out RaycastHit end,
				maximumGrabDistance);

			lookingLine.enabled = true;
			lookingLine.positionCount = 2;
			lookingLine.SetPosition(0, transform.position);
			if (occulsion)
			{
				lookingLine.SetPosition(1, end.point);
			}
			else
			{
				lookingLine.SetPosition(1, transform.position +
				                           transform.forward * maximumGrabDistance);
				                           // transform.forward * (maximumGrabDistance * 0.3f));
			}
		}

		private Vector3 Midpoint()
		{
			Vector3 hitDirection = (_hit.transform.position - transform.position).normalized;
			float angle = Vector3.Angle(transform.forward, hitDirection);
			
			// get point above the midpoint of begin and end in the direction of transform.forward
			return transform.position + 
			       transform.forward * ((hitDirection / 2).magnitude / Mathf.Cos(Mathf.Deg2Rad * angle));
		}
		

		private void DrawMatch()
		{
			lookingLine.enabled = false;
			lockedLine.enabled = false;
			
			Vector3[] points = UI.Bezier.QuadraticInterp(
				transform.position, 
				Midpoint(),
				_hit.transform.position, 
				CurveSamples);
			
			matchingLine.enabled = true;
			matchingLine.positionCount = CurveSamples;
			matchingLine.SetPositions(points);
		}

		private void DrawLocked()
		{
			lookingLine.enabled = false;
			matchingLine.enabled = false;
			
			Vector3[] points = UI.Bezier.QuadraticInterp(
				transform.position, 
				Midpoint(),
				_hit.transform.position, 
				CurveSamples);
			
			lockedLine.enabled = true;
			lockedLine.positionCount = CurveSamples;
			lockedLine.SetPositions(points);
		}
	}
}
