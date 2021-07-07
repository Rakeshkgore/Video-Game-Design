using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class treeSoundEffect : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip swayAudio;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Swaying()
    {
        audioSource.PlayOneShot(swayAudio, 1f);
    }

}
