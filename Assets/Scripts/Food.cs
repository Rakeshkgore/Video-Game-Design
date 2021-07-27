using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private const string FULL_HEALTH_MESSAGE = "You are already at full health!";

    private bool consumed = false;
    private bool showingFullHealthMessage = false;

    void OnTriggerEnter(Collider c)
    {
        if (!consumed && c.attachedRigidbody != null)
        {
            GetHealth gh = c.attachedRigidbody.gameObject.GetComponent<GetHealth>();
            if (gh != null)
            {
                if (gh.hp < 100)
                {
                    consumed = true;
                    Destroy(this.transform.parent.gameObject);
                    gh.ReceiveHealth();

                    if (showingFullHealthMessage)
                    {
                        ContextualText.Hide(FULL_HEALTH_MESSAGE);
                        showingFullHealthMessage = false;
                    }
                }
                else if (!showingFullHealthMessage && gh.CompareTag("Player"))
                {
                    ContextualText.Show(FULL_HEALTH_MESSAGE);
                    showingFullHealthMessage = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (showingFullHealthMessage
            && c.attachedRigidbody != null
            && c.attachedRigidbody.CompareTag("Player"))
        {
            ContextualText.Hide(FULL_HEALTH_MESSAGE);
            showingFullHealthMessage = false;
        }
    }
}