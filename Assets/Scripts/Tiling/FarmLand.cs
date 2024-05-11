using System;
using Grab;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Tiling
{   
    // TLDR: both items must have collider, this one must have isTrigger, and at least one must have a Rigidbody
    //
    // https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Collider.OnTriggerEnter.html
    // - Both GameObjects must contain a Collider component
    // - One must have Collider.isTrigger enabled (this)
    // - One must contain a Rigidbody (kinematic is fine?)
    [RequireComponent(typeof(Collider))]
    
    public class FarmLand : MonoBehaviour
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

        [SerializeField]
        private MeshFilter _meshFilter;  // mesh shape
        
        [SerializeField]
        private MeshRenderer _meshRenderer;  // mesh material

        private float _progress;
        private bool _planted;
        private bool _doneGrowing;
        private Plant _plant;
        private Grabbable _grabbable;

        void Start()
        {
            gameObject.tag = "FarmLand";
        }

        private void FixedUpdate()
        {
            if (!_planted || _doneGrowing) return;

            if (_plant.GrowTimeLeft <= 0)
            {
                _doneGrowing = true;
                _grabbable.UnPlantObject(gameObject);
            }
        }

        private void OnTriggerEnter(Collider c)
        {
            Debug.LogWarningFormat("entered collision with " + c.gameObject.name);

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
                    break;
                
                case "Plant":
                    Grabbable g = c.gameObject.GetComponent<Grabbable>();
                    if (_planted || _progress < 1f)
                    {
                        _grabbable.body.velocity *= -1;
                        break;
                    }
                    
                    // make the plant no longer grabbable
                    _grabbable = g;
                    _grabbable.PlantObject(gameObject);
                    _grabbable.body.position = transform.position;

                    // make the plant start growing
                    _plant = c.gameObject.GetComponent<Plant>();
                    _plant.StartGrowing();
                    
                    // set planted to true and reset digging progress
                    _doneGrowing = false;
                    _planted = true;
                    _progress = 0f;
                    _meshFilter.mesh = progress0;
                    break;
            }
        }
        
        private void OnTriggerExit(Collider c)
        {
            Debug.LogWarningFormat("exited collision with " + c.gameObject.name);
            
            switch (c.gameObject.tag)
            {
                case "Hoe":
                    UpdateSoil();
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
                _meshFilter.mesh = progress0;
            }
            else if (_progress < 0.25f)
            {
                _meshFilter.mesh = progress25;
            }
            else if (_progress < 0.5f)
            {
                _meshFilter.mesh = progress50;
            }
            else if (_progress < 0.75f)
            {
                _meshFilter.mesh = progress75;
            }
            else
            {
                _meshFilter.mesh = progress100;
            }
        }
    }
}