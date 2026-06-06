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
    public bool requireFlagToMove = true;
    public string requiredFlag = "puzzle2Solved";

    [Header("Vision Rule")]
    public bool stopWhenVisible = true;

    [Header("Movement Sound")]
    public AudioSource movementAudioSource;
    public AudioClip movementSound;
    public bool loopMovementSound = true;
    public float movementSoundVolume = 0.7f;
    public float movingVelocityThreshold = 0.05f;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private bool hasPrintedNoPlayer;
    private bool hasPrintedNoAgent;
    private bool hasPrintedWaitingFlag;
    private bool hasPrintedNoNavMesh;

    void Start()
    {
        AutoAssignReferences();

        if (agent != null)
            agent.speed = moveSpeed;

        SetupAudio();
    }

    void Update()
    {
        AutoAssignReferences();

        if (player == null)
        {
            StopMovementSound();

            if (showDebugLogs && !hasPrintedNoPlayer)
            {
                Debug.LogWarning(name + ": Angel cannot move because Player is missing.");
                hasPrintedNoPlayer = true;
            }

            return;
        }

        if (agent == null)
        {
            StopMovementSound();

            if (showDebugLogs && !hasPrintedNoAgent)
            {
                Debug.LogWarning(name + ": Angel cannot move because NavMeshAgent is missing.");
                hasPrintedNoAgent = true;
            }

            return;
        }

        if (!agent.isOnNavMesh)
        {
            StopMovementSound();

            if (showDebugLogs && !hasPrintedNoNavMesh)
            {
                Debug.LogWarning(name + ": Angel NavMeshAgent is not on a NavMesh. Bake NavMesh or place angel on baked floor.");
                hasPrintedNoNavMesh = true;
            }

            return;
        }

        if (requireFlagToMove)
        {
            bool flagReady = GameManager.Instance != null && GameManager.Instance.HasFlag(requiredFlag);

            if (!flagReady)
            {
                agent.ResetPath();
                StopMovementSound();

                if (showDebugLogs && !hasPrintedWaitingFlag)
                {
                    Debug.LogWarning(name + ": Angel is waiting for flag: " + requiredFlag);
                    hasPrintedWaitingFlag = true;
                }

                return;
            }
        }

        bool playerCanSeeMe = stopWhenVisible && IsVisibleToCamera();

        if (playerCanSeeMe)
        {
            agent.ResetPath();
            StopMovementSound();

            if (showDebugLogs)
                Debug.Log(name + ": Angel stopped because player can see it.");

            return;
        }

        agent.SetDestination(player.position);
        UpdateMovementSound();

        if (showDebugLogs)
        {
            Debug.Log(
                name +
                ": Angel moving. Distance = " +
                Vector3.Distance(transform.position, player.position)
            );
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= killDistance)
            KillPlayer();
    }

    void AutoAssignReferences()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (statueRenderer == null)
            statueRenderer = GetComponentInChildren<Renderer>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (playerStats == null && player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    void SetupAudio()
    {
        if (movementAudioSource == null)
            movementAudioSource = GetComponent<AudioSource>();

        if (movementAudioSource == null)
            movementAudioSource = gameObject.AddComponent<AudioSource>();

        movementAudioSource.playOnAwake = false;
        movementAudioSource.loop = loopMovementSound;
        movementAudioSource.volume = movementSoundVolume;
        movementAudioSource.spatialBlend = 1f;

        if (movementSound != null)
            movementAudioSource.clip = movementSound;
    }

    void UpdateMovementSound()
    {
        if (movementAudioSource == null)
            return;

        if (movementSound != null && movementAudioSource.clip != movementSound)
            movementAudioSource.clip = movementSound;

        bool isActuallyMoving =
            agent.hasPath &&
            agent.velocity.sqrMagnitude > movingVelocityThreshold * movingVelocityThreshold;

        if (isActuallyMoving)
            PlayMovementSound();
        else
            StopMovementSound();
    }

    void PlayMovementSound()
    {
        if (movementAudioSource == null)
            return;

        if (movementAudioSource.clip == null)
            return;

        movementAudioSource.volume = movementSoundVolume;
        movementAudioSource.loop = loopMovementSound;

        if (!movementAudioSource.isPlaying)
            movementAudioSource.Play();
    }

    void StopMovementSound()
    {
        if (movementAudioSource == null)
            return;

        if (movementAudioSource.isPlaying)
            movementAudioSource.Stop();
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
        StopMovementSound();

        if (playerStats != null)
            playerStats.Die();
        else if (UIManager.Instance != null)
            UIManager.Instance.ShowDeath();
    }
}