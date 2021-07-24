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
    private bool accessLock;
    GameObject rb;
    public bool IsDead;

    void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<GetHealth>();
        Debug.Assert(particleSystem != null, "Particle System must not be null");
        IsDead = false;
        accessLock = false;
        rb = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (health.hp == 0f)
        {
            IsDead = true;
            if (!accessLock)
            {
                if (this.gameObject.name == "PBR_Golem (1)" || this.gameObject.name == "PBR_Golem (2)")
                {
                    GetBlessed gb = rb.GetComponent<GetBlessed>();
                    gb.GainAccess();
                    accessLock = true;
                }
            }
        }
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