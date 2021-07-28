using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincibility : MonoBehaviour
{
    public float invincibleUntil { get; private set; }

    void Awake()
    {
        invincibleUntil = float.NegativeInfinity;
    }

    public bool IsInvincible()
    {
        return Time.time <= invincibleUntil;
    }

    public void SetInvincibleUntil(float time)
    {
        if (time > invincibleUntil)
        {
            invincibleUntil = time;
        }
    }

    public void SetInvincibleFor(float duration)
    {
        SetInvincibleUntil(Time.time + duration);
    }
}
