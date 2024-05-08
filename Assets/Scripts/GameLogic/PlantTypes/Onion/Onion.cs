using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onion : Plant
{
    public Onion()
    {
        PlantName = "Onion";
        GrowTime = 5;
        Debug.Log("Onion created");
    }

    public override void Breed(Plant partner)
    {
        throw new System.NotImplementedException();
    }
}
