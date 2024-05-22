namespace GameLogic.Plants
{
    public class Beetroot : Plant
    {
        public Beetroot()
        {
            PlantName = "Beetroot";
            stageGrowTime = 60;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}