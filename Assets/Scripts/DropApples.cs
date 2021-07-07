using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropApples : MonoBehaviour
{
    public Rigidbody[] apples;
    private int appleNum;
    void Awake()
    {
        appleNum = apples.Length;
    }
    void OnWeaponHit(Weapon weapon)
    {
        if (appleNum > 0 && appleNum <= apples.Length)
        {
            apples[appleNum - 1].isKinematic = false;
            appleNum = appleNum - 1;
        }
    }
}
