using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShowContextualLockedText : MonoBehaviour
{
    private const string LOCK_MESSAGE = "Defeat a rock golem to gain access.";

    [TextArea]
    public string text;
    public Material lockedMaterial;

    private new Renderer renderer;
    private Material defaultMaterial;
    private GetBlessed player;
    private int triggerCount = 0;
    private string message;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        player = FindObjectOfType<GetBlessed>();
        defaultMaterial = renderer.sharedMaterial;
    }

    void Update()
    {
        if (player.accessToTrials > 0)
        {
            renderer.sharedMaterial = defaultMaterial;
        }
        else
        {
            renderer.sharedMaterial = lockedMaterial;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null
            || !other.attachedRigidbody.CompareTag("Player"))
        {
            return;
        }

        if (triggerCount == 0)
        {
            message = text;
            if (player.accessToTrials <= 0)
            {
                message += "\n" + LOCK_MESSAGE;
            }
            ContextualText.Show(message);
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
            ContextualText.Hide(message);
            message = null;
        }
    }

    void OnDisable()
    {
        if (message != null)
        {
            ContextualText.Hide(message);
            message = null;
        }
    }
}
