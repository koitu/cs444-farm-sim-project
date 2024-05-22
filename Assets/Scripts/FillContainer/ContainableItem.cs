using System;
using Grab;
using UnityEngine;

namespace FillContainer
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]

    public class ContainableItem : MonoBehaviour
    {

        // Store the container to which the object is attached
        public bool isAttached;
        private ContainerItem _containerAttached;

        // Store initial mass and transform parent
        private float _initialMass;
        private Transform _initialTransformParent;

        // Store element's rigidbody
        private SpringJoint _springJoint;
        private FixedJoint _fixedJoint;
        private Rigidbody _rigidbody;
        private Grabbable _grabbable;

        // wait a bit before properly attaching the object
        private float timeSinceAttached;
        private bool fixedAttached;


        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _grabbable = GetComponent<Grabbable>();
            
            _initialMass = _rigidbody.mass;
            _initialTransformParent = transform.parent;
        }

        private void Update()
        {
            if (isAttached) timeSinceAttached += Time.deltaTime;

            // if attachment is not fixed yet then upgrade the spring joint to a fixed joint
            // this allows the object a little bit to move around before being properly affixed
            if (!fixedAttached && timeSinceAttached > 1f)
            {
                Destroy(_springJoint);
                _springJoint = null;

                // if we apply a fixed joint on collision we get weirdness as the objects can overlap
                // this is why we give them a little time to settle down
                gameObject.AddComponent<FixedJoint>();
                _fixedJoint = GetComponent<FixedJoint>();
                _fixedJoint.connectedBody = _containerAttached.body;
                _fixedJoint.enableCollision = true;
                fixedAttached = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Trying to get the ContainerItem component from the 'other' GameObject
            ContainerItem container = other.GetComponent<ContainerItem>();

            // Check if the script exists to avoid NullReferenceException
            if (container != null && !container.upsideDown)
            {
                // If the ContainerItem script is found, attach this item to it
                Attach(container);
            }
        }

        private void Attach(ContainerItem container)
        {
            if (isAttached || _grabbable.held) return;
            transform.SetParent(container.transform);

            // attach the object to the container
            // we also set the mass near zero so that velocity is not redistributed
            _rigidbody.mass = 1e-7f;
            gameObject.AddComponent<SpringJoint>();
            _springJoint = GetComponent<SpringJoint>();
            _springJoint.connectedBody = container.body;
            _springJoint.maxDistance = 10f;
            _springJoint.enableCollision = true;

            // should not be able to range grab stuff in a container
            gameObject.layer = LayerMask.NameToLayer(GrabController.grabbingLayerName);
            
            // Call the AddItem method on the container, passing this component
            isAttached = true;
            fixedAttached = false;
            timeSinceAttached = 0f;
            _containerAttached = container;
            _containerAttached.AddItem(this);
        }

        public void Detach()
        {
            if (!isAttached) return;
            transform.SetParent(_initialTransformParent);

            // detach the object from the container
            _rigidbody.mass = _initialMass;
            if (!fixedAttached)
            {
                Destroy(_springJoint);
                _springJoint = null;
            }
            else
            {
                Destroy(_fixedJoint);
                _fixedJoint = null;
            }

            gameObject.layer = LayerMask.NameToLayer(GrabController.grabbableLayerName);
            
            // Call the RemoveItem method on the container, passing this component
            isAttached = false;
            fixedAttached = false;
            timeSinceAttached = 0f;
            _containerAttached.RemoveItem(this);
            _containerAttached = null;
        }
    }
}
