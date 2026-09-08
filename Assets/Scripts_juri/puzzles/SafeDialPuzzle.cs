using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SafeDialPuzzle : MonoBehaviour
{
    [Header("Code")]
    public string correctCode = "486";

    [Header("Cinemachine Cameras Optional")]
    public GameObject playerVirtualCamera;
    public GameObject safeVirtualCamera;

    [Header("UI")]
    public GameObject safePuzzlePanel;
    public TMP_InputField codeInput;
    public TMP_Text feedbackText;

    [Header("Safe Door")]
    public Transform safeDoor;
    public bool openDoorOnSolved = true;
    public Vector3 openDoorLocalRotation = new Vector3(0f, -90f, 0f);
    public float doorOpenDuration = 0.8f;

    [Header("Reward Optional")]
    public GameObject objectToShowAfterSolved;
    public GameObject objectToHideAfterSolved;

    [Header("Player")]
    public Behaviour[] disableWhileOpen;

    [Header("Flags")]
    public string solvedFlag = "puzzle1Solved";

    [Header("Events")]
    public UnityEvent onCorrectCode;
    public UnityEvent onWrongCode;

    private bool isOpen;
    private bool solved;
    private bool doorIsOpening;

    private CursorLockMode oldCursorLockMode;
    private bool oldCursorVisible;
    private float oldTimeScale;

    void Start()
    {
        if (safePuzzlePanel != null)
            safePuzzlePanel.SetActive(false);

        if (safeVirtualCamera != null)
            safeVirtualCamera.SetActive(false);

        if (objectToShowAfterSolved != null)
            objectToShowAfterSolved.SetActive(false);

        SetFeedback("");
    }

    public void OpenDial()
    {
        if (solved)
            return;

        if (isOpen)
            return;

        isOpen = true;

        oldCursorLockMode = Cursor.lockState;
        oldCursorVisible = Cursor.visible;
        oldTimeScale = Time.timeScale;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideDialogue();
            UIManager.Instance.ShowInteract("");
        }

        if (playerVirtualCamera != null)
            playerVirtualCamera.SetActive(false);

        if (safeVirtualCamera != null)
            safeVirtualCamera.SetActive(true);

        if (safePuzzlePanel != null)
            safePuzzlePanel.SetActive(true);

        SetDisabledScripts(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (codeInput != null)
        {
            codeInput.text = "";
            codeInput.Select();
            codeInput.ActivateInputField();
        }

        SetFeedback("");
    }

    public void CloseDial()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (safePuzzlePanel != null)
            safePuzzlePanel.SetActive(false);

        if (safeVirtualCamera != null)
            safeVirtualCamera.SetActive(false);

        if (playerVirtualCamera != null)
            playerVirtualCamera.SetActive(true);

        Time.timeScale = oldTimeScale;
        Cursor.lockState = oldCursorLockMode;
        Cursor.visible = oldCursorVisible;

        SetDisabledScripts(true);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteract("");
    }

    public void SubmitFromInput()
    {
        if (!isOpen || solved)
            return;

        if (codeInput == null)
        {
            Debug.LogError("SafeDialPuzzle: Code Input is not assigned.");
            return;
        }

        SubmitCode(codeInput.text);
    }

    public void SubmitCode(string playerCode)
    {
        if (!isOpen || solved)
            return;

        string cleanPlayerCode = NormalizeCode(playerCode);
        string cleanCorrectCode = NormalizeCode(correctCode);

        GameLog.Log("SafeDialPuzzle: Player code = [" + cleanPlayerCode + "]");
        GameLog.Log("SafeDialPuzzle: Correct code = [" + cleanCorrectCode + "]");

        if (string.IsNullOrEmpty(cleanPlayerCode))
        {
            SetFeedback("Enter the code first.");
            FocusInput();
            return;
        }

        if (cleanPlayerCode == cleanCorrectCode)
        {
            SolvePuzzle();
            return;
        }

        onWrongCode.Invoke();

        SetFeedback("Wrong code.");
        
        if (codeInput != null)
            codeInput.text = "";

        FocusInput();
    }

    void SolvePuzzle()
{
    solved = true;

    if (GameManager.Instance != null)
        GameManager.Instance.SetFlag(solvedFlag, true);

    if (UIManager.Instance != null)
    {
        UIManager.Instance.SetObjective("The safe opened. Remember the color of the second visit.");
        UIManager.Instance.HideDialogue();
        UIManager.Instance.ShowInteract("");
    }

    if (objectToShowAfterSolved != null)
        objectToShowAfterSolved.SetActive(true);

    if (objectToHideAfterSolved != null)
        objectToHideAfterSolved.SetActive(false);

    onCorrectCode.Invoke();

    CloseDial();

    InteractableObject safeInteractable = GetComponent<InteractableObject>();
    if (safeInteractable != null)
        safeInteractable.enabled = false;

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

    string NormalizeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        value = value.Trim();

        value = value.Replace(" ", "");
        value = value.Replace("-", "");
        value = value.Replace("_", "");
        value = value.Replace(",", "");
        value = value.Replace(".", "");

        return value;
    }

    void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }

    void FocusInput()
    {
        if (codeInput == null)
            return;

        codeInput.Select();
        codeInput.ActivateInputField();
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

    public void OnConfirmInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        SubmitFromInput();
    }

    public void OnCloseInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (isOpen)
            CloseDial();
    }
}