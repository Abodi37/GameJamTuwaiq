using UnityEngine;
using UnityEngine.AI;

public class HeIsBehindMeIsntHe : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public float followDistance = 3f;
    public float updateRate = 0.2f;

    private float timer;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null || agent == null) return;

        timer += Time.deltaTime;

        if (timer >= updateRate)
        {
            timer = 0f;

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > followDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.ResetPath();
            }
        }
    }
}