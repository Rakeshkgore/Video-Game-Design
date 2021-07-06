using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHot : MonoBehaviour
{
    public WeaponHot DelegateOf;

    private bool isHot = false;
    public bool IsHot
    {
        get
        {
            return (DelegateOf != null) ? DelegateOf.IsHot : isHot;
        }

        private set
        {
            if (DelegateOf != null)
            {
                DelegateOf.IsHot = value;
            }
            else
            {
                isHot = value;
            }
        }
    }

    public void SetHot()
    {
        IsHot = true;
    }

    public void SetCold()
    {
        IsHot = false;
    }
}
