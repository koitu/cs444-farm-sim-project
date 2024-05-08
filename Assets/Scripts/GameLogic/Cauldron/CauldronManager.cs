using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            returnArray.Add(this.plant1);
            returnArray.Add(this.plant2);
        }

        return returnArray;
    }

    float timePassed = 0f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            this.plant1 = this.possiblePlantResults[Random.Range(0, this.possiblePlantResults.Length)];
            this.plant2 = this.possiblePlantResults[Random.Range(0, this.possiblePlantResults.Length)];
            this.startBreed();
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
                Debug.Log("Breeding process finished! Breeding result:");
                for (int i = 0 ; i < this.result.Count; i++)
                {
                    Debug.Log(" - " + i + ". " + this.result[i].PlantName);
                }
                Debug.Log("");
            }
        }
    }
}
