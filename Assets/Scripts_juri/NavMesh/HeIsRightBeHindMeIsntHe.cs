using UnityEngine;
using UnityEngine.AI;

public class HeIsBehindMeIsntHe : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;

    [Header("Follow Settings")]
    public float followDistance = 3f;
    public float stopDistance = 2f;
    public float runDistance = 8f;
    public float updateRate = 0.2f;

    [Header("Speed")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;

    [Header("Animator Parameters")]
    public string isWalkingParam = "isWalking";
    public string isRunningParam = "isRunning";

    private float timer;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = stopDistance;
        }
    }

    void Update()
    {
        if (player == null || agent == null) return;

        timer += Time.deltaTime;

        if (timer >= updateRate)
        {
            timer = 0f;
            FollowPlayer();
        }

        UpdateAnimation();
    }

    void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            agent.SetDestination(player.position);

            if (distance >= runDistance)
                agent.speed = runSpeed;
            else
                agent.speed = walkSpeed;
        }
        else
        {
            agent.ResetPath();
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        bool isMoving = agent.velocity.magnitude > 0.1f;
        bool isRunning = isMoving && agent.speed >= runSpeed - 0.1f;

        animator.SetBool(isWalkingParam, isMoving && !isRunning);
        animator.SetBool(isRunningParam, isRunning);
    }

    public void WarpToPlayer()
    {
        if (agent == null || player == null) return;

        Vector3 pos = player.position - player.forward * 2f;
        agent.Warp(pos);
        agent.ResetPath();
    }
}