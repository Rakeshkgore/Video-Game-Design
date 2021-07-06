using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHealth : MonoBehaviour
{
    public float hp = 100f;
    public void ReceiveHealth()
    {
        if (hp <= 90)
        {
            hp = hp + 10;
        }
        else if (hp > 90 && hp <100)
        {
            hp = 100;
        }
    }

    public void LoseHealth()
    {
        Debug.Log(hp);
        hp = hp - 10;
        Debug.Log(hp);
    }
}