using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainableItem : MonoBehaviour
{

    // Store the container to which the object is attached
    protected ContainerItem containerAttached = null;

    // Store initial transform parent
    protected Transform initial_transform_parent;
    protected bool initial_kinematic;

    // Store element's rigidbody
    protected Rigidbody rigidbody;
    public Collider collider;
    protected float releasedInstance;

    private void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.collider = this.GetComponent<Collider>();
        this.initial_transform_parent = this.transform.parent;
        this.initial_kinematic = this.rigidbody.isKinematic;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trying to get the ContainerItem component from the 'other' GameObject
        ContainerItem container = other.GetComponent<ContainerItem>();

        // Check if the script exists to avoid NullReferenceException
        if (container != null && Time.time - this.releasedInstance > 2)
        {
            // If the ContainerItem script is found, attach this item to it
            attach_to(container);
        }
    }

    private void attach_to(ContainerItem container)
    {
        if (containerAttached != null) return;

        // Call the AttachItem method on the container, passing this component
        container.attachItem(this);

        this.transform.SetParent(container.transform);
        this.rigidbody.isKinematic = true;
        //this.collider.enabled = false;
        containerAttached = container;
    }

    public void detach()
    {
        if (containerAttached == null) return;

        this.transform.SetParent(this.initial_transform_parent);
        this.rigidbody.isKinematic = this.initial_kinematic;
        //this.collider.enabled = true;
        containerAttached = null;
        this.releasedInstance = Time.time;
    }
}
