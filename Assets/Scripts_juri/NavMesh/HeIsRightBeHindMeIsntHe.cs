using UnityEngine;
using UnityEngine.AI;

public class HeIsRightBehindMeIsntHe : MonoBehaviour
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

    [Header("Debug")]
    public bool showDebug = true;

    private float timer;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = stopDistance;
            agent.isStopped = false;
        }

        if (showDebug)
        {
            if (agent == null)
                Debug.LogError("No NavMeshAgent found on companion.");

            if (animator == null)
                Debug.LogError("No Animator found on companion or children.");

            if (player == null)
                Debug.LogError("Player is not assigned.");
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
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (distance >= runDistance)
                agent.speed = runSpeed;
            else
                agent.speed = walkSpeed;
        }
        else
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        float velocity = agent.velocity.magnitude;
        float desiredVelocity = agent.desiredVelocity.magnitude;

        bool isMoving = velocity > 0.05f || desiredVelocity > 0.05f;
        bool isRunning = isMoving && agent.speed >= runSpeed - 0.1f;

        animator.SetBool(isWalkingParam, isMoving && !isRunning);
        animator.SetBool(isRunningParam, isRunning);

        if (showDebug)
        {
            Debug.Log("Companion Moving: " + isMoving + 
                      " | Running: " + isRunning + 
                      " | Velocity: " + velocity);
        }
    }

    public void WarpToPlayer()
    {
        if (agent == null || player == null) return;

        Vector3 pos = player.position - player.forward * 2f;

        if (agent.isOnNavMesh)
        {
            agent.Warp(pos);
            agent.ResetPath();
        }
    }
}