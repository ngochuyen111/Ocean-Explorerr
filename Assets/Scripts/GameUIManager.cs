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

    // Chơi game mới từ Intro.
    public void Play()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.StartNewGame();
        }

        SceneManager.LoadScene("Intro");
    }

    // Restart đúng checkpoint gần nhất.
    public void Restart()
    {
        Time.timeScale = 1f;

        // Scene mà người chơi đang chơi
        string currentLevel =
            SceneManager.GetActiveScene().name;

        if (ScoreManager.instance != null)
        {
            // Restart Level 1 thì bắt đầu lại từ đầu
            if (currentLevel == "Level1")
            {
                ScoreManager.instance.StartNewGame();
            }
            else
            {
                /*
                 * Level 2, 3, 4:
                 * Chỉ khôi phục checkpoint khi checkpoint
                 * đúng với màn hiện tại.
                 */
                if (ScoreManager.instance.HasSavedGame() &&
                    ScoreManager.instance.GetSavedLevel() == currentLevel)
                {
                    ScoreManager.instance.LoadCheckpoint();
                }
                else
                {
                    Debug.LogWarning(
                        "Checkpoint không khớp với scene hiện tại." +
                        "\nCurrent scene: " + currentLevel +
                        "\nSaved scene: " +
                        ScoreManager.instance.GetSavedLevel()
                    );
                }
            }
        }

        Debug.Log(
            "Restart current level: " + currentLevel
        );

        // Luôn mở lại đúng màn hiện tại
        SceneManager.LoadScene(currentLevel);
    }

    // Xóa tiến trình và bắt đầu Level1.
    public void ResetGame()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.StartNewGame();
        }

        SceneManager.LoadScene("Level1");
    }

    // Tiếp tục checkpoint gần nhất.
    public void ContinueGame()
    {
        Time.timeScale = 1f;

        if (ScoreManager.instance == null)
        {
            Debug.LogError(
                "Không tìm thấy ScoreManager."
            );

            return;
        }

        if (!ScoreManager.instance.HasSavedGame())
        {
            Debug.LogWarning(
                "Chưa có checkpoint để Continue."
            );

            return;
        }

        ScoreManager.instance.LoadCheckpoint();

        string savedLevel =
            ScoreManager.instance.GetSavedLevel();

        if (!Application.CanStreamedLevelBeLoaded(
                savedLevel
            ))
        {
            Debug.LogError(
                "Không tìm thấy Scene: " +
                savedLevel
            );

            return;
        }

        SceneManager.LoadScene(savedLevel);
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("Credit");
    }

    public void Exit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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
}