using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSlotManager : MonoBehaviour
{

    public Plant plant = null;

    public bool isFree()
    {
        return plant == null;
    }

    public void assignPlant(Plant plant)
    {
        this.plant = Instantiate(plant, this.transform.position + new Vector3(0, 0.2f, 0), this.transform.rotation, this.transform);
        this.plant.StartGrowing();
    }

    float timePassed = 0f;
    void Update()
    {
        if (this.plant && this.plant.isGrowing)
        {
            timePassed += Time.deltaTime;
            if (timePassed > 0.5f)
            {
                this.plant.GrowingStep();
                Debug.Log(this.plant.ToString());
                timePassed = 0f;
            }
        }    
    }
}
