using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Story UI")]
    public GameObject storyPanel;
    public TextMeshProUGUI storyText;

    [Header("Settings")]
    public float displayDuration = 3f;

    private bool isShowing;

    private void Awake()
    {
        // Mỗi scene dùng StoryManager của chính scene đó
        Instance = this;

        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }
    }

    public void ShowTreasureStory(
        string storyMessage,
        string nextSceneName)
    {
        if (isShowing)
        {
            return;
        }

        if (storyPanel == null || storyText == null)
        {
            Debug.LogError(
                "StoryManager: Chưa gán StoryPanel hoặc StoryText."
            );

            return;
        }

        StartCoroutine(
            ShowThenLoad(
                storyMessage,
                nextSceneName
            )
        );
    }

    private IEnumerator ShowThenLoad(
        string storyMessage,
        string nextSceneName)
    {
        isShowing = true;

        Debug.Log(
            "SHOW STORY | Scene: " +
            SceneManager.GetActiveScene().name
        );

        storyText.text = storyMessage;
        storyPanel.SetActive(true);

        // Dừng chuyển động game trong lúc hiện nội dung
        Time.timeScale = 0f;

        // Vẫn đếm thời gian dù Time.timeScale = 0
        yield return new WaitForSecondsRealtime(
            displayDuration
        );

        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                "StoryManager: Next Scene Name đang trống."
            );

            isShowing = false;
            yield break;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}