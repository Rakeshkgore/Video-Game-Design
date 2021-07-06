using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponHot), typeof(Rigidbody))]
public class RockController : MonoBehaviour
{
    private WeaponHot weapon;
    private Rigidbody rb;

    void Awake()
    {
        if (!TryGetComponent<WeaponHot>(out weapon))
        {
            Debug.LogError("A \"Weapon Hot\" script must be attached to this object!");
        }
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.LogError("A rigidbody must be attached to this object!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        weapon.SetHot();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (weapon.IsHot && !rb.isKinematic && collision.gameObject.CompareTag("ground"))
        {
            weapon.SetCold();
        }
    }
}
