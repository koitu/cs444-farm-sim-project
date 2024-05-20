namespace GameLogic.Plants
{
    public class Carrot : Plant
    {
        public Carrot()
        {
            PlantName = "Carrot";
            StageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}