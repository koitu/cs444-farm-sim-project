using GameLogic.Field;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLogic.Plants
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    
    public abstract class Plant : MonoBehaviour
    {
        // for all plants there are always 4 stages
        public GameObject[] plantStages = new GameObject[4];
        
        private Rigidbody _body;
        private FarmPlot _plot;

        private int _stageIdx;
        private GameObject _stageObj;
        private const int MaxStage = 3;

        private AudioSource _audioSource;

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
            
            _stageIdx = 0;
            _stageObj = plantStages[0];
            _audioSource = GetComponent<AudioSource>();
        }

        public void StartGrowing(FarmPlot plot)
        {
            _plot = plot;
            
            // _meshCollider.enabled = false;
            // _grabbable.PlantObject(_plot.gameObject);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	        _body.isKinematic = true;
            growing = true;
            grown = false;
            
            _stageIdx = 0;
            _stageObj.SetActive(false); // make the plant invisible when first planted
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
            this._audioSource.Play();

            if (_stageIdx < MaxStage)
            {
                _stageIdx++;
            }
            _stageObj.SetActive(false);
            _stageObj = plantStages[_stageIdx];
            _stageObj.SetActive(true);

            // we are done growing
            if (_stageIdx == MaxStage)
            {
                // _meshCollider.enabled = true;
                // _grabbable.UnPlantObject(_plot.gameObject);
                gameObject.layer = LayerMask.NameToLayer("Grabbable");
                _body.isKinematic = false;
                growing = false;
                grown = true;
            }
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
