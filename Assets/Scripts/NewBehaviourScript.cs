using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RhinoSoundEffects : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip shoutAudio;
    public AudioClip attackAudio;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayShout()
    {
        audioSource.PlayOneShot(shoutAudio, 1f);
    }

    public void PlayAttack()
    {
        audioSource.PlayOneShot(attackAudio, 1f);
    }
}
