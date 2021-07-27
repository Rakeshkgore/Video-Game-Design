using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TutorialGolemParticleSystemController : MonoBehaviour
{
    enum State
    {
        WaitingToEmit,
        Emitting,
        Emitted,
    }

    private new ParticleSystem particleSystem;
    private RootMotionControlScript player;
    private State state = State.WaitingToEmit;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        player = FindObjectOfType<RootMotionControlScript>();
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
                    player.canThrow = true;
                    ObjectiveText.SetObjective("find a way to cross the bridge");
                    enabled = false;
                }
                break;
            }
        }
    }
}
