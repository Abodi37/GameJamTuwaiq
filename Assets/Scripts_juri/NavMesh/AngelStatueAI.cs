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
        agent.SetDestination(player.position);

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