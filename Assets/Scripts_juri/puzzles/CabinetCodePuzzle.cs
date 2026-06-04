using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CabinetCodePuzzle : MonoBehaviour
{
    [Header("Puzzle Code")]
    public string correctCode = "486";
    private string currentInput = "";

    [Header("UI")]
    public GameObject keypadPanel;
    public TMP_Text inputText;

    [Header("Flags")]
    public string solvedFlag = "puzzle1Solved";

    [Header("Events")]
    public UnityEvent onCorrectCode;
    public UnityEvent onWrongCode;

    private bool solved = false;

    void Start()
    {
        if (keypadPanel != null)
            keypadPanel.SetActive(false);

        UpdateText();
    }

    public void OpenKeypad()
    {
        if (solved) return;

        currentInput = "";
        UpdateText();

        if (keypadPanel != null)
            keypadPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseKeypad()
    {
        if (keypadPanel != null)
            keypadPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PressNumber(string number)
    {
        if (solved) return;

        if (currentInput.Length >= correctCode.Length)
            return;

        currentInput += number;
        UpdateText();

        if (currentInput.Length == correctCode.Length)
            CheckCode();
    }

    public void ClearInput()
    {
        currentInput = "";
        UpdateText();
    }

    void CheckCode()
    {
        if (currentInput == correctCode)
        {
            solved = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetFlag(solvedFlag, true);
                GameManager.Instance.AddItem("Cabinet Note");
            }

            if (UIManager.Instance != null)
                UIManager.Instance.SetObjective("The cabinet note says: remember the color of the second visit.");

            onCorrectCode.Invoke();
            CloseKeypad();
        }
        else
        {
            onWrongCode.Invoke();
            currentInput = "";
            UpdateText();
        }
    }

    void UpdateText()
    {
        if (inputText != null)
            inputText.text = currentInput;
    }
}