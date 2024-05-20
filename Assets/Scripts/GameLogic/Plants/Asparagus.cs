namespace GameLogic.Plants
{
    public class Asparagus : Plant
    {
        public Asparagus()
        {
            PlantName = "Asparagus";
            StageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}
