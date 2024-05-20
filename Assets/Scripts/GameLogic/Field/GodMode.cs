using System.Collections;
using System.Collections.Generic;
using GameLogic.Field;
using GameLogic.Plants;
using UnityEngine;

public class GodMode : MonoBehaviour
{

    public Plant plant;
    public FarmPlot farmPlot;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            this.farmPlot.SetProgress(1f);
        }

        if (Input.GetKeyDown(KeyCode.S)){
            Vector3 plantPosition = this.transform.position + new Vector3(0, 1f, 0);

            Plant newPlant = Instantiate(plant, plantPosition, Quaternion.identity);
        }
    }
}
