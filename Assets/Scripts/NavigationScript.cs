using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationScript : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    public float followDistance;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followDistance && gameObject.GetComponent<Enemy>().isDie == false)
        {
            agent.destination = player.position;
        }
        else
        {
            agent.ResetPath();
        }
    }
}
