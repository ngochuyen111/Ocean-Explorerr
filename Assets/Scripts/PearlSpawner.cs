using UnityEngine;

public class PearlSpawner : MonoBehaviour
{
    [Header("Các prefab ngọc trai (kéo nhiều loại có value khác nhau vào đây)")]
    public GameObject[] pearlPrefabs;

    [Header("Khu vực random vị trí spawn")]
    public Vector2 areaMin;
    public Vector2 areaMax;

    [Header("Khoảng cách tối thiểu giữa 2 ngọc trai (tránh ra gần nhau)")]
    public float minDistanceBetweenPearls = 2.5f;

    [Header("Thời gian giữa các lần spawn (giây)")]
    public float spawnInterval = 5f;

    [Header("Số lượng ngọc trai tối đa tồn tại cùng lúc trong scene")]
    public int maxPearls = 10;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawnPearl();
        }
    }

    void TrySpawnPearl()
    {
        if (pearlPrefabs.Length == 0) return;

        GameObject[] existingPearls = GameObject.FindGameObjectsWithTag("Pearl");
        if (existingPearls.Length >= maxPearls) return;

        Vector3? spawnPos = FindValidPosition(existingPearls);
        if (spawnPos == null) return; // thử nhiều lần không tìm được chỗ trống đủ xa, bỏ qua lượt này

        GameObject prefabToSpawn = pearlPrefabs[Random.Range(0, pearlPrefabs.Length)];
        Instantiate(prefabToSpawn, spawnPos.Value, Quaternion.identity);
    }

    Vector3? FindValidPosition(GameObject[] existingPearls)
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(areaMin.x, areaMax.x);
            float y = Random.Range(areaMin.y, areaMax.y);
            Vector3 candidate = new Vector3(x, y, 0f);

            bool tooClose = false;
            foreach (GameObject pearl in existingPearls)
            {
                if (Vector3.Distance(candidate, pearl.transform.position) < minDistanceBetweenPearls)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose) return candidate;
        }

        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((areaMin.x + areaMax.x) / 2f, (areaMin.y + areaMax.y) / 2f, 0f);
        Vector3 size = new Vector3(Mathf.Abs(areaMax.x - areaMin.x), Mathf.Abs(areaMax.y - areaMin.y), 0f);
        Gizmos.DrawWireCube(center, size);
    }
}