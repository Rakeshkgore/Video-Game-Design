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

    private DeferredAction deferredAction;
    private enum DeferredAction
    {
        NONE,
        SET_HOT,
        SET_COLD,
    }

    void FixedUpdate()
    {
        switch (deferredAction)
        {
            case DeferredAction.SET_HOT:
                SetHot();
                break;
            case DeferredAction.SET_COLD:
                SetCold();
                break;
        }
        deferredAction = DeferredAction.NONE;
    }

    public void SetHot()
    {
        IsHot = true;
    }

    public void SetCold()
    {
        IsHot = false;
    }

    public void SetHotDeferred()
    {
        deferredAction = DeferredAction.SET_HOT;
    }

    public void SetColdDeferred()
    {
        deferredAction = DeferredAction.SET_COLD;
    }

    public static bool IsCollisionHot(Collision collision)
    {
        return IsCollisionHot(collision, out _);
    }

    public static bool IsCollisionHot(Collision collision, out WeaponHot weapon)
    {
        weapon = null;
        return (collision.rigidbody != null
            && collision.rigidbody.TryGetComponent<WeaponHot>(out weapon)
            && weapon.IsHot);
    }
}
