using UnityEngine;

namespace Tiling
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    
    public class Hoe : MonoBehaviour
    {
        // [SerializeField]
        // private Rigidbody body;
        
        private void Start()
        {
            gameObject.tag = "Hoe";
        }
    }
}