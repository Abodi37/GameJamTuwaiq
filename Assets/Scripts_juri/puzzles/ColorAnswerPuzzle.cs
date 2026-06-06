using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ColorAnswerPuzzle : MonoBehaviour
{
    [Header("Question")]
    public GameObject questionPanel;
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public string question = "What was the color on the second visit?";

    [Header("Answer")]
    public ColorMemoryObject memoryObject;
    public int correctVisitNumber = 2;
    public string fallbackCorrectAnswer = "Blue";

    [Header("Attempts")]
    public int maxAttempts = 3;
    public TMP_Text feedbackText;
    public PlayerStats playerStats;

    [Header("Flags")]
    public string requiredFlag = "puzzle1Solved";
    public string solvedFlag = "puzzle2Solved";

    [Header("Events")]
    public UnityEvent onSolved;
    public UnityEvent onWrong;
    public UnityEvent onFailedAllAttempts;

    private bool solved;
    private bool isOpen;
    private int attemptsLeft;

    void Start()
    {
        attemptsLeft = maxAttempts;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        UpdateQuestionText();
        UpdateFeedbackText("");
    }

    void Update()
    {
        if (!isOpen || solved) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseQuestion();
            return;
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            SubmitFromInput();
        }
    }

    public void OpenQuestion()
    {
        if (solved) return;

        if (GameManager.Instance != null && !GameManager.Instance.HasFlag(requiredFlag))
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.StartDialogue(new string[]
                {
                    "The device is asleep.",
                    "Maybe something in the garage explains what it wants."
                });
            }

            return;
        }

        isOpen = true;

        if (questionPanel != null)
            questionPanel.SetActive(true);

        if (answerInput != null)
        {
            answerInput.text = "";
            answerInput.Select();
            answerInput.ActivateInputField();
        }

        UpdateQuestionText();
        UpdateFeedbackText("Type the color, then press Enter or the Submit button.");

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseQuestion()
    {
        isOpen = false;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        if (!IsPlayerDead())
            Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SubmitFromInput()
    {
        if (!isOpen || solved) return;
        if (answerInput == null) return;

        SubmitAnswer(answerInput.text);
    }

    public void SubmitAnswer(string answer)
    {
        if (!isOpen || solved) return;

        answer = NormalizeAnswer(answer);

        if (string.IsNullOrEmpty(answer))
        {
            UpdateFeedbackText("Write an answer first.");
            FocusInput();
            return;
        }

        string correctAnswer = GetCorrectAnswer();

        if (answer == NormalizeAnswer(correctAnswer))
        {
            solved = true;

            if (GameManager.Instance != null)
                GameManager.Instance.SetFlag(solvedFlag, true);

            if (UIManager.Instance != null)
                UIManager.Instance.SetObjective("A key will appear only when the shift count is correct.");

            onSolved.Invoke();
            CloseQuestion();
        }
        else
        {
            attemptsLeft--;
            attemptsLeft = Mathf.Clamp(attemptsLeft, 0, maxAttempts);

            onWrong.Invoke();

            if (attemptsLeft <= 0)
            {
                UpdateFeedbackText("Wrong answer. No attempts left.");
                onFailedAllAttempts.Invoke();
                KillPlayer();
                return;
            }

            UpdateQuestionText();
            UpdateFeedbackText("Wrong answer. Attempts left: " + attemptsLeft);

            if (answerInput != null)
                answerInput.text = "";

            FocusInput();
        }
    }

    string GetCorrectAnswer()
    {
        if (memoryObject != null)
            return memoryObject.GetColorNameForVisit(correctVisitNumber);

        return fallbackCorrectAnswer;
    }

    string NormalizeAnswer(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        return value.Trim().ToLowerInvariant();
    }

    void UpdateQuestionText()
    {
        if (questionText != null)
            questionText.text = question + "\nAttempts left: " + attemptsLeft + " / " + maxAttempts;
    }

    void UpdateFeedbackText(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }

    void FocusInput()
    {
        if (answerInput == null) return;

        answerInput.Select();
        answerInput.ActivateInputField();
    }

    void KillPlayer()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        CloseQuestion();

        if (playerStats != null)
            playerStats.Die();
        else if (UIManager.Instance != null)
            UIManager.Instance.ShowDeath();
    }

    bool IsPlayerDead()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        return playerStats != null && playerStats.IsDead();
    }
    public void OnSubmitAnswer(InputAction.CallbackContext context)
{
    if (!context.performed) return;
    SubmitFromInput();
}
}