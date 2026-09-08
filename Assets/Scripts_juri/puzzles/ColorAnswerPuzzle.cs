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
    public TMP_Text feedbackText;

    [TextArea]
    public string question = "What was the color of the smallest bowl on the second visit?";

    [Header("Answer")]
    public ColorMemoryObject memoryObject;
    public int correctVisitNumber = 2;

    [Tooltip("Use this if Memory Object is empty.")]
    public string fallbackCorrectAnswer = "Blue";

    [Header("Attempts")]
    public int maxAttempts = 3;
    public PlayerStats playerStats;

    [Header("Flags")]
    public string requiredFlag = "puzzle1Solved";
    public string solvedFlag = "puzzle2Solved";

    [Header("Settings")]
    public bool requirePreviousPuzzleSolved = false;
    public Behaviour[] disableWhileOpen;

    [Header("Events")]
    public UnityEvent onSolved;
    public UnityEvent onWrong;
    public UnityEvent onFailedAllAttempts;

    private bool solved;
    private bool isOpen;
    private int attemptsLeft;

    private CursorLockMode oldCursorLockMode;
    private bool oldCursorVisible;
    private float oldTimeScale;

    void Start()
    {
        attemptsLeft = maxAttempts;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        ForceHideDialogue();

        UpdateQuestionText();
        UpdateFeedbackText("");
    }

    public void OpenQuestion()
    {
        if (solved)
            return;

        ForceHideDialogue();

        if (requirePreviousPuzzleSolved)
        {
            if (GameManager.Instance != null && !GameManager.Instance.HasFlag(requiredFlag))
            {
                if (UIManager.Instance != null)
                    UIManager.Instance.HideDialogue();

                return;
            }
        }

        isOpen = true;

        oldCursorLockMode = Cursor.lockState;
        oldCursorVisible = Cursor.visible;
        oldTimeScale = Time.timeScale;

        if (questionPanel != null)
            questionPanel.SetActive(true);

        SetDisabledScripts(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (answerInput != null)
        {
            answerInput.text = "";
            answerInput.Select();
            answerInput.ActivateInputField();
        }

        UpdateQuestionText();
        UpdateFeedbackText("Write the answer, then press Submit or Space.");

        GameLog.Log("ColorAnswerPuzzle: Opened.");
    }

    public void CloseQuestion()
    {
        if (!isOpen)
        {
            ForceHideDialogue();
            return;
        }

        isOpen = false;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        Time.timeScale = oldTimeScale;
        Cursor.lockState = oldCursorLockMode;
        Cursor.visible = oldCursorVisible;

        SetDisabledScripts(true);

        ForceHideDialogue();

        GameLog.Log("ColorAnswerPuzzle: Closed.");
    }

    public void SubmitFromInput()
    {
        if (answerInput == null)
        {
            Debug.LogError("ColorAnswerPuzzle: Answer Input is not assigned.");
            return;
        }

        SubmitAnswer(answerInput.text);
    }

    public void SubmitAnswer(string answer)
    {
        if (!isOpen)
        {
            Debug.LogWarning("ColorAnswerPuzzle: Submit ignored because puzzle is not open.");
            ForceHideDialogue();
            return;
        }

        if (solved)
            return;

        ForceHideDialogue();

        string playerAnswer = NormalizeAnswer(answer);
        string correctAnswer = NormalizeAnswer(GetCorrectAnswer());

        GameLog.Log("ColorAnswerPuzzle: Player answer = [" + playerAnswer + "]");
        GameLog.Log("ColorAnswerPuzzle: Correct answer = [" + correctAnswer + "]");

        if (string.IsNullOrEmpty(playerAnswer))
        {
            UpdateFeedbackText("Write an answer first.");
            FocusInput();
            return;
        }

        if (playerAnswer == correctAnswer)
        {
            solved = true;

            if (GameManager.Instance != null)
                GameManager.Instance.SetFlag(solvedFlag, true);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetObjective("A key will appear only when the shift count is correct.");
                UIManager.Instance.HideDialogue();
            }

            UpdateFeedbackText("Correct.");

            onSolved.Invoke();

            CloseQuestion();

            GameLog.Log("ColorAnswerPuzzle: Solved. Flag set = " + solvedFlag);
            return;
        }

        attemptsLeft--;
        attemptsLeft = Mathf.Clamp(attemptsLeft, 0, maxAttempts);

        onWrong.Invoke();

        if (attemptsLeft <= 0)
        {
            UpdateFeedbackText("Wrong. No attempts left.");

            onFailedAllAttempts.Invoke();

            CloseQuestion();
            KillPlayer();
            return;
        }

        UpdateQuestionText();
        UpdateFeedbackText("Wrong. Attempts left: " + attemptsLeft);

        if (answerInput != null)
            answerInput.text = "";

        FocusInput();
    }

    public void OnSubmitAnswer(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        SubmitFromInput();
    }

    public void OnCloseQuestion(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (isOpen)
            CloseQuestion();
        else
            ForceHideDialogue();
    }

    string GetCorrectAnswer()
    {
        if (memoryObject != null)
        {
            string memoryAnswer = memoryObject.GetColorNameForVisit(correctVisitNumber);

            if (!string.IsNullOrWhiteSpace(memoryAnswer))
                return memoryAnswer;
        }

        return fallbackCorrectAnswer;
    }

    string NormalizeAnswer(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        value = value.Trim().ToLowerInvariant();

        value = value.Replace(" ", "");
        value = value.Replace("_", "");
        value = value.Replace("-", "");

        value = value.Replace("أ", "ا");
        value = value.Replace("إ", "ا");
        value = value.Replace("آ", "ا");
        value = value.Replace("ى", "ي");
        value = value.Replace("ة", "ه");

        value = value.Replace("َ", "");
        value = value.Replace("ُ", "");
        value = value.Replace("ِ", "");
        value = value.Replace("ً", "");
        value = value.Replace("ٌ", "");
        value = value.Replace("ٍ", "");
        value = value.Replace("ْ", "");
        value = value.Replace("ّ", "");

        if (value == "ازرق" || value == "زرق" || value == "blue")
            return "blue";

        if (value == "احمر" || value == "حمر" || value == "red")
            return "red";

        if (value == "اخضر" || value == "خضر" || value == "green")
            return "green";

        if (value == "اصفر" || value == "صفر" || value == "yellow")
            return "yellow";

        if (value == "اسود" || value == "black")
            return "black";

        if (value == "ابيض" || value == "white")
            return "white";

        if (value == "بنفسجي" || value == "purple")
            return "purple";

        if (value == "وردي" || value == "pink")
            return "pink";

        if (value == "برتقالي" || value == "orange")
            return "orange";

        return value;
    }

    void UpdateQuestionText()
    {
        if (questionText != null)
        {
            questionText.text =
                question +
                "\nAttempts left: " + attemptsLeft + " / " + maxAttempts;
        }
    }

    void UpdateFeedbackText(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }

    void FocusInput()
    {
        if (answerInput == null)
            return;

        answerInput.Select();
        answerInput.ActivateInputField();
    }

    void KillPlayer()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
            playerStats.Die();
        else if (UIManager.Instance != null)
            UIManager.Instance.ShowDeath();
        else
            Debug.LogWarning("ColorAnswerPuzzle: No PlayerStats found, cannot kill player.");
    }

    void SetDisabledScripts(bool value)
    {
        if (disableWhileOpen == null)
            return;

        for (int i = 0; i < disableWhileOpen.Length; i++)
        {
            if (disableWhileOpen[i] != null)
                disableWhileOpen[i].enabled = value;
        }
    }

    void ForceHideDialogue()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.HideDialogue();
    }
}