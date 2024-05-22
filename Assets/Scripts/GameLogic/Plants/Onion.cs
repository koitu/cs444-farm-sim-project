using UnityEngine;

namespace GameLogic.Plants
{
    public class Onion : Plant
    {
        public Onion()
        {
            PlantName = "Onion";
            stageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}
