using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class SafeDialPuzzle : MonoBehaviour
{
    [Header("Code")]
    public int[] correctCode = { 4, 8, 6 };

    [Header("Dial")]
    public Transform dialTransform;
    public float rotateSpeed = 120f;
    public int currentNumber = 0;
    public int maxNumber = 9;

    [Header("UI Optional")]
    public GameObject dialPanel;
    public TMP_Text displayText;

    [Header("Flags")]
    public string solvedFlag = "puzzle1Solved";

    [Header("Events")]
    public UnityEvent onCorrectCode;
    public UnityEvent onWrongCode;

    private int codeIndex = 0;
    private bool isUsing = false;
    private bool solved = false;

    void Start()
    {
        if (dialPanel != null)
            dialPanel.SetActive(false);

        UpdateText();
    }

    void Update()
    {
        if (!isUsing || solved) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDial();
            return;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            RotateDial(-1);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            RotateDial(1);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmNumber();
        }
    }

    public void OpenDial()
    {
        if (solved) return;

        isUsing = true;
        codeIndex = 0;
        currentNumber = 0;

        if (dialPanel != null)
            dialPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateText();
    }

    public void CloseDial()
    {
        isUsing = false;

        if (dialPanel != null)
            dialPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void RotateDial(int direction)
    {
        if (dialTransform != null)
        {
            dialTransform.Rotate(Vector3.forward * direction * rotateSpeed * Time.deltaTime, Space.Self);
        }

        // يغير الرقم تدريجيًا حسب الدوران
        if (Time.frameCount % 10 == 0)
        {
            currentNumber += direction;

            if (currentNumber > maxNumber)
                currentNumber = 0;

            if (currentNumber < 0)
                currentNumber = maxNumber;

            UpdateText();
        }
    }

    void ConfirmNumber()
    {
        if (correctCode == null || correctCode.Length == 0) return;

        if (currentNumber == correctCode[codeIndex])
        {
            codeIndex++;

            if (codeIndex >= correctCode.Length)
            {
                SolvePuzzle();
                return;
            }

            UpdateText();
        }
        else
        {
            codeIndex = 0;
            currentNumber = 0;
            onWrongCode.Invoke();
            UpdateText();
        }
    }

    void SolvePuzzle()
    {
        solved = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetFlag(solvedFlag, true);
            GameManager.Instance.AddItem("Cabinet Note");
        }

        if (UIManager.Instance != null)
            UIManager.Instance.SetObjective("The safe opened. Remember the color of the second visit.");

        onCorrectCode.Invoke();
        CloseDial();
    }

    void UpdateText()
    {
        if (displayText != null)
        {
            displayText.text =
                "NUMBER: " + currentNumber +
                "\nENTERED: " + codeIndex + " / " + correctCode.Length +
                "\nA/D Rotate   Space Confirm";
        }
    }
}