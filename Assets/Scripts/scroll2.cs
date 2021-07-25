using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll2 : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            GetBlessed gb = c.attachedRigidbody.gameObject.GetComponent<GetBlessed>();
            if (gb != null)
            {
                 Destroy(this.gameObject);
                 gb.Invisible();
            }
        }
    }
}