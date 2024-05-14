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
    private bool is_previous_hand_closed;
    private StepManager stepManager;

    private void Start()
    { 
        this.playerController = FindObjectOfType<MainPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (this.closeSteps.Count == 0) return;

        bool hand_closed = is_hand_closed();

        if (hand_closed == this.is_previous_hand_closed) return;
        
        this.is_previous_hand_closed = hand_closed;
        if (hand_closed)
        {
            grabStep();
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
        Debug.LogWarning(this.handType + " | CLOSE TO SOMETHING WITH TAG: " + other.gameObject.tag);
        if (other.gameObject.tag == "LadderStep" && closeSteps.IndexOf(other.gameObject) == -1)
        {
            this.closeSteps.Add(other.gameObject);
            this.stepManager = other.GetComponent<StepManager>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.LogWarning(this.handType + " | NOT CLOSE ANYMORE TO SOMETHING WITH TAG: " + other.gameObject.tag);
        if (other.gameObject.tag == "LadderStep" && closeSteps.IndexOf(other.gameObject) != -1)
        {
            this.closeSteps.Remove(other.gameObject);
            if (other.GetComponent<StepManager>() == this.stepManager)
            {
                releaseStep();
                this.stepManager = null;
            }
        }
    }
}
