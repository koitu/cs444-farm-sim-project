using System;
using GameLogic.Plants;
using Grab;
using UnityEngine;

namespace GameLogic.Field
{   
    // TLDR: both items must have collider, this one must have isTrigger, and at least one must have a Rigidbody
    //
    // https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Collider.OnTriggerEnter.html
    // - Both GameObjects must contain a Collider component
    // - One must have Collider.isTrigger enabled (this)
    // - One must contain a Rigidbody (kinematic is fine?)
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    
    public class FarmPlot : MonoBehaviour
    {
        [SerializeField]
        public Mesh progress0;
        [SerializeField]
        public Mesh progress25;
        [SerializeField]
        public Mesh progress50;
        [SerializeField]
        public Mesh progress75;
        [SerializeField]
        public Mesh progress100;

        // [SerializeField]
        [HideInInspector]
        private MeshFilter meshFilter;  // mesh shape
        
        // [SerializeField]
        [HideInInspector]
        private MeshRenderer meshRenderer;  // mesh material

        private float _progress;
        private bool _planted;
        private Plant _plant;

        void Start()
        {
            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            
            gameObject.tag = "FarmLand";
            // gameObject.layer = LayerMask.NameToLayer("Environment"); // ensure range grab can go through farmland
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // ensure range grab can go through farmland
        }

        private void OnTriggerEnter(Collider c)
        {
            Debug.LogWarningFormat("entered collision with " + c.gameObject.name + " with tag " + c.gameObject.tag);

            switch (c.gameObject.tag)
            {
                case "Hoe":
                    if (_planted || _progress > 1f) break;

                    Grabbable hoe = c.gameObject.GetComponent<Grabbable>();
                    float rot = hoe.GetRotation().magnitude;
                    
                    // expert BeatSaber players can get up to 20pi Rad/s
                    // 8pi Rad/s for great (+0.55)
                    // 4pi Rad/s for good (+0.4)
                    // 2pi Rad/s for ok (+0.3)
                    // otherwise (+0.2)
                    if (rot > 8 * Math.PI)
                    {
                        _progress += 0.55f;
                    }
                    else if (rot > 4 * Math.PI)
                    {
                        _progress += 0.4f;
                    }
                    else if (rot > 2 * Math.PI)
                    {
                        _progress += 0.3f;
                    }
                    else
                    {
                        _progress += 0.2f;
                    }
                    UpdateSoil();
                    break;
                
                case "Plant":
                    if (_planted || _progress < 1f)
                    {
                        c.attachedRigidbody.velocity *= -1.5f; // bounce the plant away
                        // TODO: play a rejection noise
                        break;
                    }

                    // by setting plant to start growing it will plant itself in the plot
                    _plant = c.gameObject.GetComponent<Plant>();
                    _plant.StartGrowing(this);
                    
                    // set planted to true and reset digging progress
                    _planted = true;
                    _progress = 0f;
                    meshFilter.mesh = progress0;
                    break;
            }
        }
        
        private void OnTriggerExit(Collider c)
        {
            Debug.LogWarningFormat("exited collision with " + c.gameObject.name + " with tag " + c.gameObject.tag);
            
            switch (c.gameObject.tag)
            {
                case "Hoe":
                    break;
                
                case "Plant":
                    _planted = false;
                    break;
            }
        }
        
        private void UpdateSoil()
        {
            if (_progress == 0f)
            {
                meshFilter.mesh = progress0;
            }
            else if (_progress < 0.25f)
            {
                meshFilter.mesh = progress25;
            }
            else if (_progress < 0.5f)
            {
                meshFilter.mesh = progress50;
            }
            else if (_progress < 0.75f)
            {
                meshFilter.mesh = progress75;
            }
            else
            {
                meshFilter.mesh = progress100;
            }
        }
    }
}