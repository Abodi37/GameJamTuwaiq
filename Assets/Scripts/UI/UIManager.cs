using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Texts")]
    public TMP_Text healthText;
    public TMP_Text shiftText;
    public TMP_Text roomText;
    public TMP_Text objectiveText;
    public TMP_Text interactText;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Panels")]
    public GameObject deathPanel;
    public GameObject pausePanel;

    [Header("Damage Effect Optional")]
    public Image damageFlashImage;
    public float flashDuration = 0.25f;

    private Coroutine flashRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        ShowInteract("");

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = 0;
            damageFlashImage.color = c;
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth + " / " + maxHealth;
    }

    public void SetShifts(int shiftsLeft, int maxShifts)
    {
        if (shiftText != null)
            shiftText.text = "Shifts Left: " + shiftsLeft + " / " + maxShifts;
    }

    public void SetRoom(string roomName)
    {
        if (roomText != null)
            roomText.text = roomName;
    }

    public void SetObjective(string objective)
    {
        if (objectiveText != null)
            objectiveText.text = objective;
    }

    public void ShowInteract(string message)
    {
        if (interactText == null) return;

        bool hasMessage = !string.IsNullOrEmpty(message);

        interactText.gameObject.SetActive(hasMessage);
        interactText.text = message;
    }

    public void ShowDialogueLine(string line)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = line;
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void ShowDeath()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetPausePanel(bool isPaused)
    {
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void FlashDamage()
    {
        if (damageFlashImage == null) return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    IEnumerator DamageFlashRoutine()
    {
        Color c = damageFlashImage.color;
        c.a = 0.5f;
        damageFlashImage.color = c;

        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            c.a = Mathf.Lerp(0.5f, 0f, timer / flashDuration);
            damageFlashImage.color = c;

            yield return null;
        }

        c.a = 0f;
        damageFlashImage.color = c;
    }
}