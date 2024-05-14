using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingCheatButton : MonoBehaviour
{
    public Vector3 destinationPosition;
    public CharacterController characterController;

    public float gizmosSize = 1;

    private void Start()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    public void triggerCheat()
    {
        this.characterController.transform.position = destinationPosition;
    }

    void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
        Gizmos.DrawCube(destinationPosition, new Vector3(gizmosSize, gizmosSize, gizmosSize));
	}
}
