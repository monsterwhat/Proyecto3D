using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{

    public Transform player;
    public NavMeshAgent agent;
    public float agentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = player.position;
        agent.speed = agentSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.position;
    }
}
