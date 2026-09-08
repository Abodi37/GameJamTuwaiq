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

    [Header("Performance")]
    [Tooltip("Seconds between path recalculations. 0 recalculates every frame.")]
    public float destinationUpdateRate = 0.2f;

    // Reused every frame so the visibility test does not allocate a new Plane[6].
    private readonly Plane[] frustumPlanes = new Plane[6];
    private float destinationTimer;

    void Start()
    {
        AutoAssignReferences();

        if (agent != null)
            agent.speed = moveSpeed;

        SetupAudio();
    }

    void Update()
    {
        // AutoAssignReferences used to run every frame, which meant a
        // GameObject.FindGameObjectWithTag plus two GetComponent walks per statue
        // per frame. The references cannot change at runtime, so Start is enough.
        if (player == null || agent == null)
        {
            StopMovementSound();
            return;
        }

        if (!agent.isOnNavMesh)
        {
            StopMovementSound();
            return;
        }

        if (requireFlagToMove)
        {
            bool flagReady =
                GameManager.Instance != null &&
                GameManager.Instance.HasFlag(requiredFlag);

            if (!flagReady)
            {
                agent.ResetPath();
                StopMovementSound();
                return;
            }
        }

        bool playerCanSeeMe = stopWhenVisible && IsVisibleToCamera();

        if (playerCanSeeMe)
        {
            agent.ResetPath();
            StopMovementSound();
            return;
        }

        agent.speed = moveSpeed;
        agent.isStopped = false;

        // Recalculating a full path every frame is the single most expensive thing
        // a NavMeshAgent can do. The player cannot outrun a 0.2s refresh.
        destinationTimer -= Time.deltaTime;

        if (destinationTimer <= 0f)
        {
            destinationTimer = destinationUpdateRate;
            agent.SetDestination(player.position);
        }

        UpdateMovementSound();

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

        // Non-allocating overload: the array version allocated a Plane[6] every
        // frame for every statue, which added up to constant GC pressure.
        GeometryUtility.CalculateFrustumPlanes(playerCamera, frustumPlanes);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, statueRenderer.bounds);
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