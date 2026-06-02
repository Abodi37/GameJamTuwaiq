using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused;

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

    public void QuitGame()
    {
        Application.Quit();
    }
}