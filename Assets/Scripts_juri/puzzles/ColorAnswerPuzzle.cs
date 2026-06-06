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
    public string question = "What was the color on the second visit?";

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

        UpdateQuestionText();
        UpdateFeedbackText("");
    }

    public void OpenQuestion()
    {
        if (solved)
            return;

        if (requirePreviousPuzzleSolved)
        {
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
        UpdateFeedbackText("اكتبي الإجابة ثم اضغطي Submit أو Enter.");

        Debug.Log("ColorAnswerPuzzle: Opened. Correct answer is: " + GetCorrectAnswer());
    }

    public void CloseQuestion()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        Time.timeScale = oldTimeScale;
        Cursor.lockState = oldCursorLockMode;
        Cursor.visible = oldCursorVisible;

        SetDisabledScripts(true);
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
            return;
        }

        if (solved)
            return;

        string playerAnswer = NormalizeAnswer(answer);
        string correctAnswer = NormalizeAnswer(GetCorrectAnswer());

        Debug.Log("ColorAnswerPuzzle: Player answer = [" + playerAnswer + "]");
        Debug.Log("ColorAnswerPuzzle: Correct answer = [" + correctAnswer + "]");

        if (string.IsNullOrEmpty(playerAnswer))
        {
            UpdateFeedbackText("اكتبي إجابة أول.");
            FocusInput();
            return;
        }

        if (playerAnswer == correctAnswer)
        {
            solved = true;

            if (GameManager.Instance != null)
                GameManager.Instance.SetFlag(solvedFlag, true);

            if (UIManager.Instance != null)
                UIManager.Instance.SetObjective("A key will appear only when the shift count is correct.");

            UpdateFeedbackText("Correct.");
            onSolved.Invoke();

            CloseQuestion();
            return;
        }

        attemptsLeft--;
        attemptsLeft = Mathf.Clamp(attemptsLeft, 0, maxAttempts);

        onWrong.Invoke();

        if (attemptsLeft <= 0)
        {
            UpdateFeedbackText("غلط. خلصت المحاولات.");
            onFailedAllAttempts.Invoke();

            CloseQuestion();
            KillPlayer();
            return;
        }

        UpdateQuestionText();
        UpdateFeedbackText("غلط. باقي محاولات: " + attemptsLeft);

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
            playerStats = FindObjectOfType<PlayerStats>();

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
}