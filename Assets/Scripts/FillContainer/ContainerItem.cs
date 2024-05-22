using System.Collections.Generic;
using Grab;
using UnityEngine;

namespace FillContainer
{
    public class ContainerItem : MonoBehaviour
    {

        public List<ContainableItem> itemsContained;

        public Rigidbody rigidbody;

        public BoxCollider boxCollider;
        public float stepIncrease;
        public float maximumColliderYSize;
        private float _minimumColliderYSize;
        private int _overload;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            _minimumColliderYSize = boxCollider.size.y;
        }

        void Update()
        {
            // Determine orientation relative to the global up (Vector3.up)
            // You can adjust the threshold for sensitivity
            if (Vector3.Angle(transform.up, Vector3.up) < 150) return;
            
            Debug.Log("The container is upside down.");

            // Iterate over the objects starting from the last one added
            for (int i = itemsContained.Count - 1; i >= 0; i--)
            {
                // // detaching will be handled by Grabbable instead
                // Grabbable item = itemsContained[i].GetComponent<Grabbable>();
                // if (upsideDown || (item && item.held))
                ContainableItem item = itemsContained[i];
                item.Detach();
            }
        }
         
        public void AddItem(ContainableItem item)
        {
            // add an item to container list (does not attach it)
            itemsContained.Add(item);
        
            // increase boxCollider size as the container fills up
            if (boxCollider.size.y < maximumColliderYSize)
            {
                boxCollider.size += new Vector3(0f, stepIncrease, 0f);
                boxCollider.transform.localPosition += new Vector3(0f, stepIncrease / 2f, 0f);
            }
            else
            {
                _overload += 1;
            }
        }

        public void RemoveItem(ContainableItem item)
        {
            // remove an item from a container list (does not detach it)
            itemsContained.Remove(item);

            // decrease boxCollider size as the container is emptied
            if (_overload > 0)
            {
                _overload -= 1;
            }
            else
            {
                boxCollider.size -= new Vector3(0f, stepIncrease, 0f);
                boxCollider.transform.localPosition -= new Vector3(0f, stepIncrease / 2f, 0f);
            }
        }
    }
}
