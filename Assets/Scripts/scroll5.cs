using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll5 : MonoBehaviour
{
    [TextArea]
    public string text;
    private CharacterInputController cinput;
    private GetBlessed gb;
    private int triggerCount = 0;

    void Awake()
    {
        GameObject player = GameObject.Find("Player");
        cinput = player.GetComponent<CharacterInputController>();
        gb = player.GetComponent<GetBlessed>();
    }

    void Update()
    {
        if (triggerCount > 0 && !gb.TycheFortuneCollected && cinput.enabled && cinput.Bat)
        {
            Destroy(this.transform.parent.gameObject);
            ContextualText.Hide(text);
            gb.UnveilFireFortune();
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
}
