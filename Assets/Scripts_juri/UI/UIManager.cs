using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main UI Text")]
    public TMP_Text healthText;
    public TMP_Text shiftText;
    public TMP_Text roomText;
    public TMP_Text inventoryText;

    [Header("Main UI Images Optional")]
    public Image healthFillImage;
    public Image shiftsFillImage;
    public Image cookieIcon;
    public Image keyIcon;

    [Header("Objective UI")]
    public GameObject objectivePanel;
    public TMP_Text objectiveText;

    [Header("Interact UI")]
    public TMP_Text interactText;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public GameObject dialogueContinueIcon;

    [Header("Panels")]
    public GameObject cookiePanel;
    public GameObject safePuzzlePanel;
    public GameObject colorPuzzlePanel;
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
        if (objectivePanel != null)
            objectivePanel.SetActive(true);

        HideDialogue();

        if (safePuzzlePanel != null)
            safePuzzlePanel.SetActive(false);

        if (colorPuzzlePanel != null)
            colorPuzzlePanel.SetActive(false);

        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        ShowInteract("");
        SetCookieIcon(false);
        SetKeyIcon(false);
        SetInventoryText("Inventory: Empty");

        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = 0f;
            damageFlashImage.color = c;
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth + " / " + maxHealth;

        if (healthFillImage != null)
        {
            float value = 0f;

            if (maxHealth > 0)
                value = (float)currentHealth / maxHealth;

            healthFillImage.fillAmount = Mathf.Clamp01(value);
        }
    }

    public void SetShifts(int shiftsLeft, int maxShifts)
    {
        if (shiftText != null)
            shiftText.text = "Shifts Left: " + shiftsLeft + " / " + maxShifts;

        if (shiftsFillImage != null)
        {
            float value = 0f;

            if (maxShifts > 0)
                value = (float)shiftsLeft / maxShifts;

            shiftsFillImage.fillAmount = Mathf.Clamp01(value);
        }
    }

    public void SetRoom(string roomName)
    {
        if (roomText != null)
            roomText.text = roomName;
    }

    public void SetObjective(string objective)
    {
        if (objectivePanel != null)
            objectivePanel.SetActive(true);

        if (objectiveText != null)
            objectiveText.text = objective;
    }

    public void ClearObjective()
    {
        if (objectiveText != null)
            objectiveText.text = "";
    }

    public void ShowInteract(string message)
    {
        if (interactText == null)
            return;

        bool hasMessage = !string.IsNullOrWhiteSpace(message);

        interactText.gameObject.SetActive(hasMessage);
        interactText.text = hasMessage ? message : "";
    }

    public void ShowDialogueLine(string line)
    {
        CloseAllPanelsExcept(dialoguePanel);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = line;

        if (dialogueContinueIcon != null)
            dialogueContinueIcon.SetActive(true);
    }

    public void HideDialogue()
    {
        if (dialogueText != null)
            dialogueText.text = "";

        if (dialogueContinueIcon != null)
            dialogueContinueIcon.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public bool IsDialogueVisible()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }

    public void CloseAllPanels()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (cookiePanel != null)
            cookiePanel.SetActive(false);

        if (safePuzzlePanel != null)
            safePuzzlePanel.SetActive(false);

        if (colorPuzzlePanel != null)
            colorPuzzlePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void CloseAllPanelsExcept(GameObject panelToKeepOpen)
    {
        if (dialoguePanel != null && dialoguePanel != panelToKeepOpen)
            dialoguePanel.SetActive(false);

        if (cookiePanel != null && cookiePanel != panelToKeepOpen)
            cookiePanel.SetActive(false);

        if (safePuzzlePanel != null && safePuzzlePanel != panelToKeepOpen)
            safePuzzlePanel.SetActive(false);

        if (colorPuzzlePanel != null && colorPuzzlePanel != panelToKeepOpen)
            colorPuzzlePanel.SetActive(false);

        if (pausePanel != null && pausePanel != panelToKeepOpen)
            pausePanel.SetActive(false);

        if (deathPanel != null && deathPanel != panelToKeepOpen)
            deathPanel.SetActive(false);
    }

    public void SetInventoryText(string text)
    {
        if (inventoryText != null)
            inventoryText.text = text;
    }

    public void SetCookieIcon(bool value)
    {
        if (cookieIcon != null)
            cookieIcon.gameObject.SetActive(value);
    }

    public void SetKeyIcon(bool value)
    {
        if (keyIcon != null)
            keyIcon.gameObject.SetActive(value);
    }

    public void ShowDeath()
    {
        CloseAllPanelsExcept(deathPanel);

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetPausePanel(bool isPaused)
    {
        if (isPaused)
        {
            CloseAllPanelsExcept(pausePanel);
        }

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        if (deathPanel != null && deathPanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void FlashDamage()
    {
        if (damageFlashImage == null)
            return;

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

            float t = 0f;

            if (flashDuration > 0f)
                t = timer / flashDuration;

            c.a = Mathf.Lerp(0.5f, 0f, t);
            damageFlashImage.color = c;

            yield return null;
        }

        c.a = 0f;
        damageFlashImage.color = c;
    }
}