using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerController : MonoBehaviour
{

    public ScreenFader screenFader;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MOUSE CLICKED");
            StartCoroutine(this.screenFader.FadeInAndOut());
        }
    }
}
