using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextLevelName = "Level2";
    [SerializeField] private bool isFinalLevel;
    [SerializeField] private string endingSceneName = "Ending";

    private bool levelCompleted;

    public void CompleteLevel()
    {
        if (levelCompleted)
        {
            return;
        }

        levelCompleted = true;
        Time.timeScale = 1f;

        if (isFinalLevel)
        {
            CompleteFinalLevel();
            return;
        }

        if (ScoreManager.instance != null)
        {
            // Lưu điểm cuối màn và level tiếp theo.    
            ScoreManager.instance.SaveCheckpoint(nextLevelName);
        }

        SceneManager.LoadScene(nextLevelName);
    }

    private void CompleteFinalLevel()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveFinalResult();
        }

        SceneManager.LoadScene(endingSceneName);
    }
}