using UnityEngine;
using UnityEngine.InputSystem;

public class CookieConsent : MonoBehaviour
{
    [Header("UI")]
    public GameObject consentPanel;

    [Header("Player Optional")]
    public PlayerMovements playerMovement;
    public PlayerInput playerInput;

    [Header("Cookie Item Optional")]
    public GameObject cookiePrefab;
    public Transform cookieHoldPoint;

    void Start()
    {
        OpenCookieScreen();
    }

    public void OpenCookieScreen()
    {
        Time.timeScale = 0f;

        if (consentPanel != null)
            consentPanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AcceptCookies()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetFlag("hasCookies", true);
            GameManager.Instance.AddItem("Cookies");
            GameManager.Instance.gameStarted = true;
        }

        if (cookiePrefab != null && cookieHoldPoint != null)
        {
            GameObject cookie = Instantiate(cookiePrefab, cookieHoldPoint.position, cookieHoldPoint.rotation);
            cookie.transform.SetParent(cookieHoldPoint);
            cookie.transform.localPosition = Vector3.zero;
            cookie.transform.localRotation = Quaternion.identity;
        }

        if (consentPanel != null)
            consentPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (UIManager.Instance != null)
            UIManager.Instance.SetObjective("Find the three broken number clues. The missing spaces are not the answer.");
    }

    public void RejectCookies()
    {
        Debug.Log("No is not allowed.");
    }
}