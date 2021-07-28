using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : MonoBehaviour
{
    public float duration = 15f;

    private float invisibleUntil;

    void Awake()
    {
        invisibleUntil = float.NegativeInfinity;
    }

    public bool IsInvisible()
    {
        return Time.time <= invisibleUntil;
    }

    public void SetInvisibleUntil(float time)
    {
        invisibleUntil = time;
    }

    public void SetInvisibleFor(float duration)
    {
        SetInvisibleUntil(Time.time + duration);
    }

    public void SetInvisible(bool invisible)
    {
        SetInvisibleUntil(invisible ? float.PositiveInfinity : float.NegativeInfinity);
    }
}
