namespace GameLogic.Plants
{
    public class Carrot : Plant
    {
        public Carrot()
        {
            PlantName = "Carrot";
            stageGrowTime = 112;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}