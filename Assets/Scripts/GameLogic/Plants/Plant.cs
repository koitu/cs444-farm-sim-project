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
        
        internal Rigidbody Body;
        private FarmPlot _plot;

        private int _stageIdx;
        private GameObject _stageObj;
        private const int MaxStage = 3;

        private AudioSource _audioSource;

        private float _timePassed;
        [HideInInspector]
        public bool growing;

        public string PlantName { get; protected set; }
        
        [Range(5, 300)]
        public int stageGrowTime;
        private int _stageGrowTimeLeft;

        public abstract void Breed(Plant partner);

        public void Awake()
        {
            // Instantiate does not copy variables and we cannot wait until the next frame to call Start
            gameObject.tag = "Plant";
            Body = GetComponent<Rigidbody>();
            ResetPlant();
        }

        internal void ResetPlant()
        {
            _stageIdx = 0;
            _stageObj = plantStages[0];
            _stageObj.SetActive(true);
            for (int i = 1; i <= MaxStage; i++)
            {
                plantStages[i].SetActive(false);
            }
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void StartGrowing(FarmPlot plot)
        {
            _plot = plot;
            
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	        Body.isKinematic = true;
            growing = true;
            
            _stageIdx = 0;
            _stageObj.SetActive(false); // make the plant invisible when first planted
            _timePassed = 0f;
            _stageGrowTimeLeft = stageGrowTime;
            
            transform.position = _plot.transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }
    
        private void GrowStep()
        {
            _stageGrowTimeLeft--;

            if (_stageGrowTimeLeft > 0) return;
            _stageGrowTimeLeft = stageGrowTime;
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
                growing = false;
                _plot.PlantDoneGrowingCallback();
            }
        }

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
            return $"{PlantName} with {_stageGrowTimeLeft} days left to grow.";
        }
    }
}
