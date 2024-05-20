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
        public Plant[] possiblePlantResults = null;

        public ParticleSystem particles;

        public int breedTimeMin = 5;
        public int breedTimeMax = 10;
        private int breedTimeLeft = 0;

        public bool isBreeding = false;
        public bool consumeSeeds = true;

        public Plant onion;
        public Vector3 resultPosition;

        public bool isFree()
        {
            return !plant1 || !plant2;
        }

        public int assignPlant(Plant plant)
        {
            if (this.plant1 == null)
            {
                this.plant1 = plant;
                return 0;
            }
            if (this.plant2 == null)
            {
                this.plant2 = plant;
                startBreed();
                return 0;    
            }
            return 1;
        }

        public void startBreed()
        {
            if (this.isBreeding)
            {
                return;
            }
            this.isBreeding = true;
            this.particles.Play();
            this.breedTimeLeft = Random.Range(this.breedTimeMin, this.breedTimeMax);
            Debug.Log("Starting Breeding between " + this.plant1.PlantName + " & " + this.plant2.PlantName + " with time: " + this.breedTimeLeft);
        }

        IEnumerator InstantiateObjects(List<Plant> plants)
        {
            Instantiate(result[0], resultPosition, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        
            if (result.Count > 1)
            {
                result[1].transform.position = resultPosition;
                result[1].transform.rotation = Quaternion.identity;
                yield return new WaitForSeconds(1f);

                result[2].transform.position = resultPosition;
                result[2].transform.rotation = Quaternion.identity;
                yield return new WaitForSeconds(1f);
            }
        }

        public List<Plant> endBreed()
        {
            this.isBreeding = false;
            this.particles.Stop();

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

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter with tag " + other.gameObject.tag);
            if (!other.gameObject.CompareTag("Plant")) return;
        
            if (isFree())
            {
                assignPlant(other.GetComponent<Plant>());
            }
        }

        float timePassed = 0f;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                Plant breedingPlant1 = Instantiate(onion, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Quaternion.identity, transform.parent);
            }

            if (!this.isBreeding) return;
        
            timePassed += Time.deltaTime;
            if (timePassed > 1f)
            {
                this.breedTimeLeft--;
                Debug.Log("Breeding step done, time left: " + this.breedTimeLeft);
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
            Gizmos.DrawSphere(resultPosition, 0.3f);
        }
    }
}
