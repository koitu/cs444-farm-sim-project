using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public Plant onion;
    public FieldSlotManager[] fieldSlots = null;
    // Start is called before the first frame update
    void Start()
    {
        fieldSlots = this.GetComponentsInChildren<FieldSlotManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("AnyKeyDown. Looking for a slot.");
            for (int i = 0; i < this.fieldSlots.Length; i++)
            {
                Debug.Log("Checking fieldSlot " + i);
                if (this.fieldSlots[i].isFree() && this.onion)
                {
                    this.fieldSlots[i].assignPlant(this.onion);
                    Debug.Log("FieldManager: PLANT ASSIGNED TO SLOT " + i);
                    return;
                }
            }
        }
    }
}
