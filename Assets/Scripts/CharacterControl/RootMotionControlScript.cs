using System.Collections;
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
    public float inputForwardScaleInWater = 0.6f;
    public float inputTurnScaleInWater = 0.6f;
    public float invincibilityDuration = 2f;

    public GroundCheck[] additionalGroundChecks = {};
    public GolemAI golem;
    public Lift divination;

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
            return false;
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

        Debug.Assert(golem != null, "Golem must not be null");
    }

    // Use this for initialization
    void Start()
    {
        //example of how to get access to certain limbs
        leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");

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
        bool jump = false;
        bool attack = false;
        bool dive = false;
        bool throwBall = false;

        if (cinput.enabled)
        {
            inputForward = cinput.Forward;
            inputTurn = cinput.Turn;
            inputAction = cinput.Bat;

        }

        //onCollisionXXX() doesn't always work for checking if the character is grounded from a playability perspective
        //Uneven terrain can cause the player to become technically airborne, but so close the player thinks they're touching ground.
        //Therefore, an additional raycast approach is used to check for close ground.
        //This is good for allowing player to jump and not be frustrated that the jump button doesn't
        //work
        bool isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(this.transform.position, jumpableGroundNormalMaxAngle, 0.1f, 1f, out closeToJumpableGround);

        float buttonDistance = float.MaxValue;
        float buttonAngleDegrees = float.MaxValue;

        if (buttonPressStandingSpot != null)
        {
            buttonDistance = Vector3.Distance(transform.position, buttonPressStandingSpot.transform.position);

            buttonAngleDegrees = Quaternion.Angle(transform.rotation, buttonPressStandingSpot.transform.rotation);
        }

        if (inputAction)
        {
            Debug.Log("Action pressed");
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

        if (cinput.Jump)
        {
            jump = true;
        }
        if(cinput.Dive && !divination.IsMoving)
        {
            dive = true;
        }
        if (cinput.ThrowBall && golem.IsDead)
        {
            throwBall = true;
        }

        GetBlessed gb = GetComponent<GetBlessed>();
        float inputTurnScale = divination.IsMoving ? 0.0f : (IsInWater && !gb.PoseidonPassed) ? inputTurnScaleInWater : 1.0f;
        float inputForwardScale = divination.IsMoving ? 0.0f : (IsInWater && !gb.PoseidonPassed) ? inputForwardScaleInWater : 1.0f;

        anim.SetFloat("velx", inputTurn * inputTurnScale);
        anim.SetFloat("vely", inputForward * inputForwardScale);
        anim.SetBool("isFalling", !isGrounded);
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

    private void OnWeaponHit(Weapon weapon)
    {
        if (!invincibility.IsInvincible())
        {
            anim.SetBool("isHit", true);
            health.LoseHealth(weapon.Damage);

            if (health.hp > 0f)
            {
                invincibility.SetInvincibleFor(invincibilityDuration);
            }
        }
    }

    private void OnWeaponLeave(Weapon weapon)
    {
        anim.SetBool("isHit", false);
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

        bool isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(this.transform.position, jumpableGroundNormalMaxAngle, 0.1f, 1f, out closeToJumpableGround);

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
}