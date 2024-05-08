using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetroot : Plant
{
    public Beetroot()
    {
        PlantName = "Beetroot";
        GrowTime = 100;
    }

    public override void Breed(Plant partner)
    {
        throw new System.NotImplementedException();
    }
}