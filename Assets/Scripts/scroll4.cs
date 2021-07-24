using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll4 : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            GetBlessed gb = c.attachedRigidbody.gameObject.GetComponent<GetBlessed>();
            if (gb != null)
            {
                Destroy(this.transform.parent.gameObject);
                gb.UnveilMeleeFortune();
            }
        }
    }
}
