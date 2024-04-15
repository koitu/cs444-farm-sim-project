using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerItem : MonoBehaviour
{

    public List<ContainableItem> itemsContained;

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
    }

    public void detachItem(ContainableItem item)
    {
        item.detach();
        itemsContained.Remove(item);
    }
}
