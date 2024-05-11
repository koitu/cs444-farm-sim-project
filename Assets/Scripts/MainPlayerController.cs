using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerController : MonoBehaviour
{

    // Climbing stuff
    public StepManager climbingStep;
    public HandStepManager climbingHand;

    public CharacterController characterController;
    public OVRPlayerController OVRPlayerController;

    private float initialGravity;
    private Vector3 previousGrabPosition;

    private void Start()
    {
        this.initialGravity = this.OVRPlayerController.GravityModifier;
    }

    private void Update()
    {
        if (this.climbingHand == null)
        {
            return;
        } 
         
        // Calculate the offset in position from when the grab started
        Vector3 currentOffset = this.climbingHand.transform.position - this.previousGrabPosition;
        // Move the OVRCameraRig in the opposite direction of the offset
        this.characterController.Move(currentOffset * -1);
        //playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y - currentOffset.y, playerTransform.position.z);

        // Update the initial grab position to the new controller position
        this.previousGrabPosition = this.climbingHand.transform.position;
        
    }

    public void attachClimbingStep(HandStepManager handStepManager)
    {
        this.previousGrabPosition = handStepManager.transform.position;
        this.OVRPlayerController.GravityModifier = 0;
        
        this.climbingHand = handStepManager;
        Debug.LogWarning("MainPlayerController got attached");
    }

    public void detachClimbingStep(HandStepManager handStepManager)
    {
        if (this.climbingHand == handStepManager)
        {
            this.OVRPlayerController.GravityModifier = this.initialGravity;
            this.climbingHand = null;
            Debug.LogWarning("MainPlayerController got detached");
        }
    }
}
