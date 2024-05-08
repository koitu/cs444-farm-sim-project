using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archive
{
	// public class HandController : MonoBehaviour
	// {
	//
	// 	// Store the hand type to know which button should be pressed
	// 	public enum HandType : int { LeftHand, RightHand };
	// 	[Header("Hand Properties")]
	// 	public HandType handType;
	//
	//
	// 	// Store the player controller to forward it to the object
	// 	[Header("Player Controller")]
	// 	public MainPlayerController playerController;
	//
	//
	//
	// 	// Store the previous state of triggers to detect edges
	// 	protected bool is_hand_closed_previous_frame = false;
	//
	// 	// Store the object atached to this hand
	// 	// N.B. This can be extended by using a list to attach several objects at the same time
	// 	protected ObjectAnchor object_grasped = null;
	//
	// 	// Store all gameobjects containing an Anchor
	// 	// N.B. This list is static as it is the same list for all hands controller
	// 	// thus there is no need to duplicate it for each instance
	// 	static protected ObjectAnchor[] anchors_in_the_scene;
	//
	// 	void Start()
	// 	{
	// 		// Prevent multiple fetch
	// 		if (anchors_in_the_scene == null) anchors_in_the_scene = GameObject.FindObjectsOfType<ObjectAnchor>();
	// 	}
	//
	//
	// 	// This method checks that the hand is closed depending on the hand side
	// 	protected bool is_hand_closed()
	// 	{
	// 		if (handType == HandType.LeftHand)
	// 		{
	// 			// Check if the left middle finger is gripping
	// 			return OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) > 0.5;
	// 		}
	// 		else
	// 		{
	// 			// Check if the right middle finger is gripping
	// 			return OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.5;
	// 		}
	// 	}
	//
	//
	// 	// Automatically called at each frame
	// 	void Update()
	// 	{
	// 		bool hand_closed = is_hand_closed();
	//
	// 		if (hand_closed == is_hand_closed_previous_frame) return;
	// 		is_hand_closed_previous_frame = hand_closed;
	// 		handle_controller_behavior(hand_closed);
	// 	}
	//
	//
	// 	/// <summary>
	// 	/// This method handles the linking of object anchors to this hand controller
	// 	/// </summary>
	// 	protected void handle_controller_behavior(bool hand_closed)
	// 	{
	// 		//==============================================//
	// 		// Define the behavior when the hand get closed //
	// 		//==============================================//
	// 		if (hand_closed)
	// 		{
	//
	// 			// Log hand action detection
	// 			Debug.LogWarningFormat("{0} get closed", this.transform.parent.name);
	//
	// 			// Determine which object available is the closest from the left hand
	// 			int best_object_id = -1;
	// 			float best_object_distance = float.MaxValue;
	// 			float oject_distance;
	//
	// 			// Iterate over objects to determine if we can interact with it
	// 			for (int i = 0; i < anchors_in_the_scene.Length; i++)
	// 			{
	//
	// 				// Skip object not available
	// 				if (!anchors_in_the_scene[i].is_available()) continue;
	//
	// 				// Compute the distance to the object
	// 				oject_distance = Vector3.Distance(this.transform.position, anchors_in_the_scene[i].transform.position);
	//
	// 				// Keep in memory the closest object
	// 				// N.B. We can extend this selection using priorities
	// 				if (oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius())
	// 				{
	// 					best_object_id = i;
	// 					best_object_distance = oject_distance;
	// 				}
	// 			}
	//
	// 			// If the best object is in range grab it
	// 			if (best_object_id != -1)
	// 			{
	//
	// 				// Store in memory the object grasped
	// 				object_grasped = anchors_in_the_scene[best_object_id];
	//
	// 				// Log the grasp
	// 				Debug.LogWarningFormat("{0} grasped {1}", this.transform.parent.name, object_grasped.name);
	//
	// 				// Grab this object
	// 				object_grasped.attach_to(this);
	// 			}
	//
	//
	//
	// 			//==============================================//
	// 			// Define the behavior when the hand get opened //
	// 			//==============================================//
	// 		}
	// 		else if (object_grasped != null)
	// 		{
	// 			// Log the release
	// 			Debug.LogWarningFormat("{0} released {1}", this.transform.parent.name, object_grasped.name);
	//
	// 			// Release the object
	// 			object_grasped.detach_from(this);
	// 		}
	// 	}
	// }
}
