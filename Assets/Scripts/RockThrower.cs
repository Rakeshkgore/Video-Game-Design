using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RockThrower : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip myAudio1;
    public AudioClip myAudio2;
    public AudioClip myAudio3;
    public AudioClip walkAudio;
    public Rigidbody rockPrefab;
    public Transform spawnPosition;
    public Transform parent;
    public Vector3 launchVelocity;

    private Rigidbody currRock;

    void Awake()
    {
        Debug.Assert(rockPrefab != null, "Rock prefab must not be null");
        Debug.Assert(spawnPosition != null, "Spawn position must not be null");
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void SpawnRock()
    {
        if (currRock == null)
        {
            currRock = Instantiate(rockPrefab, spawnPosition);
            currRock.transform.localPosition = Vector3.zero;
            currRock.isKinematic = true;
        }
    }

    void ThrowRock()
    {
        currRock.transform.parent = parent;
        currRock.isKinematic = false;
        currRock.velocity = Vector3.zero;
        currRock.angularVelocity = Vector3.zero;
        currRock.AddForce(spawnPosition.rotation * launchVelocity, ForceMode.VelocityChange);
        currRock = null;
    }

    public void PlayAudio1()
    {
        audioSource.PlayOneShot(myAudio1, 1f);
    }

    public void PlayAudio2()
    {
        audioSource.PlayOneShot(myAudio2, 1f);
    }

    public void PlayAudio3()
    {
        audioSource.PlayOneShot(myAudio3, 1f);
    }

    public void PlayWalk()
    {
        audioSource.PlayOneShot(walkAudio, 1f);
    }
}
