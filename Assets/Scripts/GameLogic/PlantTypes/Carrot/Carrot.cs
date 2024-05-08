using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : Plant
{
    public Carrot()
    {
        PlantName = "Carrot";
        GrowTime = 100;
    }

    public override void Breed(Plant partner)
    {
        throw new System.NotImplementedException();
    }
}