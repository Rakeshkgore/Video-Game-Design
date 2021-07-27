using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll1 : MonoBehaviour
{
    [TextArea]
    public string text;
    public float duration = 5f;

    private bool isTriggered = false;

    void OnTriggerEnter(Collider c)
    {
        if (!isTriggered
            && c.attachedRigidbody != null
            && c.attachedRigidbody.TryGetComponent<GetBlessed>(out GetBlessed gb))
        {
            isTriggered = true;
            Destroy(this.gameObject);
            gb.WaterFree();
            ContextualText.ShowFor(text, duration);
        }
    }
}