using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentDeceleration : MonoBehaviour
{
    public float deceleration = 60f;
    private float acceleration;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        acceleration = agent.acceleration;
    }

    void Update()
    {
        if (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.acceleration = deceleration;
        }
        else
        {
            agent.acceleration = acceleration;
        }
    }
}
