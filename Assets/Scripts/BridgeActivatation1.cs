using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeActivatation1 : MonoBehaviour
{
    public GameObject tutorialWall;
    public GameObject mainWall;
    Animator animTutorialWall;
    Animator animMainWall;
    GameObject rb;

    void Awake()
    {
        Debug.Assert(tutorialWall != null, "Tutorial Wall must not be null!");
        Debug.Assert(mainWall != null, "Main Wall must not be null!");
        animTutorialWall = tutorialWall.GetComponent<Animator>();
        animMainWall = mainWall.GetComponent<Animator>();
        rb = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void OnCollisionEnter(Collision c)
    {
        GetBlessed gb = rb.GetComponent<GetBlessed>();
        if (c.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile") && gb.accessToTrials > 0)
        {
            animTutorialWall.SetTrigger("Descend");
            animMainWall.SetTrigger("Descend");
            gameObject.SetActive(false);
            gb.accessToTrials -= 1;
        }
    }
}
