namespace GameLogic.Plants
{
    public class Potato : Plant
    {
        public Potato()
        {
            PlantName = "Potato";
            StageGrowTime = 100;
        }

        public override void Breed(Plant partner)
        {
            throw new System.NotImplementedException();
        }
    }
}
