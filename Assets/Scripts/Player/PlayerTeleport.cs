using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleport : MonoBehaviour
{
    public AudioSource shiftSound;
    public GameObject shiftEffect;

    private PlayerStats playerStats;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    public void OnTeleport(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Time.timeScale == 0f) return;

        if (DialogueSystem.Instance != null && DialogueSystem.Instance.IsPlaying)
            return;

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (playerStats == null || playerStats.IsDead())
            return;

        bool canShift = playerStats.UseShift();

        if (!canShift)
            return;

        if (shiftSound != null)
            shiftSound.Play();

        if (shiftEffect != null)
            Instantiate(shiftEffect, transform.position, Quaternion.identity);

        if (RoomManager.Instance != null)
            RoomManager.Instance.SwitchRoom(transform);
    }
}