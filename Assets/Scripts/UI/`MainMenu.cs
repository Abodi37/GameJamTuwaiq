using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "Game";

    [Header("Panels Optional")]
    public GameObject mainPanel;
    public GameObject pausePanel;
  

    void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainPanel();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null)
            mainPanel.SetActive(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        
    }

    public void ShowSettingsPanel()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);

  
    }

    public void ShowCreditsPanel()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

    }
}