using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveWeaponEvents : MonoBehaviour
{
    private Dictionary<Weapon, int> weaponContactCounts = new Dictionary<Weapon, int>();

    void OnCollisionEnter(Collision collision)
    {
        Weapon weapon;
        bool isHot = Weapon.IsCollisionHot(collision, out weapon);
        int weaponContactCount = 0;

        if (weapon != null)
        {
            weaponContactCounts.TryGetValue(weapon, out weaponContactCount);
            weaponContactCount += 1;
            weaponContactCounts[weapon] = weaponContactCount;
        }

        if (isHot && weaponContactCount <= 1)
        {
            weapon.SetCold();
            SendMessage("OnWeaponHit", weapon, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Weapon weapon;
        Weapon.IsCollisionHot(collision, out weapon);
        int weaponContactCount = 0;

        if (weapon != null)
        {
            weaponContactCounts.TryGetValue(weapon, out weaponContactCount);
            weaponContactCount -= 1;
            if (weaponContactCount <= 0)
            {
                weaponContactCounts.Remove(weapon);
                SendMessage("OnWeaponLeave", weapon, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                weaponContactCounts[weapon] = weaponContactCount;
            }
        }
    }
}
