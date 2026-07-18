using System.Collections.Generic;
using UnityEngine;

public class PearlSpawner : MonoBehaviour
{
    [Header("Pearl Prefabs")]
    public GameObject[] pearlPrefabs;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Spawn Settings")]
    public float spawnAheadDistance = 20f;
    public float despawnBehindDistance = 25f;
    public float chunkWidth = 12f;
    public int minPerChunk = 2;
    public int maxPerChunk = 5;

    [Header("Y Position")]
    public float yMin = -3f;
    public float yMax = 3f;

    [Header("Scale Range (to nho ngau nhien)")]
    [Tooltip("0.5 = nho, 1.5 = to gap 3 lan")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    [Header("Spacing")]
    public float minDistanceBetweenPearls = 2.5f;

    private float lastSpawnedChunkX;
    private List<GameObject> allPearls = new List<GameObject>();

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        float startX = cameraTransform.position.x - spawnAheadDistance;
        lastSpawnedChunkX = startX;

        for (float x = startX; x < cameraTransform.position.x + spawnAheadDistance; x += chunkWidth)
        {
            SpawnChunk(x + chunkWidth / 2f);
            lastSpawnedChunkX = x + chunkWidth;
        }
    }

    void Update()
    {
        float camX = cameraTransform.position.x;

        while (lastSpawnedChunkX < camX + spawnAheadDistance)
        {
            lastSpawnedChunkX += chunkWidth;
            SpawnChunk(lastSpawnedChunkX - chunkWidth / 2f);
        }

        // Xoa pearl da di qua (chi xoa neu chua duoc nhat)
        for (int i = allPearls.Count - 1; i >= 0; i--)
        {
            if (allPearls[i] == null) { allPearls.RemoveAt(i); continue; }
            if (allPearls[i].transform.position.x < camX - despawnBehindDistance)
            {
                Destroy(allPearls[i]);
                allPearls.RemoveAt(i);
            }
        }
    }

    void SpawnChunk(float chunkCenterX)
    {
        if (pearlPrefabs.Length == 0) return;

        int count = Random.Range(minPerChunk, maxPerChunk + 1);
        List<Vector3> placed = new List<Vector3>();
        float halfChunk = chunkWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Vector3? pos = FindValidPosition(placed, chunkCenterX, halfChunk);
            if (pos == null) break;

            float scale = Random.Range(minScale, maxScale);
            GameObject prefab = pearlPrefabs[Random.Range(0, pearlPrefabs.Length)];
            GameObject pearl = Instantiate(prefab, pos.Value, Quaternion.identity, transform);
            pearl.transform.localScale = Vector3.one * scale;

            // Pearl to hoi nhieu mau hon, pearl nho hoi it hon
            PearlPickup pickup = pearl.GetComponent<PearlPickup>();
            if (pickup != null)
            {
                float t = Mathf.InverseLerp(minScale, maxScale, scale);
                pickup.healAmount = Mathf.Lerp(2f, 6f, t);
                pickup.value = scale >= (minScale + maxScale) / 2f ? 2 : 1;
            }

            placed.Add(pos.Value);
            allPearls.Add(pearl);
        }
    }

    Vector3? FindValidPosition(List<Vector3> placed, float centerX, float halfRange)
    {
        int maxAttempts = 30;
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = centerX + Random.Range(-halfRange, halfRange);
            float y = Random.Range(yMin, yMax);
            Vector3 candidate = new Vector3(x, y, 0f);

            bool tooClose = false;
            foreach (var p in placed)
            {
                if (Vector3.Distance(candidate, p) < minDistanceBetweenPearls)
                { tooClose = true; break; }
            }
            if (!tooClose) return candidate;
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        float camX = cameraTransform.position.x;
        float midY = (yMin + yMax) / 2f;
        float h = yMax - yMin;

        Gizmos.color = new Color(0f, 1f, 1f, 0.12f);
        Gizmos.DrawCube(new Vector3(camX, midY, 0f),
                        new Vector3(spawnAheadDistance * 2f, h, 1f));
        Gizmos.color = new Color(0f, 1f, 1f, 0.8f);
        Gizmos.DrawWireCube(new Vector3(camX, midY, 0f),
                            new Vector3(spawnAheadDistance * 2f, h, 1f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(camX - 15f, yMin, 0f), new Vector3(camX + 15f, yMin, 0f));
        Gizmos.DrawLine(new Vector3(camX - 15f, yMax, 0f), new Vector3(camX + 15f, yMax, 0f));
    }
}