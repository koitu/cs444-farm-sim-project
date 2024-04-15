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

    private StepManager attachedStep;
    private bool grabbing;

    private bool is_previous_hand_closed;

    private Transform initial_parent_transform;

    private float initial_gravity_modifier;

    private Transform playerTransform;

    private Vector3 initialGrabPosition;

    private OVRPlayerController OVRPlayerController;

    private CharacterController characterController;

    private void Start()
    {
        this.initial_parent_transform = this.handAnchor.transform.parent;
        this.OVRPlayerController = FindObjectOfType<OVRPlayerController>();
        this.initial_gravity_modifier = OVRPlayerController.GravityModifier;
        this.playerTransform = FindObjectOfType<OVRCameraRig>().transform;
        this.characterController = FindObjectOfType<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!attachedStep) return;

        attachedStep.calcDistanceToStep(this.transform);

        if (grabbing)
        {
            // Calculate the offset in position from when the grab started
            Vector3 currentOffset = this.transform.position - initialGrabPosition;
            Debug.LogWarning("CURRENT OFFSET: " + currentOffset);
            // Move the OVRCameraRig in the opposite direction of the offset
            characterController.Move(currentOffset * -1);
            //playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y - currentOffset.y, playerTransform.position.z);

            // Update the initial grab position to the new controller position
            initialGrabPosition = transform.position;
        }


        bool hand_closed = is_hand_closed();

        if (hand_closed == is_previous_hand_closed) return;
        
        // If it's here it's because something changed
        is_previous_hand_closed = hand_closed;
        if (hand_closed)
        {
            grabStep();
        }
        else
        {
            releaseStep();
        }
    }

    protected bool is_hand_closed()
    {
        // Case of a left hand
        if (handType == HandType.LeftHand) return
            OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5;   // Check that the index finger is pressing

        // Case of a right hand
        else return
            OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5; // Check that the index finger is pressing
    }   


    protected void grabStep()
    {
        Debug.LogWarning("GRABBING THE STEP " + attachedStep.name);
        this.grabbing = true;
        this.initialGrabPosition = this.transform.position;
        //OVRPlayerController.GravityModifier = 0;
        //OVRPlayerController.HmdResetsY = false;
    }

    protected void releaseStep()
    {
        Debug.LogWarning("RELEASING THE STEP " + attachedStep.name);
        this.grabbing = false;
        //OVRPlayerController.GravityModifier = initial_gravity_modifier;
        //OVRPlayerController.HmdResetsY = true;
        // this.handAnchor.transform.SetParent(this.initial_parent_transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("HAND MANAGER IS TRIGGERING WITH " + other.gameObject.name);
        StepManager otherStepManager = other.gameObject.GetComponent<StepManager>();
        if (otherStepManager)
        {
            otherStepManager.attach_to(this);
            if (this.attachedStep)
            {
                this.attachedStep.detach_from(this);
            }
            this.attachedStep = otherStepManager;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StepManager otherStepManager = other.gameObject.GetComponent<StepManager>();
        if (otherStepManager)
        {
            otherStepManager.detach_from(this);
            if (this.attachedStep)
            {
                this.attachedStep = null;
                releaseStep();
            }
        }
    }
}
