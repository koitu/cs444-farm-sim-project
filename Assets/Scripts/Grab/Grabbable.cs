using System;
using FillContainer;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grab
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    
    public class Grabbable : MonoBehaviour
    {
	    // // use the mesh collider instead
		// [Header("Grasping Properties")]
		// public float graspingRadius = 0.1f;

		[Header("Object Properties")]
		public bool throwable = true;
        
        [SerializeField] // mesh to be copied and slightly scaled then apply this material
        public Material highlightMaterial;

        [HideInInspector]
        public Rigidbody body;
        
        [HideInInspector]
        public Collider[] coll;

		// store the grab controller this object will be attached to
		public bool held {get; private set;}
        private GrabController _heldBy;
        private Transform _initialParent;
        
        // highlighting parameters
        private bool _highlighting;
        private GameObject _highlightObject;

        // store previous position and rotation values
        private Vector3 _prevPosition;
        private Quaternion _prevRotation;

        [HideInInspector]
		public ContainableItem containableItem;
        [HideInInspector]
        public bool isContainableItem;


        private void Start()
        {
	        body = GetComponent<Rigidbody>();
	        coll = GetComponents<Collider>();
	        
	        containableItem = GetComponent<ContainableItem>();
	        isContainableItem = (containableItem != null);
	        
            gameObject.layer = LayerMask.NameToLayer(GrabController.grabbableLayerName);
			_initialParent = transform.parent;
        }

        private void FixedUpdate()
        {
	        if (!held) return;
	        
			_prevPosition = body.position;
			_prevRotation = body.rotation;
        }

        private static void SetLayerRecursive(Transform obj, int fromLayer, int toLayer)
        {
	        if (obj.gameObject.layer == fromLayer)
	        {
		        obj.gameObject.layer = toLayer;
	        }

	        for (int i = 0; i < obj.childCount; i++)
	        {
		        SetLayerRecursive(obj.transform.GetChild(i), fromLayer, toLayer);
	        }
        }
        
        private void SetLayers(String fromLayer, String toLayer)
        {
	        int fromLayerVal = LayerMask.NameToLayer(fromLayer);
	        int toLayerVal = LayerMask.NameToLayer(toLayer);

			SetLayerRecursive(transform, fromLayerVal, toLayerVal);
        }

        public void GrabObject(GrabController grabController)
        {
	        // play nice with objects that could be inside containers
	        if (isContainableItem && containableItem.isAttached)
	        {
		        containableItem.Detach();
	        }
	        
	        // grabbed object should follow the hand
	        body.isKinematic = true;
	        body.transform.parent = grabController.transform.parent;
	        
	        // should not be able to range grab held object (but should be able to near grab it)
            gameObject.layer = LayerMask.NameToLayer(GrabController.grabbingLayerName);
	        // SetLayers(
		       //  GrabController.grabbableLayerName,
		       //  GrabController.grabbingLayerName);

	        held = true;
	        _heldBy = grabController;
        }
        
        public void ReleaseObject(GrabController grabController)
        {
	        if (grabController != _heldBy) return;

	        held = false;
	        _heldBy = null;
	        
            gameObject.layer = LayerMask.NameToLayer(GrabController.grabbableLayerName);
	        // SetLayers(
		       //  GrabController.grabbingLayerName,
		       //  GrabController.grabbableLayerName);
	        
	        // release the object from the hand and set the velocity
	        body.isKinematic = false;
	        body.transform.parent = _initialParent;
	        body.velocity = GetVelocity();
	        body.angularVelocity = GetRotation();
        }

        // since some object could have multiple colliders we need to consider this
        // e.g. a container cannot use a convex collider as it needs to hold stuff
        public Vector3 ClosestPoint(Vector3 pos)
        {
	        Vector3 best = coll[0].ClosestPoint(pos);
	        float bestDist = Vector3.Distance(best, pos);

	        for (int i = 1; i < coll.Length; i++)
	        {
				Vector3 cur = coll[i].ClosestPoint(pos);
				float curDist = Vector3.Distance(cur, pos);
				
		        if (curDist < bestDist)
		        {
			        best = cur;
			        bestDist = curDist;
		        }
	        }

	        return best;
        }

        public Vector3 GetVelocity()
        {
			float deltaPos = Vector3.Distance(_prevPosition, transform.position) / Time.fixedDeltaTime;
			return (transform.position - _prevPosition).normalized * deltaPos;
        }

        public Vector3 GetRotation()
        {
	        Quaternion deltaRot = transform.rotation * Quaternion.Inverse(_prevRotation);
	        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
	        return (axis / Time.fixedDeltaTime) * (angle * Mathf.Deg2Rad);
        }

        // highlight an object when it is selected
        public void Highlight()
        {
	        if (_highlighting) return;
	        _highlighting = true;

	        if (highlightMaterial == null) return;

	        MeshRenderer mr = GetComponent<MeshRenderer>();
	        if (mr == null) return;
	        
	        //Creates a slightly larger copy of the mesh and sets its material to highlight material
	        _highlightObject = new GameObject();
	        _highlightObject.transform.parent = transform;
	        _highlightObject.transform.localPosition = Vector3.zero;
	        _highlightObject.transform.localRotation = Quaternion.identity;
	        _highlightObject.transform.localScale = Vector3.one * 1.001f;

	        Material[] mats = new Material[mr.materials.Length];
	        for(int i = 0; i < mats.Length; i++) {
		        mats[i] = highlightMaterial;
	        }
	        _highlightObject.GetComponent<MeshRenderer>().materials = mats;
	        _highlightObject.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        }

        // stop highlighting an object when it no longer selected
        public void UnHighlight()
        {
	        if (!_highlighting) return;
	        _highlighting = false;

	        if (_highlightObject != null)
	        {
		        Destroy(_highlightObject);
	        }
        }

		// void OnDrawGizmos()
		// {
		// 	// Draw a blue sphere at the transform's position with the radius specified
		// 	Gizmos.color = Color.red;
		// 	Gizmos.DrawWireSphere(transform.position, graspingRadius);
		// }
		//
		// public float get_grasping_radius()
		// {
		// 	return graspingRadius;
		// }
    }
}