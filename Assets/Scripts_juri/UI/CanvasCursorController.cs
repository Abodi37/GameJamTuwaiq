using UnityEngine;

public class CanvasCursorController : MonoBehaviour
{
    [Header("Panels that need mouse")]
    public GameObject[] uiPanels;

    [Header("Disable while UI is open")]
    public Behaviour[] disableWhileUIOpen;

    [Header("Settings")]
    public bool unlockCursorWhenPanelOpen = true;
    public bool lockCursorWhenNoPanelOpen = true;

    [Tooltip("Never re-enable player scripts while the game is paused (Time.timeScale == 0). " +
             "Puzzles, the pause menu and the cookie screen all pause the game and disable " +
             "player scripts themselves - without this the controller fought them every frame.")]
    public bool respectPausedGame = true;

    // -1 = nothing applied yet, so the first LateUpdate always writes once.
    private int lastState = -1;

    void LateUpdate()
    {
        bool uiIsOpen = IsAnyPanelOpen();

        // A panel this controller does not know about (a puzzle panel, the death
        // screen) still pauses the game. Treat "paused" as "UI is open" so player
        // scripts are not switched back on underneath it.
        if (respectPausedGame && Time.timeScale == 0f)
            uiIsOpen = true;

        int state = uiIsOpen ? 1 : 0;

        // Previously this wrote Cursor.lockState and looped over every Behaviour
        // on every single frame, overwriting whatever PauseMenu, UIManager,
        // CookieConsent or the puzzle scripts had just set. Only act on a change.
        if (state == lastState)
            return;

        lastState = state;

        if (uiIsOpen)
        {
            if (unlockCursorWhenPanelOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            SetBehaviours(false);
        }
        else
        {
            if (lockCursorWhenNoPanelOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SetBehaviours(true);
        }
    }

    bool IsAnyPanelOpen()
    {
        if (uiPanels == null)
            return false;

        for (int i = 0; i < uiPanels.Length; i++)
        {
            if (uiPanels[i] != null && uiPanels[i].activeInHierarchy)
                return true;
        }

        return false;
    }

    void SetBehaviours(bool value)
    {
        if (disableWhileUIOpen == null)
            return;

        for (int i = 0; i < disableWhileUIOpen.Length; i++)
        {
            if (disableWhileUIOpen[i] != null)
                disableWhileUIOpen[i].enabled = value;
        }
    }
}
