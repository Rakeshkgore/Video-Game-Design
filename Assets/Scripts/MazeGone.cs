using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGone : MonoBehaviour
{
    Animator anim;
    GameObject rb;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        GetBlessed gb = rb.GetComponent<GetBlessed>();
        if (gb.DaedalusPassed)
        {
            anim.SetTrigger("Down");
        }
    }
}
