using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class AudioTriggerer : MonoBehaviour
{
    private AudioSource audioSource;
    
    public AudioClip[] audioClips = [];

    public float probabilityPerSecond = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        InvokeRepeating("TriggerEvent", 1.0f, 1.0f);
    }

    // Update is called once per frame
    private void TriggerEvent()
    {
        // Check if the random value is less than the specified probability
        if (Random.value < probabilityPerSecond && !this.audioSource.isPlaying)
        {
            // Play the audio
            if (audioClips.Length > 0)
            {
                this.audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            }
            this.audioSource.Play();
        }
    }
}
