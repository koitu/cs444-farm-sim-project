using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
//
// public class ExtendedReach : MonoBehaviour
// {
//     public float velocityThreshold = 2;
//     public float jumpAngleInDegree = 60;
//     
//     private Rigidbody interactableRigidbody;
//
//     private RayInteractor rayInteractor;
//     private Vector3 previousPos;
//     private bool canJump = true;
//
//     void Start()
//     {
//         interactableRigidbody = GetComponent<Rigidbody>();
//     }
//
//     private void Update()
//     {
//         if(isSelected && canJump)
//         {
//             Vector3 velocity = (rayInteractor.transform.position - previousPos) / Time.deltaTime;
//             previousPos = rayInteractor.transform.position;
//
//             // Vector3.Distance(this.transform.position, interactableRigidbody.position)
//             // if(velocity.magnitude > Mathf.Log10(interactableRigidbody.mass + 10))
//             if(velocity.magnitude > velocityThreshold)
//             {
//                 // TODO: alert the player by vibrating the controller
//                 Drop();
//                 interactableRigidbody.velocity = ComputeVelocity();
//                 canJump = false;
//             }
//         }
//     }
//
//     private Vector3 ComputeVelocity()
//     {
//         Vector3 diff = rayInteractor.transform.position - this.transform.position;
//         Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);
//         float diffXZLength = diffXZ.magnitude;
//         float diffYLength = diff.y;
//
//         float angleInRadian = jumpAngleInDegree * Mathf.Deg2Rad;
//
//         float jumpSpeed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(diffXZLength, 2)
//             / (2 * Mathf.Cos(angleInRadian) * Mathf.Cos(angleInRadian) * (diffXZ.magnitude * Mathf.Tan(angleInRadian) - diffYLength)));
//
//         Vector3 jumpVelocityVector = diffXZ.normalized * Mathf.Cos(angleInRadian) * jumpSpeed + Vector3.up * Mathf.Sin(angleInRadian) * jumpSpeed;
//
//         return jumpVelocityVector;
//     }
//
//     public void select_object()
//     {
//         if(args.interactorObject is XRRayInteractor)
//         {
//             trackPosition = false;
//             trackRotation = false;
//             throwOnDetach = false;
//
//             rayInteractor = (XRRayInteractor)args.interactorObject;
//             previousPos = rayInteractor.transform.position;
//             canJump = true;
//         }
//         else
//         {
//             trackPosition = true;
//             trackRotation = true;
//             throwOnDetach = true;
//         }
//
//         base.OnSelectEntered(args);
//     }
// }
