using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif

public class CloudAnimationHelper : MonoBehaviour
{
    public float animationSpeed = 1f;
    public float animationOffset = 0.4f;
    private int timeEndCurve = 100;

    void Start()
    {
        #if UNITY_EDITOR
        // Create a new animation clip
        AnimationClip clip = new AnimationClip();
        clip.legacy = false; // Ensure it's not a legacy clip
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        // Create the curve to animate the transform.x
        AnimationCurve curveX = AnimationCurve.EaseInOut(0, 480, timeEndCurve, -480); // Example values
        AnimationCurve curveY = AnimationCurve.EaseInOut(0, transform.position.y, timeEndCurve, transform.position.y); // Example values
        AnimationCurve curveZ = AnimationCurve.EaseInOut(0, transform.position.z, timeEndCurve, transform.position.z); // Example values

        // Set the curve for transform.x
        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

        // Create an Animator Controller
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/Cloud/CloudAnimator.controller");

        // Add the clip to the controller
        var rootStateMachine = animatorController.layers[0].stateMachine;
        var state = rootStateMachine.AddState("MoveX");
        state.motion = clip;
        state.cycleOffset = animationOffset;

        // Assign the controller to the Animator component
        Animator animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController as RuntimeAnimatorController;
        animator.speed = animationSpeed / 100;
        #endif
    }
}
