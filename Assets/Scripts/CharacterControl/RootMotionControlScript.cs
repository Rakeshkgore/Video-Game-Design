﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterInputController))]
[RequireComponent(typeof(Invincibility))]
public class RootMotionControlScript : MonoBehaviour
{
    //My additions to "add some tweaks to the playback of animations"
    public float animationSpeed = 1f;
    public float rootMovementSpeed = 1f;
    public float rootTurnSpeed = 1f;
    public float jumpVelocity = 5f;
    public float inputForwardScaleInWater = 0.6f;
    public float inputTurnScaleInWater = 0.6f;
    public float invincibilityDuration = 2f;
    public bool canThrow = false;

    public GroundCheck[] additionalGroundChecks = {};

    private Animator anim;
    private Rigidbody rbody;
    private CharacterInputController cinput;
    private Transform leftFoot;
    private Transform rightFoot;
    private GetHealth health;
    private Invincibility invincibility;

    public GameObject buttonPressStandingSpot;
    public float buttonCloseEnoughForMatchDistance = 2f;
    public float buttonCloseEnoughForPressDistance = 0.22f;
    public float buttonCloseEnoughForPressAngleDegrees = 5f;
    public float initalMatchTargetsAnimTime = 0.25f;
    public float exitMatchTargetsAnimTime = 0.75f;

    //Useful if you implement jump in the future...
    public float jumpableGroundNormalMaxAngle = 45f;
    public bool closeToJumpableGround;
    private bool jump = false;
    private float drag;
    private float angularDrag;


    private int groundContactCount = 0;
    private int waterContactCount = 0;
    private int wallContactCount = 0;

    public Canvas canvas;

    public bool IsGrounded
    {
        get
        {
            if (groundContactCount > 0)
            {
                return true;
            }
            foreach (GroundCheck groundCheck in additionalGroundChecks)
            {
                if (groundCheck.IsGrounded)
                {
                    return true;
                }
            }

            //onCollisionXXX() doesn't always work for checking if the character is grounded from a playability perspective
            //Uneven terrain can cause the player to become technically airborne, but so close the player thinks they're touching ground.
            //Therefore, an additional raycast approach is used to check for close ground.
            //This is good for allowing player to jump and not be frustrated that the jump button doesn't
            //work
            return CharacterCommon.CheckGroundNear(this.transform.position, jumpableGroundNormalMaxAngle, 0.1f, 1f, out closeToJumpableGround);
        }
    }

    public bool IsInWater
    {
        get
        {
            return waterContactCount > 0;
        }
    }

    public bool IsHitWall
    {
        get
        {
            return wallContactCount > 0;
        }
    }

    void Awake()
    {

        anim = GetComponent<Animator>();

        if (anim == null)
            Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

        cinput = GetComponent<CharacterInputController>();
        if (cinput == null)
            Debug.Log("CharacterInput could not be found");

        health = rbody.GetComponent<GetHealth>();
        invincibility = GetComponent<Invincibility>();

        drag = rbody.drag;
        angularDrag = rbody.angularDrag;
    }

    // Use this for initialization
    void Start()
    {
        //example of how to get access to certain limbs
        leftFoot = this.transform.Find("metarig_001/hips/thigh_L/shin_L/foot_L");
        rightFoot = this.transform.Find("metarig_001/hips/thigh_R/shin_R/foot_R");

        if (leftFoot == null || rightFoot == null)
            Debug.Log("One of the feet could not be found");

    }

    void Update()
    {
        if (health.hp <= 0f)
        {
            StartCoroutine(FadeScene());
        }

        float inputForward = 0f;
        float inputTurn = 0f;
        bool inputAction = false;
        bool doButtonPress = false;
        bool doMatchToButtonPress = false;
        bool attack = false;
        bool dive = false;
        bool throwBall = false;

        if (cinput.enabled)
        {
            inputForward = cinput.Forward;
            inputTurn = cinput.Turn;
            inputAction = cinput.Bat;

        }

        float buttonDistance = float.MaxValue;
        float buttonAngleDegrees = float.MaxValue;

        if (buttonPressStandingSpot != null)
        {
            buttonDistance = Vector3.Distance(transform.position, buttonPressStandingSpot.transform.position);

            buttonAngleDegrees = Quaternion.Angle(transform.rotation, buttonPressStandingSpot.transform.rotation);
        }

        if (inputAction)
        {
            attack = true;
            //if (buttonDistance <= buttonCloseEnoughForMatchDistance)
            //{
            //    if (buttonDistance <= buttonCloseEnoughForPressDistance &&
            //        buttonAngleDegrees <= buttonCloseEnoughForPressAngleDegrees)
            //    {
            //        Debug.Log("Button press initiated");

            //        doButtonPress = true;

            //    }
            //    else
            //    {
            //        // TODO UNCOMMENT THESE LINES FOR TARGET MATCHING
            //        Debug.Log("match to button initiated");
            //        doMatchToButtonPress = true;
            //    }

            //}
        }


        // TODO HANDLE BUTTON MATCH TARGET HERE

        // Get info about current animation
        var animState = anim.GetCurrentAnimatorStateInfo(0);

        // If the transistion to button press has been initiated then we want
        // To correct the character posistion to the correct place

        if( animState.IsName("MatchToButtonPress") && !anim.IsInTransition(0)
            && !anim.isMatchingTarget)
        {
            if(buttonPressStandingSpot != null)
            {
                Debug.Log("Target matching correction started");

                initalMatchTargetsAnimTime = animState.normalizedTime;
                var t = buttonPressStandingSpot.transform;
                anim.MatchTarget(t.position, t.rotation, AvatarTarget.Root,
                    new MatchTargetWeightMask(new Vector3(1f, 0f, 1f), 1f),
                    initalMatchTargetsAnimTime, exitMatchTargetsAnimTime);

            }
        }

        if (!jump && cinput.Jump && IsAnimationPlaying(1, "idle"))
        {
            jump = true;
            Jump();
        }
        if(cinput.Dive)
        {
            dive = true;
        }
        if (canThrow && cinput.ThrowBall)
        {
            throwBall = true;
        }

        GetBlessed gb = GetComponent<GetBlessed>();
        float inputTurnScale = (IsInWater && !gb.PoseidonPassed) ? inputTurnScaleInWater : 1.0f;
        float inputForwardScale = (IsInWater && !gb.PoseidonPassed) ? inputForwardScaleInWater : 1.0f;

        anim.SetFloat("velx", inputTurn * inputTurnScale);
        anim.SetFloat("vely", inputForward * inputForwardScale);
        anim.SetBool("isFalling", !IsGrounded);
        anim.SetBool("doButtonPress", doButtonPress);
        anim.SetBool("matchToButtonPress", doMatchToButtonPress);
        anim.SetBool("jump", jump);
        anim.SetBool("attack", attack);
        anim.SetBool("dive", dive);
        anim.SetBool("throwTest", throwBall);

        //My additions to "add some tweaks to the playback of animations"
        anim.speed = animationSpeed;
    }

    IEnumerator FadeScene()
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        float duration = 0.8f;
        float counter = 0f;
        while (counter < duration) {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);
            yield return null;
        }
        SceneManager.LoadScene("Defeat");
    }


    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("water"))
        {

            ++groundContactCount;

            // Generate an event that might play a sound, generate a particle effect, etc.
            EventManager.TriggerEvent<PlayerLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);

        }

        if (collision.gameObject.CompareTag("water"))
        {
            ++waterContactCount;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            ++wallContactCount;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("water"))
        {
            --groundContactCount;
        }

        if (collision.gameObject.CompareTag("water"))
        {
            --waterContactCount;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            --wallContactCount;
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("water"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("Submarine");
        }
    }
    private void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("water"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    private bool TryTakeDamage(float damage)
    {
        if (invincibility.IsInvincible())
        {
            return false;
        }

        anim.SetTrigger("isHit");
        health.LoseHealth(damage);

        if (health.hp > 0f)
        {
            invincibility.SetInvincibleFor(invincibilityDuration);
        }

        return true;
    }

    private void OnWeaponHit(Weapon weapon)
    {
        TryTakeDamage(weapon.Damage);
    }

    private void OnSoundParticleHit()
    {
        TryTakeDamage(5f);
    }

    private void OnFireHit()
    {
        TryTakeDamage(15f);
    }

    public GameObject buttonObject;
    private void OnAnimatorIK(int layerIndex)
    {
        if(anim)
        {
            AnimatorStateInfo astate = anim.GetCurrentAnimatorStateInfo(0);
            if(astate.IsName("ButtonPress"))
            {
                float buttonWeight = anim.GetFloat("buttonClose");
                if (buttonObject != null)
                {
                    anim.SetLookAtWeight(buttonWeight);
                    anim.SetLookAtPosition(buttonObject.transform.position);
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, buttonWeight);
                    anim.SetIKPosition(AvatarIKGoal.RightHand,
                    buttonObject.transform.position);
                }

            } else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim.SetLookAtWeight(0);
            }
        }
    }

    void OnAnimatorMove()
    {
        Vector3 newRootPosition;
        Quaternion newRootRotation;

        if (IsGrounded)
        {
            // use root motion as is
            newRootPosition = anim.rootPosition;
            newRootRotation = anim.rootRotation;

            //TODO Here, you could scale the difference in position and rotation to make the character go faster or slower

            //My additions to "add some tweaks to the playback of animations"
            newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
            newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, rootTurnSpeed);

            this.transform.position = newRootPosition;
            this.transform.rotation = newRootRotation;
        }
        else
        {
            Vector3 rootVelocity = GetEstimatedVelocity();
            float rootAngularSpeed = GetEstimatedAngularSpeed();
            rbody.velocity = new Vector3(rootVelocity.x, rbody.velocity.y, rootVelocity.z);
            rbody.angularVelocity = new Vector3(0f, rootAngularSpeed, 0f);
        }
    }

    public Vector3 GetEstimatedVelocity()
    {
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        Vector3 velocity = Vector3.zero;
        foreach (AnimatorClipInfo clip in clips)
        {
            velocity += clip.clip.averageSpeed * clip.weight;
        }
        return transform.rotation * velocity * rootMovementSpeed;
    }

    public float GetEstimatedAngularSpeed()
    {
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        float angularSpeed = 0f;
        foreach (AnimatorClipInfo clip in clips)
        {
            angularSpeed += clip.clip.averageAngularSpeed * clip.weight;
        }
        return angularSpeed * rootTurnSpeed;
    }

    private void Jump()
    {
        rbody.AddForce(new Vector3(0f, jumpVelocity, 0f), ForceMode.VelocityChange);
    }

    private void Land()
    {
        jump = false;
    }

    public bool IsAnimationPlaying(int layer, string tag)
    {
        return anim.GetCurrentAnimatorStateInfo(layer).IsTag(tag);
    }
}