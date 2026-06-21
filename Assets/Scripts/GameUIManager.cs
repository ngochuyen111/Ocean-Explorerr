using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject guidePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Intro");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        string levelName = PlayerPrefs.GetString("LastLevel", "Level1");
        SceneManager.LoadScene(levelName);
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Setting()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Setting");
    }

    public void Credit()
    {
        SceneManager.LoadScene("Credit");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        if (pausePanel == null) return;

        bool active = !pausePanel.activeSelf;
        pausePanel.SetActive(active);
        Time.timeScale = active ? 0f : 1f;
    }

    public void OpenGuide()
    {
        if (guidePanel != null)
            guidePanel.SetActive(true);
    }

    public void CloseGuide()
    {
        if (guidePanel != null)
            guidePanel.SetActive(false);
    }
}