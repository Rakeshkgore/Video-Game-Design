using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingBeanController : MonoBehaviour
{

    private Rigidbody rb;
    public float torque;
    public float jumpableGroundNormalMaxAngle = 45f;
    public bool closeToJumpableGround;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private int groundContactCount = 0;

    public bool IsGrounded
    {
        get
        {
            return groundContactCount > 0;
        }
    }
    void FixedUpdate()
    {

        bool isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(transform.position, jumpableGroundNormalMaxAngle, 0.1f, 1f, out closeToJumpableGround);
        if (isGrounded)
        {
            
            int y = Random.Range(2, 6);
            rb.AddForce(new Vector3(0, y, 0), ForceMode.VelocityChange);
            //Debug.Log("HERE");
            StartCoroutine(myTimer());
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
        {

            ++groundContactCount;

        }

    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
        {
            --groundContactCount;

        }

    }

    IEnumerator myTimer()
    {

        int y = Random.Range(5,10);
        yield return new WaitForSeconds(y);
        //Debug.Log("WAIT");

    }
}
