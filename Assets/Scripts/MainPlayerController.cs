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

    private AudioSource audioSource;
    public AudioClip[] stepSound;

    private LastStepTeleporter[] lastStepTeleporters;

    private void Start()
    {
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        this.initialGravity = this.OVRPlayerController.GravityModifier;
        lastStepTeleporters = GetComponents<LastStepTeleporter>();
        this.audioSource = GetComponent<AudioSource>();
        enableLastStepTeleporters(false);
    }

    private void Update()
    {
        if (this.climbingHand == null)
        {
            return;
        }

        if (this.characterController.velocity.normalized.magnitude > 0.1f)
        {
            if (!this.audioSource.isPlaying)
            {
                this.audioSource.clip = this.stepSound[Random.Range(0, this.stepSound.Length)];
                this.audioSource.Play();
            }
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
        enableLastStepTeleporters(true);
        Debug.LogWarning("MainPlayerController got attached");
    }

    public void detachClimbingStep(HandStepManager handStepManager, bool bringBackGravity = true)
    {
        if (this.climbingHand == handStepManager)
        {
            if (bringBackGravity)
            {
                this.OVRPlayerController.GravityModifier = this.initialGravity;
            }
            enableLastStepTeleporters(false);
            this.climbingHand = null;
            Debug.LogWarning("MainPlayerController got detached");
        }
    }

    public void bringBackGravity()
    {
        this.OVRPlayerController.GravityModifier = this.initialGravity;
    }

    public void enableLastStepTeleporters(bool enable)
    {
        foreach (LastStepTeleporter element in lastStepTeleporters)
        {
            element.enabled = enable;
        }
    }
}
