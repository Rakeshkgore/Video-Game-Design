using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDelegate : MonoBehaviour
{
    public Weapon DelegateTo;

    public bool IsHot
    {
        get => DelegateTo.IsHot;
    }

    void Awake()
    {
        Debug.Assert(DelegateTo != null, $"{name} must have a weapon to delegate to!");
    }

    public void SetHot()
    {
        DelegateTo.SetHot();
    }

    public void SetCold()
    {
        DelegateTo.SetCold();
    }

    public void SetHotDeferred()
    {
        DelegateTo.SetHotDeferred();
    }

    public void SetColdDeferred()
    {
        DelegateTo.SetColdDeferred();
    }
}
