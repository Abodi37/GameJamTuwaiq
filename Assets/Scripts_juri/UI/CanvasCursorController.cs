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

    void LateUpdate()
    {
        bool uiIsOpen = IsAnyPanelOpen();

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