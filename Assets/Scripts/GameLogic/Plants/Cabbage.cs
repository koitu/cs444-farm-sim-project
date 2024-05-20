namespace GameLogic.Plants
{
    public class Cabbage : Plant
    {
        public Cabbage()
        {
            PlantName = "Cabbage";
            StageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}
