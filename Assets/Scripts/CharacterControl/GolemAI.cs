using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GetHealth))]
public class GolemAI : MonoBehaviour
{
    private Animator animator;
    private GetHealth health;

    void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<GetHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("throw", true);
        animator.SetBool("dead", health.hp <= 0f);
    }

    void OnWeaponHit(Weapon weapon)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("hit"))
        {
            health.LoseHealth(weapon.Damage);
            animator.SetTrigger("hit");
        }
    }
}
