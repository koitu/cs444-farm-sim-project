using System;
using UnityEngine;

namespace Grab
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    
    public class Grabbable : MonoBehaviour
    {
		[Header("Grasping Properties")]
		public float graspingRadius = 0.1f;

		[Header("Object Properties")]
		public bool throwable = true;
        
        [SerializeField] // mesh to be copied and slightly scaled then apply this material
        public Material highlightMaterial;

        [HideInInspector]
        public Rigidbody body;
        
        [HideInInspector]
        public Collider coll;

		// store the grab controller this object will be attached to
        private bool _held;
        private GrabController _heldBy;
        private Transform _initialParent;
        
        // help for planting
        private bool _planted;
        private GameObject _plantedBy;

        // highlighting parameters
        private bool _highlighting;
        private GameObject _highlightObject;

        // store previous position and rotation values
        private Vector3 _prevPosition;
        private Quaternion _prevRotation;


        private void Start()
        {
	        body = GetComponent<Rigidbody>();
	        coll = GetComponent<Collider>();
            gameObject.layer = LayerMask.NameToLayer(GrabController.grabbableLayerName);
			_initialParent = transform.parent;
        }

        private void FixedUpdate()
        {
	        if (_planted || !_held) return;
	        
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
	        body.isKinematic = true;
	        body.constraints = RigidbodyConstraints.None;
	        body.transform.parent = grabController.transform.parent;
	        
	        SetLayers(
		        GrabController.grabbableLayerName,
		        GrabController.grabbingLayerName);

	        _held = true;
	        _heldBy = grabController;
        }
        
        public void ReleaseObject(GrabController grabController)
        {
	        if (grabController != _heldBy) return;

	        _held = false;
	        _heldBy = null;
	        
	        SetLayers(
		        GrabController.grabbingLayerName,
		        GrabController.grabbableLayerName);
	        
	        body.isKinematic = false;
	        body.transform.parent = _initialParent;
	        body.velocity = GetVelocity() * 0.7f;
	        body.angularVelocity = GetRotation();
        }

        public Vector3 GetVelocity()
        {
			float deltaPos = Vector3.Distance(_prevPosition, transform.position) / Time.fixedDeltaTime;
			return (transform.position - _prevPosition).normalized * (deltaPos * 1.2f);
        }

        public Vector3 GetRotation()
        {
	        Quaternion deltaRot = transform.rotation * Quaternion.Inverse(_prevRotation);
	        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
	        return (axis / Time.fixedDeltaTime) * (angle * Mathf.Deg2Rad) / 1.2f;
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
        
		void OnDrawGizmos()
		{
			// Draw a blue sphere at the transform's position with the radius specified
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, graspingRadius);
		}
		
		public float get_grasping_radius()
		{
			return graspingRadius;
		}
    }
}