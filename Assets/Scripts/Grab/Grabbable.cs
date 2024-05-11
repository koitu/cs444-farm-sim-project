using System;
using UnityEngine;

namespace Grab
{
    [RequireComponent(typeof(Rigidbody))]
    
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

		// store the grab controller this object will be attached to
        private bool held = false;
        private GrabController heldBy;
        private Transform initialParent;

        // highlighting parameters
        private bool highlighting = false;
        private GameObject highlightObject;

        // store previous position and rotation values
        private Vector3 prevPosition;
        private Quaternion prevRotation;


        private void Start()
        {
	        body = GetComponent<Rigidbody>();
            gameObject.layer = LayerMask.NameToLayer(GrabController.grabbableLayerName);
			initialParent = transform.parent;
        }

        private void FixedUpdate()
        {
	        if (!held) return;
	        
			prevPosition = body.position;
			prevRotation = body.rotation;
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
	        body.transform.parent = grabController.transform.parent;
	        
	        SetLayers(
		        GrabController.grabbableLayerName,
		        GrabController.grabbingLayerName);

	        held = true;
	        heldBy = grabController;
        }
        
        public void ReleaseObject(GrabController grabController)
        {
	        if (grabController != heldBy) return;

	        held = false;
	        heldBy = null;
	        
	        SetLayers(
		        GrabController.grabbingLayerName,
		        GrabController.grabbableLayerName);
	        
	        body.isKinematic = false;
	        body.transform.parent = initialParent;
	        body.velocity = getVelocity();
	        body.angularVelocity = getRotation();
        }

        public Vector3 getVelocity()
        {
			float deltaPos = Vector3.Distance(prevPosition, transform.position) / Time.fixedDeltaTime;
			return (transform.position - prevPosition).normalized * (deltaPos * 1.2f);
        }

        public Vector3 getRotation()
        {
	        Quaternion deltaRot = transform.rotation * Quaternion.Inverse(prevRotation);
	        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
	        return (axis / Time.fixedDeltaTime) * (angle * Mathf.Deg2Rad) / 1.2f;
        }

        // highlight an object when it is selected
        public void Highlight()
        {
	        if (highlighting) return;
	        highlighting = true;

	        if (highlightMaterial == null) return;

	        MeshRenderer mr = GetComponent<MeshRenderer>();
	        if (mr == null) return;
	        
	        //Creates a slightly larger copy of the mesh and sets its material to highlight material
	        highlightObject = new GameObject();
	        highlightObject.transform.parent = transform;
	        highlightObject.transform.localPosition = Vector3.zero;
	        highlightObject.transform.localRotation = Quaternion.identity;
	        highlightObject.transform.localScale = Vector3.one * 1.001f;

	        Material[] mats = new Material[mr.materials.Length];
	        for(int i = 0; i < mats.Length; i++) {
		        mats[i] = highlightMaterial;
	        }
	        highlightObject.GetComponent<MeshRenderer>().materials = mats;
	        highlightObject.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        }

        // stop highlighting an object when it no longer selected
        public void UnHighlight()
        {
	        if (!highlighting) return;
	        highlighting = false;

	        if (highlightObject != null)
	        {
		        Destroy(highlightObject);
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