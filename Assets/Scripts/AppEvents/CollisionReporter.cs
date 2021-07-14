using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    bool played;
    void Start() {
        played = false;
    }
    void OnCollisionEnter(Collision c)
    {
        Debug.Log("coll");
        if (played == false) {
            EventManager.TriggerEvent<BombBounceEvent, Vector3>(c.contacts[0].point);
            played = true;
        }
    }
    void OnCollisionExit(Collision c) {
        Debug.Log("coll exit");
        played = false;
    }
}