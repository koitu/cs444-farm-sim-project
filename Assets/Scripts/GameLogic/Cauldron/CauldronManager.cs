using System.Collections;
using System.Collections.Generic;
using GameLogic.Plants;
using UnityEngine;

namespace GameLogic.Cauldron
{
    public class CauldronManager : MonoBehaviour
    {
        private Plant plant1 = null;
        private Plant plant2 = null;

        private List<Plant> result = null;
        public Plant[] possiblePlantResults = new Plant[9];

        public ParticleSystem particles;

        public int breedTimeMin = 5;
        public int breedTimeMax = 10;
        private int breedTimeLeft = 0;

        public bool isBreeding = false;
        public bool consumeSeeds = true;

        public AudioSource audioSource;

        // public Plant onion;
        
        // place the resulting plant at a offset from the container position
        private readonly Vector3 _resultPositionOffset = new Vector3(0f, 0.25f, 0f);
        private Vector3 GetResultPosition()
        {
            return transform.position + transform.right + _resultPositionOffset;
        }

        /**
         * Checks if there is space for breeding in the cauldron.
         */
        public bool IsFree()
        {
            return !plant1 || !plant2;
        }

        /**
         * Assigns a plant to the available slot if there is one.
         * In case there isn't, it pushes the plant away.
         */
        public int assignPlant(Plant plant)
        {
            if (this.plant1 == null)
            {
                Debug.Log("Assigning plant 1");
                this.plant1 = plant;
                return 0;
            }
            if (this.plant2 == null)
            {
                Debug.Log("Assigning plant 2");
                this.plant2 = plant;
                startBreed();
                return 0;
            }
            Debug.Log("Throwing away the plant");
            Vector3 throwingVector = new Vector3(Random.Range(-1f, 1f), 2, Random.Range(-1f, 1f));
            float throwStrength = 5;
            Vector3 force = throwingVector.normalized * throwStrength;

            plant.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            return 1;
        }

        /**
         * Calling this starts the breeding process in case it wasn't started already.
         */
        public void startBreed()
        {
            if (this.isBreeding)
            {
                return;
            }
            this.isBreeding = true;
            this.particles.Play();
            this.breedTimeLeft = Random.Range(this.breedTimeMin, this.breedTimeMax);
            this.audioSource.Play();
            Debug.Log("Starting Breeding between " + this.plant1.PlantName + " & " + this.plant2.PlantName + " with time: " + this.breedTimeLeft);
        }

        /**
         * Auxiliar function to instantiate the result plants.
         */
        IEnumerator InstantiateObjects(List<Plant> plants)
        {
            Instantiate(result[0], GetResultPosition(), Quaternion.identity);
            yield return new WaitForSeconds(1f);
        
            if (result.Count > 1)
            {
                result[1].transform.position = GetResultPosition();
                result[1].transform.rotation = Quaternion.identity;
                this.plant1 = null;
                yield return new WaitForSeconds(1f);

                result[2].transform.position = GetResultPosition();
                result[2].transform.rotation = Quaternion.identity;
                this.plant2 = null;
                yield return new WaitForSeconds(1f);
            }
        }

        /**
         * Ends the breeding process and returns the list of result plants.
         */
        public List<Plant> endBreed()
        {
            this.isBreeding = false;
            this.particles.Stop();
            this.audioSource.Stop();

            this.breedTimeLeft = 0;
            if (this.possiblePlantResults == null || this.possiblePlantResults.Length == 0)
            {
                return null;
            }

            List<Plant> returnArray = new List<Plant>();

            returnArray.Add(this.possiblePlantResults[Random.Range(0, this.possiblePlantResults.Length)]);

            if (!this.consumeSeeds)
            {
                Debug.Log("NOT CONSUMING SEEDS");
                returnArray.Add(this.plant1);
                returnArray.Add(this.plant2);
            } else
            {
                this.plant1.Destroy();
                this.plant2.Destroy();
            }

            return returnArray;
        }

        /**
         * Detects and stores when a plant enters the cauldron.
         */
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter with tag " + other.gameObject.tag);
            if (!other.gameObject.CompareTag("Plant")) return;
        
            if (other.GetComponent<Plant>())
            {
                assignPlant(other.GetComponent<Plant>());
            } else
            {
                other.transform.position = GetResultPosition();
            }
        }

        float timePassed = 0f;
        void Update()
        {
            // Debugging without VR headset
            if (Input.GetKeyDown(KeyCode.B))
            {
                Plant breedingPlant1 = 
                    Instantiate(
                        possiblePlantResults[6],
                        new Vector3( transform.position.x, transform.position.y + 1.5f, transform.position.z),
                        Quaternion.identity,
                        transform.parent);
            }

            if (!this.isBreeding) return;
        
            timePassed += Time.deltaTime;
            if (timePassed > 1f)
            {
                this.breedTimeLeft--;
                //Debug.Log("Breeding step done, time left: " + this.breedTimeLeft);
                timePassed = 0f;
                if (this.breedTimeLeft == 0)
                {
                    this.result = this.endBreed();
                    StartCoroutine(InstantiateObjects(this.result));
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(GetResultPosition(), 0.3f);
        }
    }
}
