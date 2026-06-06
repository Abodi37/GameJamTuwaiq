using System.Collections;
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

    [Header("Flags")]
    public string solvedFlag = "puzzle1Solved";

    [Header("Events")]
    public UnityEvent onCorrectCode;
    public UnityEvent onWrongCode;

    private int codeIndex;
    private bool isUsing;
    private bool solved;
    private bool doorIsOpening;

    private Vector2 dialInput;
    private float numberStepTimer;

    private CursorLockMode oldCursorLockMode;
    private bool oldCursorVisible;
    private float oldTimeScale;

    private Quaternion doorClosedRotation;
    private Quaternion doorOpenRotation;

    void Start()
    {
        if (safeVirtualCamera != null)
            safeVirtualCamera.SetActive(false);

        if (objectToShowAfterSolved != null)
            objectToShowAfterSolved.SetActive(false);

        if (dialPanel != null)
            dialPanel.SetActive(false);

        if (safeDoor != null)
        {
            doorClosedRotation = safeDoor.localRotation;
            doorOpenRotation = Quaternion.Euler(openDoorLocalRotation);
        }

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

        if (safeVirtualCamera == null)
        {
            Debug.LogError("SafeDialPuzzle: Safe Virtual Camera is missing.");
            return;
        }

        Debug.Log("SafeDialPuzzle: OpenDial started.");

        isUsing = true;
        codeIndex = 0;
        currentNumber = 0;
        dialInput = Vector2.zero;
        numberStepTimer = 0f;

        oldCursorLockMode = Cursor.lockState;
        oldCursorVisible = Cursor.visible;
        oldTimeScale = Time.timeScale;

        SetDisabledScripts(false);

        if (playerVirtualCamera != null)
            playerVirtualCamera.SetActive(false);

        safeVirtualCamera.SetActive(true);

        if (dialPanel != null)
            dialPanel.SetActive(true);

        if (freezeTimeWhileUsing)
            Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateText("A / D rotate   Space confirm   Esc close");
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

        Debug.Log("SafeDialPuzzle: Confirmed " + currentNumber + ". Need " + correctCode[codeIndex]);

        if (currentNumber == correctCode[codeIndex])
        {
            codeIndex++;

            if (codeIndex >= correctCode.Length)
            {
                SolvePuzzle();
                return;
            }

            UpdateText("Correct. Choose next number.");
        }
        else
        {
            codeIndex = 0;
            currentNumber = 0;

            onWrongCode.Invoke();

            UpdateText("Wrong. The dial reset.");
            Debug.Log("SafeDialPuzzle: Wrong code. Reset.");
        }
    }

    void SolvePuzzle()
    {
        Debug.Log("SafeDialPuzzle: Correct code entered. Opening safe door.");

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

        CloseDial();

        if (openDoorOnSolved && safeDoor != null && !doorIsOpening)
            StartCoroutine(OpenDoorRoutine());
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
    }

    void UpdateText(string extraMessage)
    {
        if (displayText == null) return;

        int total = correctCode != null ? correctCode.Length : 0;

        displayText.text =
            "NUMBER: " + currentNumber +
            "\nENTERED: " + codeIndex + " / " + total +
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