using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasurePickup : MonoBehaviour
{
    [Header("Story")]
    public GameObject storyPanel;
    public TextMeshProUGUI storyText;

    [TextArea(2, 4)]
    public string storyLine = "You found the hidden treasure! The submarine continues deeper into the ocean...";

    [Header("Scene")]
    public string nextSceneName = "Level2";
    public float delayBeforeLoad = 3f;

    private bool collected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        Debug.Log("TOUCH TREASURE");

        if (StoryManager.Instance == null)
        {
            Debug.LogError("StoryManager.Instance NULL");
            return;
        }

        StoryManager.Instance.ShowTreasureStory(nextSceneName);
    }

    IEnumerator ShowStoryAndLoad()
    {
        StoryManager.Instance.ShowTreasureStory(nextSceneName);
        yield return null;
    }
}