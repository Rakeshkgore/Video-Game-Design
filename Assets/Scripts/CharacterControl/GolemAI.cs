using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GetHealth))]
public class GolemAI : MonoBehaviour
{
    public new GameObject particleSystem;
    private Animator animator;
    private GetHealth health;

    public bool IsDead
    {
        get => health.hp <= 0f;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<GetHealth>();
        Debug.Assert(particleSystem != null, "Particle System must not be null");
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("throw", true);
        animator.SetBool("dead", IsDead);
        particleSystem.SetActive(IsDead);
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
