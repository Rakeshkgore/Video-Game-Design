using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowContextualTextOnTrigger : MonoBehaviour
{
    [TextArea]
    public string text;

    private int triggerCount = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null
            || !other.attachedRigidbody.CompareTag("Player"))
        {
            return;
        }

        if (triggerCount == 0)
        {
            ContextualText.Show(text);
        }
        ++triggerCount;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null
            || !other.attachedRigidbody.CompareTag("Player"))
        {
            return;
        }

        --triggerCount;
        if (triggerCount == 0)
        {
            ContextualText.Hide(text);
        }
    }

    void OnDestroy()
    {
        if (triggerCount > 0)
        {
            ContextualText.Hide(text);
        }
    }
}
