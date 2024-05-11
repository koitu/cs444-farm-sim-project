using System;
using System.Security.Cryptography;
using UnityEngine;

namespace Tiling
{
    [RequireComponent(typeof(Rigidbody))]
    
    public class Hoe : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody body;
        
        private FarmLand fl;
        
        private void Start()
        {
            gameObject.tag = "Hoe";
        }

        private void OnTriggerEnter(Collider c)
        {
            Debug.LogWarningFormat("entered collision");
            Debug.LogWarningFormat(c.gameObject.name);
            
            if (c.gameObject.CompareTag("FarmLand"))
            {
                fl = c.gameObject.GetComponent<FarmLand>();

                if (fl.IsPlanted)
                {
                    return;
                }

                if (fl.Progress > 1f)
                {
                    return;
                }
                
                float v = body.angularVelocity.magnitude;

                // expert BeatSaber players can get up to 20pi Rad/s
                // 8pi Rad/s for great (+0.55)
                // 4pi Rad/s for good (+0.4)
                // 2pi Rad/s for ok (+0.3)
                // otherwise (+0.2)
                if (v > 8 * Math.PI)
                {
                    fl.Progress += 0.55f;
                }
                else if (v > 4 * Math.PI)
                {
                    fl.Progress += 0.4f;
                }
                else if (v > 2 * Math.PI)
                {
                    fl.Progress += 0.3f;
                }
                else
                {
                    fl.Progress += 0.2f;
                }
            }
        }

        private void OnTriggerExit(Collider c)
        {
            Debug.LogWarningFormat("exited collided");
            Debug.LogWarningFormat(c.gameObject.tag);
            
            if (c.gameObject.CompareTag("FarmLand"))
            {
                fl.UpdateSoil();
            }
        }
    }
}