using UnityEngine;

public class ShowPanelByShiftAndRoom : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelToShow;

    [Header("Condition")]
    public int requiredRoomIndex = 2;
    public int requiredShiftsLeft = 4;

    [Header("Options")]
    public bool showOnlyOnce = true;
    public bool pauseGameWhenShown = true;
    public bool unlockCursorWhenShown = true;

    private bool hasShown = false;

    void Start()
    {
        if (panelToShow != null)
            panelToShow.SetActive(false);
    }

    void Update()
    {
        if (hasShown && showOnlyOnce) return;
        if (GameManager.Instance == null) return;

        bool correctRoom = GameManager.Instance.currentRoomIndex == requiredRoomIndex;
        bool correctShifts = GameManager.Instance.currentShiftsLeft == requiredShiftsLeft;

        if (correctRoom && correctShifts)
        {
            ShowPanel();
        }
    }

    void ShowPanel()
    {
        hasShown = true;

        if (panelToShow != null)
            panelToShow.SetActive(true);

        if (pauseGameWhenShown)
            Time.timeScale = 0f;

        if (unlockCursorWhenShown)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ClosePanel()
    {
        if (panelToShow != null)
            panelToShow.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}