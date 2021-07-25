using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dry : MonoBehaviour
{
    private Animator anim;
    GameObject rb;

    void Awake()
    {
        anim = this.transform.parent.gameObject.GetComponent<Animator>();
        rb = GameObject.Find("Player");
    }

    void Update()
    {
        GetBlessed gb = rb.GetComponent<GetBlessed>();
        if (gb.PoseidonPassed)
        {
            Destroy(this.transform.parent.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = new Vector3 (2.62f, 0f, 40.72f);
        }
    }
}
