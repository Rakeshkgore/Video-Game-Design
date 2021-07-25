using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHealth : MonoBehaviour
{
    public float hp = 100f;
    public float maxHp { get; private set; }
    public float mhp;
    public float defaultAdd;

    void Awake()
    {
        maxHp = hp;
        mhp = maxHp;
        defaultAdd = 10f;
    }

    void Update()
    {
        if (hp > mhp)
        {
            hp = mhp;
        }
    }

    public void ReceiveHealth()
    {
        ReceiveHealth(defaultAdd);
    }

    public void ReceiveHealth(float amount)
    {
        hp += defaultAdd;
        if (hp > mhp)
        {
            hp = mhp;
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