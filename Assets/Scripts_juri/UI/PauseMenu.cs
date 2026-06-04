using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    void Start()
    {
        SetPaused(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void SetPaused(bool value)
    {
        isPaused = value;

        Time.timeScale = isPaused ? 0f : 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.SetPausePanel(isPaused);
        else
            gameObject.SetActive(isPaused);
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}