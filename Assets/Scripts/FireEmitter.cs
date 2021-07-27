using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEmitter : MonoBehaviour
{
    public FireController prefab;
    public float radius = 3f;
    public int slices = 180;
    public float speed = 6f;

    void EmitFire()
    {
        FireController previous = null;
        FireController first = null;
        FireController current = null;

        for (int i = 0; i < slices; ++i)
        {
            float theta = 2f * Mathf.PI * i / slices;
            float sin = Mathf.Sin(theta);
            float cos = Mathf.Cos(theta);
            Vector3 position = transform.position + cos * radius * transform.right + sin * radius * transform.forward;
            Vector3 velocity = cos * speed * transform.right + sin * speed * transform.forward;

            current = Instantiate(prefab, position, transform.rotation);
            current.velocity = velocity;
            current.neighbors = new FireController[] { previous, null };

            if (previous != null)
            {
                previous.neighbors[1] = current;
            }
            else
            {
                first = current;
            }

            previous = current;
        }

        first.neighbors[0] = current;
        current.neighbors[1] = first;
    }
}
