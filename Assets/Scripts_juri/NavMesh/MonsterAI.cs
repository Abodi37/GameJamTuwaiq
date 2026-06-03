using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Movement")]
    public float wanderRadius = 10f;
    public float changePointEvery = 4f;

    [Header("Chase")]
    public float chaseDistance = 10f;
    public float stopChaseDistance = 15f;
    public float normalSpeed = 3.5f;
    public float chaseSpeed = 7f;

    private float timer;
    private bool isChasing;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.speed = normalSpeed;
        PickNewPoint();
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isChasing && distance <= chaseDistance)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
        }

        if (isChasing && distance >= stopChaseDistance)
        {
            isChasing = false;
            agent.speed = normalSpeed;
            PickNewPoint();
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= changePointEvery || agent.remainingDistance < 1f)
            {
                timer = 0f;
                PickNewPoint();
            }
        }
    }

    void PickNewPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}