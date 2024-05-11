using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.1f;

    void Start()
    {
        // Optionally start with an unfaded screen
        fadeImage.color = new Color(0, 0, 0, 0);
    }


    // Public method to trigger both fades
    public void FadeInAndOut()
    {
        // First, fade to black
        StartCoroutine(Fade(0, 0.6f));
        // Then, fade back to clear
        StartCoroutine(Fade(0.6f, 0));
    }

    // Call this method to start fading to black
    public void FadeToBlack()
    {
        StartCoroutine(Fade(0, 0.6f));
    }

    // Call this method to fade back to transparent
    public void FadeToClear()
    {
        Debug.Log("Is this even called?");
        StartCoroutine(Fade(0.6f, 0));
    }

    // General fade method
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timeElapsed = 0;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / fadeDuration);
            Debug.Log("FADING TO " + newAlpha.ToString());
            fadeImage.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, endAlpha);
    }
}
