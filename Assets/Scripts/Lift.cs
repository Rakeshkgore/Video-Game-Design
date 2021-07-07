using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public GolemAI golem;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            GetHealth gh = c.attachedRigidbody.gameObject.GetComponent<GetHealth>();
            if (gh != null && golem.IsDead)
            {
                anim.SetTrigger("Lift");
            }
        }
    }
}
