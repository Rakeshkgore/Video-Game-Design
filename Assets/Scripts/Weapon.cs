using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float Damage = 10f;
    public bool IsHot { get; private set; }

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

    public static bool IsCollisionHot(Collision collision, out Weapon weapon)
    {
        weapon = null;
        return (collision.rigidbody != null
            && collision.rigidbody.TryGetComponent<Weapon>(out weapon)
            && weapon.IsHot);
    }
}
