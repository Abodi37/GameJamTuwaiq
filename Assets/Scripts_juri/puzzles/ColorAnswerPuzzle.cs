using UnityEngine;
using TMPro;
using UnityEngine.Events;

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

    [Header("Flags")]
    public string requiredFlag = "puzzle1Solved";
    public string solvedFlag = "puzzle2Solved";

    [Header("Events")]
    public UnityEvent onSolved;
    public UnityEvent onWrong;

    private bool solved;

    void Start()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);
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

        if (questionPanel != null)
            questionPanel.SetActive(true);

        if (questionText != null)
            questionText.text = question;

        if (answerInput != null)
            answerInput.text = "";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseQuestion()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SubmitFromInput()
    {
        if (answerInput == null) return;
        SubmitAnswer(answerInput.text);
    }

    public void SubmitAnswer(string answer)
    {
        if (solved) return;

        string correctAnswer = fallbackCorrectAnswer;

        if (memoryObject != null)
            correctAnswer = memoryObject.GetColorNameForVisit(correctVisitNumber);

        if (answer.Trim().ToLower() == correctAnswer.Trim().ToLower())
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
            onWrong.Invoke();

            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.StartDialogue(new string[]
                {
                    "That memory does not fit.",
                    "Think back to the second time you entered this room."
                });
            }
        }
    }
}