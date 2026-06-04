using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Shift Health")]
    public int maxHealth = 10;
    public int currentHealth;
    public int maxShifts = 10;
    public int shiftsUsed = 0;

    [Header("Death")]
    public GameObject deathPanel;
    public MonoBehaviour playerMovement;

    private bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        UpdateUI();
    }

    public bool UseShift()
    {
        if (isDead) return false;

        shiftsUsed++;
        currentHealth = Mathf.Clamp(maxHealth - shiftsUsed, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0 || shiftsUsed >= maxShifts)
        {
            Die();
            return false;
        }

        return true;
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
            Die();
    }

    public int GetShiftsLeft()
    {
        return Mathf.Clamp(maxShifts - shiftsUsed, 0, maxShifts);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (deathPanel != null)
            deathPanel.SetActive(true);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowDeath();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Player Died");
    }

    void UpdateUI()
    {
        int shiftsLeft = GetShiftsLeft();

        if (GameManager.Instance != null)
            GameManager.Instance.SetShiftsLeft(shiftsLeft);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetHealth(currentHealth, maxHealth);
            UIManager.Instance.SetShifts(shiftsLeft, maxShifts);
        }
    }
}