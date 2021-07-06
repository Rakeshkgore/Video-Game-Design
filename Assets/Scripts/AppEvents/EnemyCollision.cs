using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{

    void OnCollisionEnter(Collision c)
    {

        EventManager.TriggerEvent<BombBounceEvent, Vector3>(c.contacts[0].point);

    }
}