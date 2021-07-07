using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public GolemAI golem;
    Animator anim;

    public bool IsMoving
    {
        get
        {
            AnimatorStateInfo animatorState = anim.GetCurrentAnimatorStateInfo(0);
            return animatorState.IsTag("lift") && animatorState.normalizedTime < 1f;
        }
    }

    void Awake()
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
