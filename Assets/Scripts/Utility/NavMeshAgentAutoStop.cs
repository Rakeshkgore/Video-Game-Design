using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentAutoStop : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.velocity = Vector3.zero;
        }
    }
}
