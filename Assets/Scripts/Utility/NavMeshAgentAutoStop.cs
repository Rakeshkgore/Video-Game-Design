using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentAutoStop : MonoBehaviour
{
    private NavMeshAgent agent;
    private float acceleration;
    public float deceleration = 30f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        acceleration = agent.acceleration;
    }

    void Update()
    {
        if (agent.hasPath
            && agent.remainingDistance <= agent.stoppingDistance
            && agent.velocity.sqrMagnitude != 0f)
        {
            agent.acceleration = deceleration;
        }
        else
        {
            agent.acceleration = acceleration;
        }
    }
}
