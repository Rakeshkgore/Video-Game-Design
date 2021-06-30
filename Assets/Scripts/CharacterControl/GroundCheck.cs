using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GroundCheck : MonoBehaviour
{
    public string groundTag = "ground";
    private int groundContactCount = 0;

    public bool IsGrounded
    {
        get
        {
            return groundContactCount > 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider collider in GetComponents<Collider>())
        {
            if (collider.isTrigger)
            {
                return;
            }
        }
        Debug.LogErrorFormat(
            "There is no trigger collider attached to {0}. Ground checking will not work.",
            gameObject.name
        );
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(groundTag))
        {
            groundContactCount += 1;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag(groundTag))
        {
            groundContactCount -= 1;
        }
    }
}
