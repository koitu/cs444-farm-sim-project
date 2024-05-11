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

    private bool isCloseToStep;
    private bool is_previous_hand_closed;

    private void Start()
    { 
        this.playerController = FindObjectOfType<MainPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!this.isCloseToStep) return;


        bool hand_closed = is_hand_closed();

        if (hand_closed == this.is_previous_hand_closed) return;
        
        // If it's here it's because something changed
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
        // Case of a left hand
        if (this.handType == HandType.LeftHand) return
            OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5;   // Check that the index finger is pressing

        // Case of a right hand
        else return
            OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5; // Check that the index finger is pressing
    }   


    public void grabStep()
    {
        Debug.LogWarning(this.handType + "| HandStepManager STARTING grabbing");
        this.playerController.attachClimbingStep(this);
    }

    public void releaseStep()
    {
        Debug.LogWarning(this.handType + "| HandStepManager RELEASED grabbing");
        this.playerController.detachClimbingStep(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("CLOSE TO SOMETHING WITH TAG: " + other.gameObject.tag);
        this.isCloseToStep = other.gameObject.tag == "LadderStep";
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.LogWarning("NOT CLOSE ANYMORE TO SOMETHING WITH TAG: " + other.gameObject.tag);
        if (other.gameObject.tag == "LadderStep")
        {
            this.isCloseToStep = false;
            releaseStep();
        }
    }
}
