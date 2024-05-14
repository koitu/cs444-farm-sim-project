using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastStepTeleporter : MonoBehaviour
{

    public Vector3 teleportDestination;

    private ScreenFader screenFader;
    private CharacterController characterController;
    private MainPlayerController mainPlayerController;

    public float gizmosSize = 0.3f;

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
            this.screenFader.FadeToBlack(1f);
            this.mainPlayerController.detachClimbingStep(this.mainPlayerController.climbingHand, false);
            this.characterController.enabled = false;
            this.characterController.transform.position = teleportDestination;
            this.characterController.enabled = true;
            this.mainPlayerController.bringBackGravity();
            this.screenFader.FadeToClear(1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(teleportDestination, new Vector3(gizmosSize, gizmosSize, gizmosSize));
    }
}
