using UnityEngine;
using UnityEngine.AI;

public class ShiftManager : MonoBehaviour
{
    [Header("Rooms")]
    public ShiftRoom[] rooms;
    public int currentRoomIndex = 0;

    [Header("Player")]
    public Transform player;
    public HealthSystem playerHealth;

    [Header("Companion Optional")]
    public NavMeshAgent companionAgent;
    public Transform companionTransform;

    [Header("Shift Settings")]
    public int maxShifts = 10;
    public int shiftsUsed = 0;

    [Header("Effects Optional")]
    public AudioSource shiftSound;
    public GameObject shiftEffect;

    private bool canShift = true;

    void Start()
    {
        MoveToRoom(currentRoomIndex);
        UpdateUI();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (DialogueSystem.Instance != null && DialogueSystem.Instance.IsPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryShift();
        }
    }

    public void TryShift()
    {
        if (!canShift) return;
        if (player == null) return;
        if (rooms == null || rooms.Length == 0) return;

        shiftsUsed++;

        if (shiftSound != null)
            shiftSound.Play();

        if (shiftEffect != null)
            Instantiate(shiftEffect, player.position, Quaternion.identity);

        if (shiftsUsed >= maxShifts)
        {
            UpdateUI();

            if (playerHealth != null)
                playerHealth.Die();

            return;
        }

        currentRoomIndex++;

        if (currentRoomIndex >= rooms.Length)
            currentRoomIndex = 0;

        MoveToRoom(currentRoomIndex);
        UpdateUI();
    }

    void MoveToRoom(int index)
    {
        if (rooms == null || rooms.Length == 0) return;
        if (index < 0 || index >= rooms.Length) return;
        if (rooms[index] == null) return;

        Transform spawn = rooms[index].playerSpawnPoint;

        if (spawn != null && player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (cc != null)
                cc.enabled = false;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            player.position = spawn.position;
            player.rotation = spawn.rotation;

            if (cc != null)
                cc.enabled = true;
        }

        if (companionAgent != null)
        {
            Transform companionSpawn = rooms[index].companionSpawnPoint;

            if (companionSpawn != null)
            {
                companionAgent.Warp(companionSpawn.position);
            }
            else if (player != null)
            {
                companionAgent.Warp(player.position + player.right * 2f);
            }

            companionAgent.ResetPath();
        }

        Debug.Log("Shifted to room: " + rooms[index].roomName);
    }

    void UpdateUI()
    {
        int shiftsLeft = maxShifts - shiftsUsed;
        shiftsLeft = Mathf.Clamp(shiftsLeft, 0, maxShifts);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetShifts(shiftsLeft, maxShifts);

            if (rooms != null && rooms.Length > 0)
            {
                if (currentRoomIndex >= 0 && currentRoomIndex < rooms.Length)
                {
                    if (rooms[currentRoomIndex] != null)
                        UIManager.Instance.SetRoom(rooms[currentRoomIndex].roomName);
                }
            }
        }
    }

    public void SetCanShift(bool value)
    {
        canShift = value;
    }
}