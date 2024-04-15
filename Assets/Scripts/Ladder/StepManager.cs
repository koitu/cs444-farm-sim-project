using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{

    [Header("Hint Element")]
    public GameObject hintSphere;


    private float distanceToHand = Mathf.Infinity;
    private float radiusOfHint = 0f;

    private float maxDistance = 0.5f;
    private float minDistance = 0f;

    private float maxRadius = 0.12f;
    private float minRadius = 0f;

    private HandStepManager attachedManager;
    private bool binded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (attachedManager)
        {
            radiusOfHint = Mathf.Lerp(maxRadius, minRadius, distanceToHand / maxDistance);
            hintSphere.transform.localScale = new Vector3(radiusOfHint, radiusOfHint, radiusOfHint);
        }
    }

    public void attach_to(HandStepManager handStepManager)
    {
        if (!this.attachedManager)
        {
            this.attachedManager = handStepManager;
        }

    }

    public void detach_from(HandStepManager handStepManager)
    {
        this.attachedManager = null;
        this.distanceToHand = Mathf.Infinity;
        this.radiusOfHint = 0f;
    }

    public void calcDistanceToStep(Transform transform)
    {
        this.distanceToHand = Vector3.Distance(this.transform.position, transform.position);
    }
}
