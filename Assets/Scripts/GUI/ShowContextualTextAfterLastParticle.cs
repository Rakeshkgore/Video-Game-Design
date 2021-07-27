using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShowContextualTextAfterLastParticle : MonoBehaviour
{
    enum State
    {
        WaitingToEmit,
        Emitting,
        Emitted,
    }

    [TextArea]
    public string text;
    public float duration = 5f;

    private new ParticleSystem particleSystem;
    private State state = State.WaitingToEmit;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
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
                    ContextualText.ShowFor(text, duration);
                    enabled = false;
                }
                break;
            }
        }
    }
}
