using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Death")]
    public MonoBehaviour playerMovement;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (UIManager.Instance != null)
            UIManager.Instance.FlashDamage();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowDeath();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Player died");
    }

    void UpdateUI()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetHealth(currentHealth, maxHealth);
    }
}