namespace GameLogic.Plants
{
    public class Tomato : Plant
    {
        public Tomato()
        {
            PlantName = "Tomato";
            stageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}