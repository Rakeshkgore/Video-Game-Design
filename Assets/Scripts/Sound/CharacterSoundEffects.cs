using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSoundEffects : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip footstepAudio;
    public AudioClip swipeAudio;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFootstep()
    {
        audioSource.PlayOneShot(footstepAudio, 1f);
    }

    public void PlayWeaponSwipe()
    {
        audioSource.PlayOneShot(swipeAudio, 1f);
    }
}
