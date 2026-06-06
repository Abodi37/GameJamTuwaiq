using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    void Start()
    {
        SetPaused(false);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (Time.timeScale == 0f && !isPaused)
            return;

        TogglePause();
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