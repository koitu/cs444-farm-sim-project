using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandStepManager : MonoBehaviour
{
    public enum HandType : int { LeftHand, RightHand };
    [Header("Hand Properties")]
    public HandType handType;

    // Store the player controller to forward it to the object
    [Header("Player Controller")]
    public MainPlayerController playerController;
    public GameObject handAnchor;

    private List<GameObject> closeSteps;
    private ClimbingCheatButton cheatButton;
    private bool is_previous_hand_closed;
    private StepManager stepManager;

    private void Start()
    { 
        this.playerController = FindObjectOfType<MainPlayerController>();
        this.closeSteps = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.closeSteps.Count == 0 & this.cheatButton == null) return;

        bool hand_closed = is_hand_closed();

        if (hand_closed == this.is_previous_hand_closed) return;
        
        this.is_previous_hand_closed = hand_closed;
        if (hand_closed)
        {
            if (cheatButton) cheatButton.triggerCheat();
            else grabStep();
        }
        else if (!hand_closed)
        {
            releaseStep();
        }
    }

    protected bool is_hand_closed()
    {
        // // Case of a left hand
        // if (this.handType == HandType.LeftHand) return
        //     OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5;   // Check that the index finger is pressing
        //
        // // Case of a right hand
        // else return
        //     OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5; // Check that the index finger is pressing
        
		return OVRInput.Get(handType == HandType.LeftHand ?
			OVRInput.RawAxis1D.LIndexTrigger : OVRInput.RawAxis1D.RIndexTrigger) > 0.5;
    }   


    public void grabStep()
    {
        Debug.LogWarning(this.handType + " | HandStepManager STARTING grabbing");
        this.playerController.attachClimbingStep(this);
    }

    public void releaseStep()
    {
        Debug.LogWarning(this.handType + " | HandStepManager RELEASED grabbing");
        this.playerController.detachClimbingStep(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "LadderStep":
                Debug.LogWarning(this.handType + " | OnTriggerEnter LadderStep");
                if (closeSteps.IndexOf(other.gameObject) == -1)
                {
            this.closeSteps.Add(other.gameObject);
            this.stepManager = other.GetComponent<StepManager>();
                }
                break;
            case "LadderButton":
                Debug.LogWarning(this.handType + " | OnTriggerEnter LadderButton");
                this.cheatButton = other.gameObject.GetComponent<ClimbingCheatButton>();
                Debug.LogWarning("Gathered cheatButton script " + this.cheatButton);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "LadderStep":
                Debug.LogWarning(this.handType + " | OnTriggerExit LadderStep");
                if (closeSteps.IndexOf(other.gameObject) != -1)
                {
                    this.closeSteps.Remove(other.gameObject);
                    if (other.GetComponent<StepManager>() == this.stepManager)
                    {
                        releaseStep();
                        this.stepManager = null;
                    }
                }
                break;
            case "LadderButton":
                Debug.LogWarning(this.handType + " | OnTriggerExit LadderButton");
                this.cheatButton = null;
                break;
        }
    }
}
