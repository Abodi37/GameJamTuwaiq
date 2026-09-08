using UnityEngine;

public class ShowPaperAtShift : MonoBehaviour
{
    [Header("Paper")]
    public GameObject paperObject;

    [Header("Condition")]
    [Tooltip("Room 3 is usually index 2 if your rooms start from 0.")]
    public int requiredRoomIndex = 2;

    public int requiredShiftsLeft = 5;

    [Header("Debug")]
    public bool showDebugLogs = false;

    [Tooltip("Seconds between checks. Room and shift count only change a handful of times per run.")]
    public float checkInterval = 0.25f;

    private float checkTimer;

    void Start()
    {
        UpdatePaperVisibility();
    }

    void Update()
    {
        checkTimer -= Time.deltaTime;

        if (checkTimer > 0f)
            return;

        checkTimer = checkInterval;

        UpdatePaperVisibility();
    }

    void UpdatePaperVisibility()
    {
        if (paperObject == null)
            return;

        if (GameManager.Instance == null)
        {
            paperObject.SetActive(false);
            return;
        }

        bool isCorrectRoom =
            GameManager.Instance.currentRoomIndex == requiredRoomIndex;

        bool isCorrectShift =
            GameManager.Instance.currentShiftsLeft == requiredShiftsLeft;

        bool shouldShow = isCorrectRoom && isCorrectShift;

        if (paperObject.activeSelf != shouldShow)
            paperObject.SetActive(shouldShow);

        if (showDebugLogs)
        {
            GameLog.Log(
                "Paper Check | Room: " + GameManager.Instance.currentRoomIndex +
                " | Shifts: " + GameManager.Instance.currentShiftsLeft +
                " | Show: " + shouldShow
            );
        }
    }
}