namespace GameLogic.Plants
{
    public class Chili : Plant
    {
        public Chili()
        {
            PlantName = "Chili";
            StageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}