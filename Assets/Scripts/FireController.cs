using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ParticleSystem))]
public class FireController : MonoBehaviour
{
    public float activateIfNeighborActiveFor = 0.1f;
    public FireController[] neighbors;
    public Vector3 velocity;

    private bool active = true;
    private bool destroyed = false;
    private int waterContactCount = 0;
    private float neighborActiveTime = 0f;

    private new ParticleSystem particleSystem;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!active && !destroyed && waterContactCount <= 0 && IsNeighborActive())
        {
            neighborActiveTime += Time.deltaTime;
            if (neighborActiveTime >= activateIfNeighborActiveFor)
            {
                active = true;
            }
        }
        else
        {
            neighborActiveTime = 0f;
        }

        if (active)
        {
            if (!particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }
        else
        {
            if (!particleSystem.isStopped)
            {
                particleSystem.Stop();
            }
        }

        if (destroyed)
        {
            if (particleSystem.particleCount <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position += Time.deltaTime * velocity;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            destroyed = true;
            active = false;
        }
        else if (other.CompareTag("water"))
        {
            ++waterContactCount;
            active = false;
            neighborActiveTime = 0f;
        }
        else if (active)
        {
            other.SendMessage("OnFireHit", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("water"))
        {
            --waterContactCount;
        }
    }

    private bool IsNeighborActive()
    {
        foreach (FireController neighbor in neighbors)
        {
            if (neighbor != null && neighbor.active)
            {
                return true;
            }
        }
        return false;
    }
}
