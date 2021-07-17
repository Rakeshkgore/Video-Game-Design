using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeActivatation : MonoBehaviour
{
    private GameObject tutorialWall;
    private GameObject mainWall;
    Animator animTutorialWall;
    Animator animMainWall;
    void Awake()
    {
        tutorialWall = GameObject.Find("Cube (9)");
        mainWall = GameObject.Find("Cube (10)");
        animTutorialWall = tutorialWall.GetComponent<Animator>();
        animMainWall = mainWall.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            animTutorialWall.SetTrigger("Descend");
            animMainWall.SetTrigger("Descend");
            gameObject.SetActive(false);
        }
    }
}
