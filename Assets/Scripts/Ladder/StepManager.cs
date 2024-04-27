using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    private HandStepManager attachedManager;
    private bool binded;

    public void attach_to(HandStepManager handStepManager)
    {
        if (!this.attachedManager)
        {
            this.attachedManager = handStepManager;
        }

    }

    public void detach_from(HandStepManager handStepManager)
    {
        if (this.attachedManager == handStepManager) this.attachedManager = null;
    }
}
