using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingCheatButton : MonoBehaviour
{
    public Vector3 destinationPosition;
    public CharacterController characterController;
    public OVRPlayerController playerController;
    private float initialGravity;
    public ScreenFader screenFader;

    public float gizmosSize = 1;


    private void Start()
    {
        if (characterController == null)
        {
            characterController = FindObjectOfType<CharacterController>();
        }
        if (playerController == null)
        {
            playerController = FindObjectOfType<OVRPlayerController>();
        }
        initialGravity = playerController.GravityModifier;

        if (screenFader == null)
        {
            screenFader = FindObjectOfType<ScreenFader>();
        }   
    }

    public void triggerCheat()
    {
        Debug.LogWarning("Triggering CheatButton");
        screenFader.FadeToBlack(1);
        playerController.GravityModifier = 0;
        this.characterController.enabled = false;
        this.characterController.transform.position = destinationPosition;
        this.characterController.enabled = true;
        playerController.GravityModifier = initialGravity;
        screenFader.FadeToClear(1);
    }

    void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
        Gizmos.DrawCube(destinationPosition, new Vector3(gizmosSize, gizmosSize, gizmosSize));
	}
}
