namespace GameLogic.Plants
{
    public class Beetroot : Plant
    {
        public Beetroot()
        {
            PlantName = "Beetroot";
            StageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}