using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Bubble Settings")]
    public GameObject bubblePrefab;
    public float spawnRate = 0.3f;        // Tần suất spawn
    public float spawnWidth = 10f;        // Độ rộng vùng spawn
    public float bubbleSpeed = 2f;        // Tốc độ nổi lên
    public float bubbleLifetime = 4f;     // Thời gian sống
    public Vector2 sizeRange = new Vector2(0.1f, 0.5f); // Kích thước random

    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = spawnRate;
            SpawnBubble();
        }
    }

    void SpawnBubble()
    {
        if (bubblePrefab == null) return;

        // Vị trí random theo chiều ngang
        float randomX = transform.position.x + Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y, 0f);

        GameObject bubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);

        // Kích thước random
        float size = Random.Range(sizeRange.x, sizeRange.y);
        bubble.transform.localScale = Vector3.one * size;

        // Tốc độ nổi lên random một chút
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float randomSpeed = bubbleSpeed * Random.Range(0.7f, 1.3f);
            rb.linearVelocity = new Vector2(Random.Range(-0.3f, 0.3f), randomSpeed);
        }

        Destroy(bubble, bubbleLifetime);
    }
}