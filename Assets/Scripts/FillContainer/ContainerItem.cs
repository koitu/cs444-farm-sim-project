using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerItem : MonoBehaviour
{

    public List<ContainableItem> itemsContained;

    public BoxCollider boxCollider;
    public float maximumColliderYSize;
    public float stepIncrease;
    private float minimumColliderYSize;

    private void Start()
    {
        minimumColliderYSize = boxCollider.size.y;
    }

    void Update()
    {
        // Using the dot product to determine orientation relative to the global up (Vector3.up)
        float dot = Vector3.Dot(transform.up, Vector3.up);
        bool upsideDown = false;

        if (dot < -0.9) // You can adjust the threshold for sensitivity
        {
            Debug.Log("The object is upside down.");
            upsideDown = true;

        }

        for (int i = itemsContained.Count - 1; i >= 0; i--)
        {
            ObjectAnchor itemAnchor = itemsContained[i].GetComponent<ObjectAnchor>();
            if (upsideDown || itemAnchor && itemAnchor.grabbed)
            {
                this.detachItem(itemsContained[i]); // Assuming SomeFunction triggers deletion inside itself
                return;
            }
        }
    }
         
    public void attachItem(ContainableItem item)
    {
        itemsContained.Add(item);
        if (this.boxCollider.size.y < this.maximumColliderYSize)
        {
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y + this.stepIncrease, boxCollider.size.z);
            boxCollider.transform.position = new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y + (this.stepIncrease / 2), boxCollider.transform.position.z);
        }
        else
        {
            boxCollider.size = new Vector3(boxCollider.size.x, this.maximumColliderYSize, boxCollider.size.z);
            boxCollider.transform.position = new Vector3(boxCollider.transform.position.x, (this.maximumColliderYSize - this.minimumColliderYSize) / 2, boxCollider.transform.position.z);
        }
    }

    public void detachItem(ContainableItem item)
    {
        item.detach();
        itemsContained.Remove(item);
        if (this.boxCollider.size.y > this.minimumColliderYSize)
        {
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y - this.stepIncrease, boxCollider.size.z);
            boxCollider.transform.position = new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y - (this.stepIncrease / 2), boxCollider.transform.position.z);
        }
        else
        {
            boxCollider.size = new Vector3(boxCollider.size.x, this.minimumColliderYSize, boxCollider.size.z);
            boxCollider.transform.position = new Vector3(boxCollider.transform.position.x, 0, boxCollider.transform.position.z);
        }
    }
}
