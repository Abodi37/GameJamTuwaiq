using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SafeDialPuzzle : MonoBehaviour
{
    [Header("Code")]
    public int[] correctCode = { 4, 8, 6 };

    [Header("Cinemachine Cameras")]
    public GameObject playerVirtualCamera;
    public GameObject safeVirtualCamera;

    [Tooltip("حطي هنا PlayerMovements أو سكربت النظر فقط. لا تحطين PlayerInput.")]
    public Behaviour[] disableWhileUsing;

    [Header("Dial")]
    public Transform dialTransform;
    public Vector3 dialRotationAxis = Vector3.forward;
    public bool invertDirection = false;
    public float visualRotateSpeed = 140f;
    public float numberStepDelay = 0.15f;
    public int currentNumber = 0;
    public int maxNumber = 9;

    [Header("Safe Door")]
    public Transform safeDoor;
    public bool openDoorOnSolved = true;
    public Vector3 openDoorLocalRotation = new Vector3(0f, -90f, 0f);
    public float doorOpenDuration = 0.8f;

    [Header("Reward Optional")]
    public GameObject objectToShowAfterSolved;
    public GameObject objectToHideAfterSolved;

    [Header("UI Optional")]
    public GameObject dialPanel;
    public TMP_Text displayText;

    [Header("Settings")]
    public bool freezeTimeWhileUsing = false;
    public bool closeViewAfterSolved = true;

    [Header("Flags")]
    public string solvedFlag = "puzzle1Solved";

    [Header("Events")]
    public UnityEvent onCorrectCode;
    public UnityEvent onWrongCode;

    private bool isUsing;
    private bool solved;
    private bool doorIsOpening;

    private List<int> enteredNumbers = new List<int>();
    private Vector2 dialInput;
    private float numberStepTimer;

    private CursorLockMode oldCursorLockMode;
    private bool oldCursorVisible;
    private float oldTimeScale;

    void Start()
    {
        if (safeVirtualCamera != null)
            safeVirtualCamera.SetActive(false);

        if (objectToShowAfterSolved != null)
            objectToShowAfterSolved.SetActive(false);

        if (dialPanel != null)
            dialPanel.SetActive(false);

        UpdateText("Press E to inspect the safe.");
    }

    void Update()
    {
        if (!isUsing || solved) return;

        if (Mathf.Abs(dialInput.x) > 0.2f)
        {
            int direction = dialInput.x > 0f ? 1 : -1;
            RotateDial(direction);
        }
        else
        {
            numberStepTimer = 0f;
        }
    }

    public void OpenDial()
    {
        if (solved)
        {
            Debug.Log("SafeDialPuzzle: Safe already solved.");
            return;
        }

        if (isUsing) return;

        isUsing = true;
        currentNumber = 0;
        dialInput = Vector2.zero;
        numberStepTimer = 0f;
        enteredNumbers.Clear();

        oldCursorLockMode = Cursor.lockState;
        oldCursorVisible = Cursor.visible;
        oldTimeScale = Time.timeScale;

        SetDisabledScripts(false);

        if (playerVirtualCamera != null)
            playerVirtualCamera.SetActive(false);

        if (safeVirtualCamera != null)
            safeVirtualCamera.SetActive(true);
        else
            Debug.LogWarning("SafeDialPuzzle: Safe Virtual Camera is empty. The puzzle still works, but camera will not zoom.");

        if (dialPanel != null)
            dialPanel.SetActive(true);

        if (freezeTimeWhileUsing)
            Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateText("A / D rotate   Space confirm   Esc close");

        Debug.Log("SafeDialPuzzle: Opened.");
    }

    public void CloseDial()
    {
        if (!isUsing) return;

        isUsing = false;
        dialInput = Vector2.zero;

        if (safeVirtualCamera != null)
            safeVirtualCamera.SetActive(false);

        if (playerVirtualCamera != null)
            playerVirtualCamera.SetActive(true);

        if (dialPanel != null)
            dialPanel.SetActive(false);

        if (freezeTimeWhileUsing)
            Time.timeScale = oldTimeScale;

        Cursor.lockState = oldCursorLockMode;
        Cursor.visible = oldCursorVisible;

        SetDisabledScripts(true);

        Debug.Log("SafeDialPuzzle: Closed.");
    }

    void SetDisabledScripts(bool value)
    {
        if (disableWhileUsing == null) return;

        for (int i = 0; i < disableWhileUsing.Length; i++)
        {
            if (disableWhileUsing[i] != null)
                disableWhileUsing[i].enabled = value;
        }
    }

    void RotateDial(int direction)
    {
        int visualDirection = invertDirection ? -direction : direction;

        if (dialTransform != null)
        {
            dialTransform.Rotate(
                dialRotationAxis * visualDirection * visualRotateSpeed * Time.unscaledDeltaTime,
                Space.Self
            );
        }

        numberStepTimer -= Time.unscaledDeltaTime;

        if (numberStepTimer <= 0f)
        {
            currentNumber += direction;

            if (currentNumber > maxNumber)
                currentNumber = 0;

            if (currentNumber < 0)
                currentNumber = maxNumber;

            numberStepTimer = numberStepDelay;

            UpdateText("A / D rotate   Space confirm   Esc close");
            Debug.Log("SafeDialPuzzle: Current number = " + currentNumber);
        }
    }

    public void ConfirmNumber()
    {
        if (!isUsing) return;
        if (solved) return;

        if (correctCode == null || correctCode.Length == 0)
        {
            Debug.LogError("SafeDialPuzzle: Correct Code is empty.");
            return;
        }

        enteredNumbers.Add(currentNumber);

        Debug.Log("SafeDialPuzzle: Added number " + currentNumber);
        Debug.Log("SafeDialPuzzle: Entered code = " + GetEnteredCodeText());

        UpdateText("Number saved.");

        if (enteredNumbers.Count < correctCode.Length)
            return;

        if (IsEnteredCodeCorrect())
        {
            SolvePuzzle();
        }
        else
        {
            Debug.Log("SafeDialPuzzle: Wrong code. Entered = " + GetEnteredCodeText());

            enteredNumbers.Clear();
            currentNumber = 0;

            onWrongCode.Invoke();

            UpdateText("Wrong code. Try again.");
        }
    }

    bool IsEnteredCodeCorrect()
    {
        if (enteredNumbers.Count != correctCode.Length)
            return false;

        for (int i = 0; i < correctCode.Length; i++)
        {
            if (enteredNumbers[i] != correctCode[i])
                return false;
        }

        return true;
    }

    void SolvePuzzle()
    {
        Debug.Log("SafeDialPuzzle: Correct code entered. Opening safe.");

        solved = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetFlag(solvedFlag, true);
            GameManager.Instance.AddItem("Cabinet Note");
        }

        if (UIManager.Instance != null)
            UIManager.Instance.SetObjective("The safe opened. Remember the color of the second visit.");

        if (objectToShowAfterSolved != null)
            objectToShowAfterSolved.SetActive(true);

        if (objectToHideAfterSolved != null)
            objectToHideAfterSolved.SetActive(false);

        onCorrectCode.Invoke();

        if (openDoorOnSolved && safeDoor != null && !doorIsOpening)
            StartCoroutine(OpenDoorRoutine());

        if (closeViewAfterSolved)
            CloseDial();
        else
            UpdateText("Correct. Safe opened.");
    }

    IEnumerator OpenDoorRoutine()
    {
        doorIsOpening = true;

        Quaternion startRotation = safeDoor.localRotation;
        Quaternion targetRotation = Quaternion.Euler(openDoorLocalRotation);

        float timer = 0f;

        while (timer < doorOpenDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / doorOpenDuration);
            t = t * t * (3f - 2f * t);

            safeDoor.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        safeDoor.localRotation = targetRotation;
        doorIsOpening = false;

        Debug.Log("SafeDialPuzzle: Door opened.");
    }

    string GetEnteredCodeText()
    {
        if (enteredNumbers.Count == 0)
            return "Empty";

        string result = "";

        for (int i = 0; i < enteredNumbers.Count; i++)
        {
            result += enteredNumbers[i].ToString();

            if (i < enteredNumbers.Count - 1)
                result += " ";
        }

        return result;
    }

    void UpdateText(string extraMessage)
    {
        if (displayText == null) return;

        int total = correctCode != null ? correctCode.Length : 0;

        displayText.text =
            "CURRENT NUMBER: " + currentNumber +
            "\nENTERED: " + GetEnteredCodeText() +
            "\nCOUNT: " + enteredNumbers.Count + " / " + total +
            "\n" + extraMessage;
    }

    public void OnDialMove(InputAction.CallbackContext context)
    {
        if (!isUsing) return;

        dialInput = context.ReadValue<Vector2>();

        if (context.canceled)
            dialInput = Vector2.zero;
    }

    public void OnConfirmInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ConfirmNumber();
    }

    public void OnCloseInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        CloseDial();
    }
}