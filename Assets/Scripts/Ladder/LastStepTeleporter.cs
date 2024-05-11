using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastStepTeleporter : MonoBehaviour
{

    public Vector3 teleportOffset;

    private ScreenFader screenFader;
    private CharacterController characterController;
    private MainPlayerController mainPlayerController;

    private void Start()
    {
        this.characterController = FindObjectOfType<CharacterController>();
        this.mainPlayerController = FindObjectOfType<MainPlayerController>();
        this.screenFader = FindObjectOfType<ScreenFader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.mainPlayerController.climbingHand != null)
        {
            Debug.LogWarning("HELLO:)");
            this.screenFader.FadeToBlack(1f);
            this.mainPlayerController.detachClimbingStep(this.mainPlayerController.climbingHand, false);
            Debug.LogWarning("PREVIOUS POSITION:" + this.characterController.transform.position);
            this.characterController.Move(this.teleportOffset); //transform.position += this.teleportOffset;
            Debug.LogWarning("POSTERIOR POSITION:" + this.characterController.transform.position);
            this.mainPlayerController.bringBackGravity();
            this.screenFader.FadeToClear(1f);
            Debug.LogWarning("BYEE :(");
        }
    }
}
