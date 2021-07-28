using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeActivatation : MonoBehaviour
{
    public GameObject tutorialWall;
    public GameObject mainWall;
    Animator animTutorialWall;
    Animator animMainWall;
    void Awake()
    {
        Debug.Assert(tutorialWall != null, "Tutorial Wall must not be null!");
        Debug.Assert(mainWall != null, "Main Wall must not be null!");
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
            ObjectiveText.SetObjective("collect more powers and defeat the rhino");
        }
    }
}
