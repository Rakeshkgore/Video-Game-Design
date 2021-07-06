using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetHealth : MonoBehaviour
{
    public float hp = 100f;
    public void ReceiveHealth()
    {
        hp += 10f;
        if (hp > 100f)
        {
            hp = 100f;
        }
    }

    public void LoseHealth()
    {
        hp -= 10f;
        if (hp <= 0f)
        {
            hp = 0f;
            SceneManager.LoadScene("Defeat");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (WeaponHot.IsCollisionHot(collision))
        {
            LoseHealth();
        }
    }
}