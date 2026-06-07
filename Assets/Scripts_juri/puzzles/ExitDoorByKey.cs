using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorByKey : MonoBehaviour
{
    [Header("Requirements")]
    public string keyFlag = "exitKeyCollected";
    public string puzzle1Flag = "puzzle1Solved";
    public string puzzle2Flag = "puzzle2Solved";

    [Header("Door")]
    public Transform doorTransform;
    public Vector3 openLocalRotation = new Vector3(0f, 90f, 0f);
    public float openDuration = 1f;

    [Header("End Game Panel")]
    public GameObject endGamePanel;
    public GameObject playerUI;
    public float showEndPanelDelay = 1.2f;

    [Header("Player")]
    public Behaviour[] disableOnEnd;

    [Header("Messages")]
    public string missingKeyMessage = "The door is locked. You need the key.";
    public string missingPuzzleMessage = "Something is still unfinished. Solve both puzzles first.";
    public string openingMessage = "The door opens. You made it out.";

    private bool opened;
    private bool opening;

    void Start()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
    }

    public void TryOpenDoor()
    {
        if (opened || opening)
            return;

        if (GameManager.Instance == null)
        {
            ShowMessage("GameManager is missing.");
            return;
        }

        bool hasKey = GameManager.Instance.HasFlag(keyFlag);
        bool puzzle1Solved = GameManager.Instance.HasFlag(puzzle1Flag);
        bool puzzle2Solved = GameManager.Instance.HasFlag(puzzle2Flag);

        if (!hasKey)
        {
            ShowMessage(missingKeyMessage);
            return;
        }

        if (!puzzle1Solved || !puzzle2Solved)
        {
            ShowMessage(missingPuzzleMessage);
            return;
        }

        StartCoroutine(OpenDoorAndEndGame());
    }

    IEnumerator OpenDoorAndEndGame()
    {
        opening = true;
        opened = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowInteract("");
            UIManager.Instance.HideDialogue();
            UIManager.Instance.SetObjective(openingMessage);
        }

        if (doorTransform != null)
        {
            Quaternion startRotation = doorTransform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(openLocalRotation);

            float timer = 0f;

            while (timer < openDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(timer / openDuration);
                t = t * t * (3f - 2f * t);

                doorTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);

                yield return null;
            }

            doorTransform.localRotation = targetRotation;
        }

        yield return new WaitForSeconds(showEndPanelDelay);

        ShowEndPanel();
    }

    void ShowEndPanel()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseAllPanels();

            if (UIManager.Instance.dialoguePanel != null)
                UIManager.Instance.dialoguePanel.SetActive(false);
        }

        if (playerUI != null)
            playerUI.SetActive(false);

        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        for (int i = 0; i < disableOnEnd.Length; i++)
        {
            if (disableOnEnd[i] != null)
                disableOnEnd[i].enabled = false;
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ShowMessage(string message)
    {
        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.StartDialogue(new string[]
            {
                message
            });
        }
        else if (UIManager.Instance != null)
        {
            UIManager.Instance.SetObjective(message);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void GoToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}