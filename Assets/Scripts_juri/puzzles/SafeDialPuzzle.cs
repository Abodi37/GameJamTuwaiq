using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SafeDialPuzzle : MonoBehaviour
{
    [Header("Code")]
    public int[] correctCode = { 4, 8, 6 };

    [Header("Camera Zoom")]
    public Camera playerCamera;
    public Transform safeViewPoint;
    public float zoomDuration = 0.35f;

    [Header("Dial")]
    public Transform dialTransform;
    public Vector3 dialRotationAxis = Vector3.forward;
    public bool invertDirection = false;
    public float visualRotateSpeed = 140f;
    public float numberStepDelay = 0.15f;
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

    private Vector2 dialInput;
    private float numberStepTimer;

    private Transform originalCameraParent;
    private Vector3 originalCameraLocalPosition;
    private Quaternion originalCameraLocalRotation;
    private Vector3 originalCameraWorldPosition;
    private Quaternion originalCameraWorldRotation;

    private CursorLockMode oldCursorLockMode;
    private bool oldCursorVisible;

    private Coroutine cameraRoutine;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (dialPanel != null)
            dialPanel.SetActive(false);

        UpdateText("Press E to inspect the safe.");
    }

    void Update()
    {
        if (!isUsing || solved) return;

        float horizontal = dialInput.x;

        if (Mathf.Abs(horizontal) > 0.2f)
        {
            int direction = horizontal > 0f ? 1 : -1;
            RotateDial(direction);
        }
        else
        {
            numberStepTimer = 0f;
        }
    }

    public void OpenDial()
    {
        if (solved) return;
        if (isUsing) return;

        isUsing = true;
        codeIndex = 0;
        currentNumber = 0;
        dialInput = Vector2.zero;
        numberStepTimer = 0f;

        SaveCameraAndCursor();

        if (dialPanel != null)
            dialPanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null && safeViewPoint != null)
        {
            StartCameraMove(
                playerCamera.transform.position,
                playerCamera.transform.rotation,
                safeViewPoint.position,
                safeViewPoint.rotation
            );
        }

        UpdateText("A / D rotate   Space confirm   Esc close");
    }

    public void CloseDial()
    {
        if (!isUsing) return;

        isUsing = false;
        dialInput = Vector2.zero;

        if (dialPanel != null)
            dialPanel.SetActive(false);

        if (playerCamera != null)
        {
            StartCoroutine(CloseCameraRoutine());
        }
        else
        {
            FinishClose();
        }
    }

    IEnumerator CloseCameraRoutine()
    {
        Vector3 targetPosition = GetOriginalCameraWorldPosition();
        Quaternion targetRotation = GetOriginalCameraWorldRotation();

        yield return MoveCameraRoutine(
            playerCamera.transform.position,
            playerCamera.transform.rotation,
            targetPosition,
            targetRotation
        );

        FinishClose();
    }

    void FinishClose()
    {
        Time.timeScale = 1f;

        Cursor.lockState = oldCursorLockMode;
        Cursor.visible = oldCursorVisible;

        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = originalCameraLocalPosition;
            playerCamera.transform.localRotation = originalCameraLocalRotation;
        }
    }

    void SaveCameraAndCursor()
    {
        oldCursorLockMode = Cursor.lockState;
        oldCursorVisible = Cursor.visible;

        if (playerCamera == null) return;

        originalCameraParent = playerCamera.transform.parent;
        originalCameraLocalPosition = playerCamera.transform.localPosition;
        originalCameraLocalRotation = playerCamera.transform.localRotation;
        originalCameraWorldPosition = playerCamera.transform.position;
        originalCameraWorldRotation = playerCamera.transform.rotation;
    }

    Vector3 GetOriginalCameraWorldPosition()
    {
        if (originalCameraParent != null)
            return originalCameraParent.TransformPoint(originalCameraLocalPosition);

        return originalCameraWorldPosition;
    }

    Quaternion GetOriginalCameraWorldRotation()
    {
        if (originalCameraParent != null)
            return originalCameraParent.rotation * originalCameraLocalRotation;

        return originalCameraWorldRotation;
    }

    void StartCameraMove(Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot)
    {
        if (cameraRoutine != null)
            StopCoroutine(cameraRoutine);

        cameraRoutine = StartCoroutine(MoveCameraRoutine(fromPos, fromRot, toPos, toRot));
    }

    IEnumerator MoveCameraRoutine(Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot)
    {
        if (playerCamera == null)
            yield break;

        float timer = 0f;

        while (timer < zoomDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / zoomDuration);
            t = t * t * (3f - 2f * t);

            playerCamera.transform.position = Vector3.Lerp(fromPos, toPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(fromRot, toRot, t);

            yield return null;
        }

        playerCamera.transform.position = toPos;
        playerCamera.transform.rotation = toRot;
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
        }
    }

    public void ConfirmNumber()
    {
        if (!isUsing) return;
        if (solved) return;
        if (correctCode == null || correctCode.Length == 0) return;

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

    void UpdateText(string extraMessage)
    {
        if (displayText == null) return;

        int total = 0;

        if (correctCode != null)
            total = correctCode.Length;

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