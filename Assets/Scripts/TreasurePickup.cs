using UnityEngine;

public class TreasurePickup : MonoBehaviour
{
    [Header("Story")]
    [SerializeField]
    [TextArea(3, 6)]
    private string storyLine;

    [Header("Next Scene")]
    [SerializeField]
    private string nextSceneName;

    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        if (StoryManager.Instance == null)
        {
            Debug.LogError(
                "TreasurePickup: Không tìm thấy StoryManager."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(storyLine))
        {
            Debug.LogError(
                "TreasurePickup: Chưa nhập Story Line trong Inspector."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                "TreasurePickup: Chưa nhập Next Scene Name trong Inspector."
            );
            return;
        }

        collected = true;

        Collider2D treasureCollider =
            GetComponent<Collider2D>();

        if (treasureCollider != null)
        {
            treasureCollider.enabled = false;
        }

        SpriteRenderer treasureSprite =
            GetComponent<SpriteRenderer>();

        if (treasureSprite != null)
        {
            treasureSprite.enabled = false;
        }

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveCheckpoint(
                nextSceneName
            );
        }
        else
        {
            Debug.LogWarning(
                "TreasurePickup: Không tìm thấy ScoreManager."
            );
        }

        StoryManager.Instance.ShowTreasureStory(
            storyLine,
            nextSceneName
        );
    }
}