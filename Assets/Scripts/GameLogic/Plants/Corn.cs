namespace GameLogic.Plants
{
    public class Corn : Plant
    {
        public Corn()
        {
            PlantName = "Corn";
            stageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}