using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventManager : MonoBehaviour
{

    public EventSound3D eventSound3DPrefab;

    public AudioClip[] boxAudio = null;
    public AudioClip bombBounceAudio;


    private UnityAction<Vector3,float> boxCollisionEventListener;
    private UnityAction<Vector3> bombBounceEventListener;

    void Awake()
    {

        boxCollisionEventListener = new UnityAction<Vector3,float>(boxCollisionEventHandler);
        bombBounceEventListener = new UnityAction<Vector3>(bombBounceEventHandler);
    }


    // Use this for initialization
    void Start()
    {
        EventManager.TriggerEvent<NoParamEvent>();
        EventManager.TriggerEvent<UnityEvent<int>, int>(5);
        EventManager.TriggerEvent<OneParamEvent, int>(5);
        EventManager.TriggerEvent<TwoParamEvent, float, float>(3f, 5f);		
    }


    void OnEnable()
    {

        EventManager.StartListening<BoxCollisionEvent, Vector3,float>(boxCollisionEventListener);
        EventManager.StartListening<BombBounceEvent, Vector3>(bombBounceEventListener);

    }

    void OnDisable()
    {

        EventManager.StopListening<BoxCollisionEvent, Vector3,float>(boxCollisionEventListener);
        EventManager.StopListening<BombBounceEvent, Vector3>(bombBounceEventListener);
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
