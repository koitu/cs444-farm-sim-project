using GameLogic.Plants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Archive
{
    public class FieldSlotManager : MonoBehaviour
    {

        [FormerlySerializedAs("plantsCommon")] public Plant plant = null;

        public bool isFree()
        {
            return plant == null;
        }

        public void assignPlant(Plant plant)
        {
            this.plant = Instantiate(plant, this.transform.position + new Vector3(0, 0.2f, 0), this.transform.rotation, this.transform);
            // this.plant.StartGrowing();
        }

        float timePassed = 0f;
        void Update()
        {
            if (this.plant && this.plant.growing)
            {
                timePassed += Time.deltaTime;
                if (timePassed > 1f)
                {
                    // plant.GrowingStep();
                    Debug.Log(this.plant.ToString());
                    timePassed = 0f;
                }
            }    
        }
    }
}
