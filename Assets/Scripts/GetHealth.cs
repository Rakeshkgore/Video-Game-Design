using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHealth : MonoBehaviour
{
    public float hp = 100f;
    private float maxHp;

    void Awake()
    {
        maxHp = hp;
    }

    public void ReceiveHealth()
    {
        ReceiveHealth(10f);
    }

    public void ReceiveHealth(float amount)
    {
        hp += 10f;
        if (hp > maxHp)
        {
            hp = maxHp;
        }
    }

    public void LoseHealth()
    {
        LoseHealth(10f);
    }

    public void LoseHealth(float amount)
    {
        hp -= amount;
        if (hp < 0f)
        {
            hp = 0f;
        }
    }
}