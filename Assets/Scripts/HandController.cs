using UnityEngine;

// Boilerplate code for interactions that want to use controller button presses
// https://cs444-practice.epfl.ch/tp/tp3/img/interaction/raw_mapping.png
public class HandController : MonoBehaviour
{
	// store the hand type to know which button should be pressed
	public enum HandType : int { LeftHand, RightHand };
	[Header( "Hand Properties" )]
	public HandType handType;
	

	// get how much index trigger is activated
	internal float index_trigger_pressed()
	{
		return OVRInput.Get(handType == HandType.LeftHand ?
			OVRInput.RawAxis1D.LIndexTrigger : OVRInput.RawAxis1D.RIndexTrigger);
	}
	
	// get how much hand trigger is activated
	internal float hand_trigger_pressed()
	{
		return OVRInput.Get(handType == HandType.LeftHand ?
			OVRInput.RawAxis1D.LHandTrigger : OVRInput.RawAxis1D.RHandTrigger);
	}
	
	// check if the near button is being pressed (X for left and A for right)
	internal bool near_button_pressed()
	{
		return OVRInput.Get(handType == HandType.LeftHand ?
			OVRInput.RawButton.X : OVRInput.RawButton.A);
	}
	
	// check if the far button is being pressed (Y for left and B for right)
	internal bool far_button_pressed()
	{
		return OVRInput.Get(handType == HandType.LeftHand ?
			OVRInput.RawButton.Y : OVRInput.RawButton.B);
	}
	
	// check if the thumbstick is being pressed
	internal bool thumbstick_button_pressed()
	{
		return OVRInput.Get(handType == HandType.LeftHand ?
			OVRInput.RawButton.LThumbstick : OVRInput.RawButton.RThumbstick);
	}
}