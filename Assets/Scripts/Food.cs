using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            GetHealth gh = c.attachedRigidbody.gameObject.GetComponent<GetHealth>();
            if (gh != null)
            {
                if (gh.hp < 100)
                {
                    Destroy(this.gameObject);
                    gh.ReceiveHealth();
                }
            }
        }
    }
}