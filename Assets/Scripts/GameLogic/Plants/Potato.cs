namespace GameLogic.Plants
{
    public class Potato : Plant
    {
        public Potato()
        {
            PlantName = "Potato";
            stageGrowTime = 130;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}
