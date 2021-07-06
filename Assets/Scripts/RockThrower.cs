using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrower : MonoBehaviour
{
    public Rigidbody rockPrefab;
    public Transform spawnPosition;
    public Transform parent;
    public Vector3 launchVelocity;

    private Rigidbody currRock;

    void Awake()
    {
        Debug.Assert(rockPrefab != null, "Rock prefab must not be null");
        Debug.Assert(spawnPosition != null, "Spawn position must not be null");
    }

    void SpawnRock()
    {
        if (currRock == null)
        {
            currRock = Instantiate(rockPrefab, spawnPosition);
            currRock.transform.localPosition = Vector3.zero;
            currRock.isKinematic = true;
        }
    }

    void ThrowRock()
    {
        currRock.transform.parent = parent;
        currRock.isKinematic = false;
        currRock.velocity = Vector3.zero;
        currRock.angularVelocity = Vector3.zero;
        currRock.AddForce(spawnPosition.rotation * launchVelocity, ForceMode.VelocityChange);
        currRock = null;
    }
}
