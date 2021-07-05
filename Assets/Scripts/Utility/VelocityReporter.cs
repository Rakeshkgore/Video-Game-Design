using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityReporter : MonoBehaviour
{
    public Vector3 velocity { get; private set; } = Vector3.zero;

    private Rigidbody rb;
    private Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        prevPos = transform.position;
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        Vector3 newPos = transform.position;
        velocity = (newPos - prevPos) / Time.fixedDeltaTime;
        prevPos = newPos;
    }
}
