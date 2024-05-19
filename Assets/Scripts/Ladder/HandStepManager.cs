using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HandController))]

public class HandStepManager : MonoBehaviour
{
    // Store the player controller to forward it to the object
    [Header("Player Controller")]
    public MainPlayerController playerController;
    public GameObject handAnchor;

    private List<GameObject> closeSteps;
    private ClimbingCheatButton cheatButton;
    private bool is_previous_hand_closed;
    private StepManager stepManager;

    private HandController _handController;

    private void Start()
    {
        _handController = GetComponent<HandController>();
        this.playerController = FindObjectOfType<MainPlayerController>();
        this.closeSteps = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.closeSteps.Count == 0 & this.cheatButton == null) return;

        bool hand_closed = _handController.index_trigger_pressed() > 0.5;

        if (hand_closed == this.is_previous_hand_closed) return;
        
        this.is_previous_hand_closed = hand_closed;
        if (hand_closed)
        {
            if (cheatButton)
            {
                _handController.medium_vibrate();
                cheatButton.triggerCheat();
            }
            else
            {
                _handController.short_vibrate();
                grabStep();
            }
        }
        else if (!hand_closed)
        {
            releaseStep();
        }
    }

    public void grabStep()
    {
        Debug.LogWarning(_handController.handType + " | HandStepManager STARTING grabbing");
        this.playerController.attachClimbingStep(this);
    }

    public void releaseStep()
    {
        Debug.LogWarning(_handController.handType + " | HandStepManager RELEASED grabbing");
        this.playerController.detachClimbingStep(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "LadderStep":
                Debug.LogWarning(_handController.handType + " | OnTriggerEnter LadderStep");
                _handController.short_vibrate();
                if (closeSteps.IndexOf(other.gameObject) == -1)
                {
                    this.closeSteps.Add(other.gameObject);
                    this.stepManager = other.GetComponent<StepManager>();
                }
                break;
            
            case "LadderButton":
                Debug.LogWarning(_handController.handType + " | OnTriggerEnter LadderButton");
                _handController.short_vibrate();
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
                Debug.LogWarning(_handController.handType + " | OnTriggerExit LadderStep");
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
                Debug.LogWarning(_handController.handType + " | OnTriggerExit LadderButton");
                this.cheatButton = null;
                break;
        }
    }
}
