using UnityEngine;
using UnityEngine.AI;

public class AngelStatueAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera playerCamera;
    public NavMeshAgent agent;
    public PlayerStats playerStats;
    public Renderer statueRenderer;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float killDistance = 1.2f;

    [Header("Activation")]
    public string requiredFlag = "puzzle2Solved";
    public bool requireFlagToMove = true;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (statueRenderer == null)
            statueRenderer = GetComponentInChildren<Renderer>();

        if (player != null && playerStats == null)
            playerStats = player.GetComponent<PlayerStats>();

        if (agent != null)
            agent.speed = moveSpeed;
    }

    void Update()
    {
        if (player == null || agent == null) return;

        if (requireFlagToMove && GameManager.Instance != null && !GameManager.Instance.HasFlag(requiredFlag))
        {
            agent.ResetPath();
            return;
        }

        bool playerCanSeeMe = IsVisibleToCamera();

        if (playerCanSeeMe)
            agent.ResetPath();
        else
            agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= killDistance)
            KillPlayer();
    }

    bool IsVisibleToCamera()
    {
        if (playerCamera == null || statueRenderer == null)
            return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        return GeometryUtility.TestPlanesAABB(planes, statueRenderer.bounds);
    }

    void KillPlayer()
    {
        if (playerStats != null)
            playerStats.Die();
    }
}