using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject pausePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC PRESSED");
            TogglePause();
        }
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        string levelName =
            PlayerPrefs.GetString("LastLevel", "Level1");

        SceneManager.LoadScene(levelName);
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Setting()
    {
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
}