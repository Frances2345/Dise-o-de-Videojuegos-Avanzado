using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{

    public Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
    }
}
