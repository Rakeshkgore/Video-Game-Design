﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GetHealth))]
[RequireComponent(typeof(Invincibility))]
public class GolemAI : MonoBehaviour
{
    public float invincibilityDuration = 1f;
    public new GameObject particleSystem;
    private Animator animator;
    private GetHealth health;
    private Invincibility invincibility;

    public bool IsDead
    {
        get => health.hp <= 0f;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<GetHealth>();
        invincibility = GetComponent<Invincibility>();
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
        if (!invincibility.IsInvincible())
        {
            health.LoseHealth(weapon.Damage);
            animator.SetTrigger("hit");

            if (health.hp > 0f)
            {
                invincibility.SetInvincibleFor(80f / 60f);
            }
        }
    }
}
