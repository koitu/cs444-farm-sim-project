using UnityEngine;

namespace Tiling
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    
    public class Seed : MonoBehaviour
    {
        // [SerializeField]
        // private Rigidbody body;
        
        private void Start()
        {
            gameObject.tag = "Seed";
        }
    }
}