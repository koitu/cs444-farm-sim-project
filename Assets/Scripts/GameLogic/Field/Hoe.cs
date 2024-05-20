using UnityEngine;

namespace GameLogic.Field
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