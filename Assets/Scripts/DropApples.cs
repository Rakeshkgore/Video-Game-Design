using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropApples : MonoBehaviour
{
    public Rigidbody[] apples;
    private int appleNum;
    // Start is called before the first frame update
    void Start()
    {
        appleNum = apples.Length;
    }
    void OnCollisionEnter(Collision c)
    {
        if (appleNum > 0 && appleNum <= apples.Length)
        {
            if (c.gameObject.TryGetComponent<WeaponHot>(out WeaponHot weapon)
                && weapon.IsHot)
            {
                apples[appleNum - 1].isKinematic = false;
                appleNum = appleNum - 1;
            }
        }
    }
}
