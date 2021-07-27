using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DescendingWallSoundEffect : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip descendingWallAudio;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDescendingWall()
    {
        audioSource.PlayOneShot(descendingWallAudio, 1f);
    }
}
