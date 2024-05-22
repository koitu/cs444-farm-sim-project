using System;
using GameLogic.Plants;
using Grab;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public AudioSource audioSource;

        private MeshFilter _meshFilter;  // mesh shape
        private MeshRenderer meshRenderer;  // mesh material

        private float _progress;
        
        private int _numPlant;
        private bool _doneGrowing;
        private Plant _plant;
        private Plant _extraPlant;

        void Start()
        {
            _meshFilter = gameObject.GetComponent<MeshFilter>();
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
                    if (_numPlant > 0 || _progress > 1f) break;

                    audioSource.Play();
                    Grabbable hoe = c.gameObject.GetComponent<Grabbable>();
                    float rot = hoe.GetRotation().magnitude;
                    
                    // expert BeatSaber players can get up to 20pi Rad/s
                    // 8pi Rad/s for great (+0.55)
                    // 4pi Rad/s for good (+0.4)
                    // 2pi Rad/s for ok (+0.3)
                    // otherwise (+0.2)
                    // Debug.LogWarningFormat("rot: {0}", rot);
                    if (rot > 4 * Math.PI)
                    {
                        _progress += 0.61f;
                    }
                    else if (rot > 2 * Math.PI)
                    {
                        _progress += 0.31f;
                    }
                    else
                    {
                        _progress += 0.16f;
                    }
                    UpdateSoil();
                    break;
                
                case "Plant":
                    if (_numPlant > 0 || _progress < 1f)
                    {
                        // only reject plants that are traveling down
                        // previous issues:
                        // - plant that just finished growing is rejected
                        // - when pulling a grown plant it is rejected (velocity is reset)
                        Plant plant = c.gameObject.GetComponent<Plant>();
                        if (plant == _plant ||
                            plant == _extraPlant ||
                            plant.Body.velocity.y > 0f) break;
                        
                        // reject the plant if there is already a plant or the ground is not tiled enough
                        Vector3 direction = Random.insideUnitSphere;
                        direction.y = 1; // about 45 degrees
                        
                        c.attachedRigidbody.rotation = Quaternion.LookRotation(direction);
                        c.attachedRigidbody.velocity = direction * 1.8f;
                        // TODO: play a rejection noise
                    }
                    else
                    {
                        // accept the plant
                        c.attachedRigidbody.isKinematic = true; // this will be unset when grab the object
                        c.transform.position = transform.position;
                        c.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                        
                        _plant = c.gameObject.GetComponent<Plant>();
                        
                        // set the plant to ignore grabbing and start growing
                        _plant.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        _plant.Body.detectCollisions = false;
                        _plant.StartGrowing(this);
                        
                        // set planted to true and reset digging progress
                        _numPlant = 1;
                        _doneGrowing = false;
                        _progress = 0f;
                        _meshFilter.mesh = progress0;
                    }
                    break;
            }
        }

        internal void PlantDoneGrowingCallback()
        {
            _extraPlant = Instantiate(
                _plant,
                transform.position,
                transform.rotation,
                transform.parent);
            
            _extraPlant.Body.isKinematic = true;
            _extraPlant.gameObject.layer = LayerMask.NameToLayer("Grabbable");
            _extraPlant.transform.position += Vector3.down * 0.1f;   // hide the extra plant

            // set done growing to true to all us to dig out plants
            _numPlant = 2;
            _doneGrowing = true;
        }
        
        private void OnTriggerExit(Collider c)
        {
            Debug.LogWarningFormat("exited collision with " + c.gameObject.name + " with tag " + c.gameObject.tag);
            
            switch (c.gameObject.tag)
            {
                case "Hoe":
                    break;
                
                case "Plant":
                    // setting the position of a plant will cause it trigger exit and enter
                    // so we wait until we are sure we are done growing
                    if (!_doneGrowing) break;

                    Plant plant = c.gameObject.GetComponent<Plant>();
                    if (_numPlant == 2)
                    {
                        if (_extraPlant == plant)
                        {
                            _extraPlant = null;
                            _numPlant = 1;
                            
                            _plant.ResetPlant();
                            _plant.Body.detectCollisions = true;
                            _plant.gameObject.layer = LayerMask.NameToLayer("Grabbable");
                            
                            _meshFilter.mesh = progress50;  // reveal the plant
                        }
                    }
                    else if(_numPlant == 1)
                    {
                        if (_plant == plant)
                        {
                            _plant = null;
                            _numPlant = 0;
                            _doneGrowing = false;

                            _meshFilter.mesh = progress0;
                        }
                    }
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

        public void SetProgress(float newProgress)
        {
            this._progress = newProgress;
            this.UpdateSoil();
        }
    }
}