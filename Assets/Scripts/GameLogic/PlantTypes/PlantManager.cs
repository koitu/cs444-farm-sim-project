using System;
using UnityEngine;

public class PlantManager : MonoBehaviour
{

    public Plant plant;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (!plant)
        {
            throw new Exception("No plant assigned.");
        }
    }
}
