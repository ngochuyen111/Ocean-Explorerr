using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    public GameObject storyPanel;
    public TextMeshProUGUI storyText;

    void Awake()
    {
        Instance = this;

        if (storyPanel != null)
            storyPanel.SetActive(false);
    }

    public void ShowTreasureStory(string nextSceneName)
    {
        StartCoroutine(ShowThenLoad(nextSceneName));
    }

    IEnumerator ShowThenLoad(string nextSceneName)
    {
        Debug.Log("SHOW STORY START");

        storyPanel.SetActive(true);
        storyText.text = "Bạn đã tìm thấy kho báu ẩn giấu!\nMột con đường mới dẫn đến đại dương sâu thẳm đã mở ra...";

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(nextSceneName);
    }
}