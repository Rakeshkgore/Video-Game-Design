using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilityTrigger : MonoBehaviour
{
    private int triggerCount = 0;

    void OnTriggerEnter(Collider other)
    {
        GetBlessed gb;
        Invisibility invisibility;

        if (other.attachedRigidbody == null
            || !other.attachedRigidbody.TryGetComponent<GetBlessed>(out gb)
            || !other.attachedRigidbody.TryGetComponent<Invisibility>(out invisibility))
        {
            return;
        }

        if (triggerCount == 0 && gb.DaedalusPassed)
        {
            invisibility.SetInvisible(true);
        }
        ++triggerCount;
    }

    void OnTriggerExit(Collider other)
    {
        GetBlessed gb;
        Invisibility invisibility;

        if (other.attachedRigidbody == null
            || !other.attachedRigidbody.TryGetComponent<GetBlessed>(out gb)
            || !other.attachedRigidbody.TryGetComponent<Invisibility>(out invisibility))
        {
            return;
        }

        --triggerCount;
        if (triggerCount == 0 && gb.DaedalusPassed)
        {
            invisibility.SetInvisibleFor(invisibility.duration);
        }
    }
}
