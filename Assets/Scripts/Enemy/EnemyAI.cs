using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Vector3 startPosition;
    private NavMeshAgent agent;

    private void Start()
    {
        startPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }

    public void ResetEnemy()
    {
        agent.ResetPath();
        agent.SetDestination(startPosition);
    }
}