using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tiling
{
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

        internal float Progress = 0f;

        internal bool IsPlanted = false;
        private GameObject _plant;

        void Start()
        {
            gameObject.tag = "FarmLand";
        }

        // ???: simulate dirt getting out of the hole as a single object
        // then use force grab on all the dirt at once to fill back the hole
        // private void FixedUpdate()
        // {
        //     if (Progress >= 1f)
        //     {
        //         
        //     }
        // }

        internal void UpdateSoil()
        {
            if (Progress == 0f)
            {
                _meshFilter.mesh = progress0;
            }
            else if (Progress < 0.25f)
            {
                _meshFilter.mesh = progress25;
            }
            else if (Progress < 0.5f)
            {
                _meshFilter.mesh = progress50;
            }
            else if (Progress < 0.75f)
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