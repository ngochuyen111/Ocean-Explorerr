using TMPro;
using UnityEngine;

public class TreasurePickup : MonoBehaviour
{
    [Header("Story")]
    public GameObject storyPanel;
    public TextMeshProUGUI storyText;

    [TextArea(2, 4)]
    public string storyLine =
        "You found the hidden treasure! " +
        "The submarine continues deeper into the ocean...";

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "Level2";

    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (StoryManager.Instance == null)
        {
            Debug.LogError(
                "TreasurePickup: StoryManager.Instance is null."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                "TreasurePickup: Next Scene Name is empty."
            );

            return;
        }

        collected = true;

        // Ngăn Player kích hoạt Treasure nhiều lần.
        Collider2D treasureCollider = GetComponent<Collider2D>();

        if (treasureCollider != null)
        {
            treasureCollider.enabled = false;
        }

        // Lưu điểm cuối màn và màn tiếp theo.
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveCheckpoint(nextSceneName);
        }
        else
        {
            Debug.LogWarning(
                "TreasurePickup: ScoreManager was not found."
            );
        }

        Debug.Log(
            "Treasure collected. Checkpoint saved for: " +
            nextSceneName
        );

        // StoryManager sẽ hiển thị story rồi chuyển Scene.
        StoryManager.Instance.ShowTreasureStory(nextSceneName);
    }
}