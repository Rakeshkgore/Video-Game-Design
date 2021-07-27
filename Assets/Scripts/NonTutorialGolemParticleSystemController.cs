using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class NonTutorialGolemParticleSystemController : MonoBehaviour
{
    enum State
    {
        WaitingToEmit,
        Emitting,
        Emitted,
    }

    private new ParticleSystem particleSystem;
    private GetBlessed player;
    private State state = State.WaitingToEmit;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        player = FindObjectOfType<GetBlessed>();
    }

    void Update()
    {
        switch (state)
        {
            case State.WaitingToEmit:
            {
                if (particleSystem.particleCount > 0)
                {
                    state = State.Emitting;
                }
                break;
            }
            case State.Emitting:
            {
                if (particleSystem.particleCount <= 0)
                {
                    state = State.Emitted;
                    player.GainAccess();
                    enabled = false;
                }
                break;
            }
        }
    }
}
