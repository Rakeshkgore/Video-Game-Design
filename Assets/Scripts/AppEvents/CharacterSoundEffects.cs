using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(RootMotionControlScript))]
public class CharacterSoundEffects : MonoBehaviour
{
    AudioSource audioSource;
    RootMotionControlScript character;
    public AudioClip footstepAudio;
    public AudioClip swipeAudio;
    public AudioClip myAudio;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<RootMotionControlScript>();
    }

    public void PlayFootstep()
    {
        if (character.IsGrounded)
        {
            audioSource.PlayOneShot(footstepAudio, 1f);
        }
    }

    public void PlayWeaponSwipe()
    {
        audioSource.PlayOneShot(swipeAudio, 1f);
    }

    public void PlayAudio()
    {
        audioSource.PlayOneShot(myAudio, 1f);
    }
}
