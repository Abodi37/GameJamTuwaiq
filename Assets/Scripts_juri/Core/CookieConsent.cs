using UnityEngine;

public class CookieConsent : MonoBehaviour
{
    [Header("Cookie Screen")]
    public GameObject cookiePanel;
    public bool showCookieScreenOnStart = true;

    [Header("Player UI")]
    public GameObject playerUI;

    [Header("Disable Player Until Accept")]
    public Behaviour[] disableBeforeAccept;

    [Header("Companion Spawn")]
    public GameObject companionObject;
    public Transform[] companionSpawnPoints;
    public bool hideCompanionUntilAccept = true;

    [Header("Cookie Inventory")]
    public string cookieFlag = "hasCookies";
    public string cookieItemName = "Cookies";
    public bool addCookiesToInventory = true;

    [Header("After Accept")]
    public string afterAcceptObjective = "Turn around. You are not alone.";

    private bool accepted;

    void Awake()
    {
        accepted = false;
    }

    void Start()
    {
        if (showCookieScreenOnStart)
            OpenCookieScreen();
        else
            SkipCookieScreen();
    }

    void OpenCookieScreen()
    {
        accepted = false;

        if (cookiePanel != null)
            cookiePanel.SetActive(true);

        if (playerUI != null)
            playerUI.SetActive(false);

        if (hideCompanionUntilAccept && companionObject != null)
            companionObject.SetActive(false);

        SetPlayerEnabled(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AcceptCookies()
    {
        if (accepted)
            return;

        accepted = true;

        if (cookiePanel != null)
            cookiePanel.SetActive(false);

        if (playerUI != null)
            playerUI.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetFlag(cookieFlag, true);

            if (addCookiesToInventory)
                GameManager.Instance.AddItem(cookieItemName);

            GameManager.Instance.gameStarted = true;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetCookieIcon(true);
            UIManager.Instance.SetObjective(afterAcceptObjective);
            UIManager.Instance.HideDialogue();

            if (addCookiesToInventory)
                UIManager.Instance.SetInventoryText("Inventory: Cookies");
        }

        SpawnCompanion();

        Time.timeScale = 1f;

        SetPlayerEnabled(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RejectCookies()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ShowDialogueLine("No cookies? That is not how this starts.");
    }

    public void SkipCookieScreen()
    {
        accepted = true;

        if (cookiePanel != null)
            cookiePanel.SetActive(false);

        if (playerUI != null)
            playerUI.SetActive(true);

        SpawnCompanion();

        SetPlayerEnabled(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetPlayerEnabled(bool value)
    {
        if (disableBeforeAccept == null)
            return;

        for (int i = 0; i < disableBeforeAccept.Length; i++)
        {
            if (disableBeforeAccept[i] != null)
                disableBeforeAccept[i].enabled = value;
        }
    }

    void SpawnCompanion()
    {
        if (companionObject == null)
            return;

        if (companionSpawnPoints != null && companionSpawnPoints.Length > 0)
        {
            int index = Random.Range(0, companionSpawnPoints.Length);
            Transform spawnPoint = companionSpawnPoints[index];

            if (spawnPoint != null)
            {
                companionObject.transform.position = spawnPoint.position;
                companionObject.transform.rotation = spawnPoint.rotation;
            }
        }

        companionObject.SetActive(true);

        HeIsRightBehindMeIsntHe followScript =
            companionObject.GetComponent<HeIsRightBehindMeIsntHe>();

        if (followScript != null)
            followScript.enabled = false;
    }
}