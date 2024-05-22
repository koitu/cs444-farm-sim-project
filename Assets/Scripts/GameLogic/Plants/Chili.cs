namespace GameLogic.Plants
{
    public class Chili : Plant
    {
        public Chili()
        {
            PlantName = "Chili";
            stageGrowTime = 120;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}