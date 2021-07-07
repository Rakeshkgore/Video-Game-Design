using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventManager : MonoBehaviour
{

    public EventSound3D eventSound3DPrefab;

    public AudioClip bombBounceAudio;
    public AudioClip[] boxAudio = null;

    private UnityAction<Vector3,float> boxCollisionEventListener;

    void Awake()
    {

        boxCollisionEventListener = new UnityAction<Vector3,float>(boxCollisionEventHandler);
    }


    // Use this for initialization
    void Start()
    {

        			
    }


    void OnEnable()
    {

        EventManager.StartListening<BoxCollisionEvent, Vector3,float>(boxCollisionEventListener);

    }

    void OnDisable()
    {

        EventManager.StopListening<BoxCollisionEvent, Vector3,float>(boxCollisionEventListener);
    }
 

    void boxCollisionEventHandler(Vector3 worldPos, float impactForce)
    {

        const float halfSpeedRange = 0.2f;

        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

        snd.audioSrc.clip = this.boxAudio[Random.Range(0,boxAudio.Length)];

        snd.audioSrc.pitch = Random.Range(1f-halfSpeedRange, 1f+halfSpeedRange);

        snd.audioSrc.minDistance = Mathf.Lerp(1f, 8f, impactForce /200f);
        snd.audioSrc.maxDistance = 100f;

        snd.audioSrc.Play();
    }


    void bombBounceEventHandler(Vector3 worldPos)
    {

        if (eventSound3DPrefab)
        {

            EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

            snd.audioSrc.clip = this.bombBounceAudio;

            snd.audioSrc.minDistance = 10f;
            snd.audioSrc.maxDistance = 500f;

            snd.audioSrc.Play();
        }
    }

}
