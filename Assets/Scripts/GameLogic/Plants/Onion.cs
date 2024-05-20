using UnityEngine;

namespace GameLogic.Plants
{
    public class Onion : Plant
    {
        public Onion()
        {
            PlantName = "Onion";
            StageGrowTime = 5;
            Debug.Log("Onion created");
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}
