using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleport : MonoBehaviour
{
    public int teleportCost = 10;

    private PlayerStats health;

    private void Awake()
    {
        health = GetComponent<PlayerStats>();
    }

    public void OnTeleport(InputAction.CallbackContext context)
    {
        Debug.Log("Shifted");

        if (!context.performed)
            return;

        if (health.currentHealth <= teleportCost)
            return;

        health.TakeDamage(teleportCost);

        RoomManager.Instance.SwitchRoom(transform);
    }
}