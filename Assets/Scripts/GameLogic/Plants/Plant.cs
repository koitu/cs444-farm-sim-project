using GameLogic.Field;
using UnityEngine;

namespace GameLogic.Plants
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    
    public abstract class Plant : MonoBehaviour
    {
        private enum MeshNames {
            LStage,
            MStage,
            SStage,
            Seed,
            Default,
            Group
        }

        public Mesh growthStageL;
        public Mesh growthStageM;
        public Mesh growthStageS;
        public Mesh seed;

        public Mesh defaultStage;
        public Mesh group;

        private Rigidbody _body;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private FarmPlot _plot;

        private int _stage;
        private const int MaxStage = 3;

        private float _timePassed;
        [HideInInspector]
        public bool growing;
        [HideInInspector]
        public bool grown;

        public string PlantName { get; protected set; }
        public int StageGrowTime { get; protected set; }
        public int StageGrowTimeLeft { get; set; }

        public abstract void Breed(Plant partner);

        private void Start()
        {
            gameObject.tag = "Plant";
            _body = GetComponent<Rigidbody>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        public void StartGrowing(FarmPlot plot)
        {
            _plot = plot;
            
            // _meshCollider.enabled = false;
            // _grabbable.PlantObject(_plot.gameObject);
	        _body.isKinematic = true;
            growing = true;
            grown = false;
            
            _stage = 0;
            _timePassed = 0f;
            StageGrowTimeLeft = StageGrowTime;
            
            transform.position = _plot.transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }
    
        private void GrowStep()
        {
            StageGrowTimeLeft--;

            if (StageGrowTimeLeft > 0) return;
            StageGrowTimeLeft = StageGrowTime;

            if (_stage < MaxStage) _stage++;
            if (_stage != MaxStage) return;
            
            // _meshCollider.enabled = true;
            // _grabbable.UnPlantObject(_plot.gameObject);
            _body.isKinematic = false;
            growing = false;
            grown = true;
            
            transform.position = _plot.transform.position + Vector3.up;
        }
   //      public void PlantObject(GameObject go)
   //      {
	  //       body.isKinematic = true;
	  //       body.transform.parent = go.transform.parent;
	  //       
	  //       SetLayers(
		 //        GrabController.grabbableLayerName,
		 //        LayerMask.LayerToName(0)); // default layer (will no longer interact with GrabController)
   //
	  //       _planted = true;
	  //       _plantedBy = go;
			// Debug.LogWarningFormat("the plant has been planted");
   //      }
        
   //      public void UnPlantObject(GameObject go)
   //      {
	  //       if (go != _plantedBy) return;
   //
	  //       _planted = false;
	  //       _plantedBy = null;
   //
			// SetLayers(
		 //        LayerMask.LayerToName(0), // default layer (will no longer interact with GrabController)
		 //        GrabController.grabbableLayerName);
	  //       
	  //       body.useGravity = false;
	  //       body.isKinematic = false;
   //          body.transform.parent = _initialParent;
			// Debug.LogWarningFormat("the plant has been unplanted");
   //      }


        private void FixedUpdate()
        {
            if (!growing) return;

            _timePassed += Time.fixedDeltaTime;
            if (_timePassed < 1f) return;
            _timePassed = 0f;

            GrowStep();
            switch (_stage)
            {
                case 0:
                    SetMesh(MeshNames.Seed);
                    break;
                case 1:
                    SetMesh(MeshNames.SStage);
                    break;
                case 2:
                    SetMesh(MeshNames.MStage);
                    break;
                case 3:
                    SetMesh(MeshNames.LStage);
                    break;
                default:
                    SetMesh(MeshNames.Default);
                    break;
            }
        }
        
        private void SetMesh(MeshNames meshType)
        {
            // TODO: it appears that is the issue has something to do with changing the mesh
            switch(meshType) {
                case MeshNames.Group:
                    this._meshFilter.mesh = this.group;
                    // this._meshCollider.sharedMesh = this.group;
                    break;
                case MeshNames.Seed:
                    this._meshFilter.mesh = this.seed;
                    // this._meshCollider.sharedMesh = this.seed;
                    break;
                case MeshNames.SStage:
                    this._meshFilter.mesh = this.growthStageS;
                    // this._meshCollider.sharedMesh = this.growthStageS;
                    break;
                case MeshNames.MStage:
                    this._meshFilter.mesh = this.growthStageM;
                    // this._meshCollider.sharedMesh = this.growthStageM;
                    break;
                case MeshNames.LStage:
                    this._meshFilter.mesh = this.growthStageL;
                    // this._meshCollider.sharedMesh = this.growthStageL;
                    break;
                default:
                    this._meshFilter.mesh = this.defaultStage;
                    // this._meshCollider.sharedMesh = this.defaultStage;
                    break;
            }
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public override string ToString()
        {
            return $"{PlantName} with {StageGrowTimeLeft} days left to grow.";
        }
    }
}
