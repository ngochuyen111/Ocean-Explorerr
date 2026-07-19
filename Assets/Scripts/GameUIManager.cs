using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject guidePanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Bắt đầu game mới
    public void Play()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.StartNewGame();
        }

        SceneManager.LoadScene("Intro");
    }

    // Chơi lại từ checkpoint gần nhất
    public void Restart()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance == null)
        {
            Debug.LogWarning("ScoreManager was not found.");
            return;
        }

        if (!ScoreManager.instance.HasSavedGame())
        {
            Debug.LogWarning("No saved checkpoint was found.");
            SceneManager.LoadScene("Level1");
            return;
        }

        ScoreManager.instance.LoadCheckpoint();

        string savedLevel =
            ScoreManager.instance.GetSavedLevel();

        SceneManager.LoadScene(savedLevel);
    }

    // Về Menu nhưng không lưu điểm hiện tại
    public void Menu()
    {
        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
        {
            Destroy(MusicManager.Instance.gameObject);
        }

        SceneManager.LoadScene("Menu");
    }

    public void Setting()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Setting");
    }

    public void Credit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Credit");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        if (pausePanel == null)
        {
            return;
        }

        bool isPaused = !pausePanel.activeSelf;

        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OpenGuide()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(true);
        }
    }

    public void CloseGuide()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(false);
        }
    }

    // Tiếp tục từ lần hoàn thành level gần nhất
    public void ContinueGame()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance == null)
        {
            Debug.LogWarning("ScoreManager was not found.");
            return;
        }

        if (!ScoreManager.instance.HasSavedGame())
        {
            Debug.LogWarning("No saved checkpoint was found.");
            return;
        }

        ScoreManager.instance.LoadCheckpoint();

        string savedLevel =
            ScoreManager.instance.GetSavedLevel();

        SceneManager.LoadScene(savedLevel);
    }
}