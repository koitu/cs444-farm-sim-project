using Grab;
using UnityEngine;

namespace FillContainer
{
    [RequireComponent(typeof(FixedJoint))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]

    public class ContainableItem : MonoBehaviour
    {

        // Store the container to which the object is attached
        public bool isAttached;
        protected ContainerItem containerAttached;

        // Store initial transform parent
        protected Transform initial_transform_parent;

        // Store element's rigidbody
        private FixedJoint _fixedJoint;
        private Rigidbody _rigidbody;
        private Grabbable _grabbable;

        private float originalMass;
        
        protected float releasedInstance;

        private void Start()
        {
            _fixedJoint = GetComponent<FixedJoint>();
            _rigidbody = GetComponent<Rigidbody>();
            _grabbable = GetComponent<Grabbable>();
            
            initial_transform_parent = transform.parent;
            
            originalMass = _rigidbody.mass;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Trying to get the ContainerItem component from the 'other' GameObject
            ContainerItem container = other.GetComponent<ContainerItem>();

            // Check if the script exists to avoid NullReferenceException
            if (container != null && Time.time - releasedInstance > 2f)
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
            _fixedJoint.connectedBody = container.rigidbody;

            // should not be able to range grab stuff in a container
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            
            // Call the AddItem method on the container, passing this component
            isAttached = true;
            containerAttached = container;
            containerAttached.AddItem(this);
        }

        public void Detach()
        {
            if (!isAttached) return;
            transform.SetParent(initial_transform_parent);

            // detach the object from the container
            _rigidbody.mass = originalMass;
            _fixedJoint.connectedBody = null;

            gameObject.layer = LayerMask.NameToLayer("Grabbable");
            
            // Call the RemoveItem method on the container, passing this component
            isAttached = false;
            containerAttached.RemoveItem(this);
            containerAttached = null;
            releasedInstance = Time.time;
        }
    }
}
