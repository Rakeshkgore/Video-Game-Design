using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RhinoSoundEffects : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip attackAudio;
    public AudioClip shoutAudio;
    public AudioClip footstepAudio;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAttack()
    {
        audioSource.PlayOneShot(attackAudio, 1f);
    }

    public void PlayShout()
    {
        audioSource.PlayOneShot(shoutAudio, 1f);
    }

    public void PlayFootStep()
    {
        audioSource.PlayOneShot(footstepAudio, 1f);
    }

}
