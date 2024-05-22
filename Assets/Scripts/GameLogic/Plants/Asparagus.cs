namespace GameLogic.Plants
{
    public class Asparagus : Plant
    {
        public Asparagus()
        {
            PlantName = "Asparagus";
            stageGrowTime = 42;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}
