using System.Collections.Generic;
using UnityEngine;

public class CoralSpawner : MonoBehaviour
{
    [Header("Coral Prefab")]
    public GameObject coralHazardPrefab;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Spawn Settings")]
    public float spawnAheadDistance = 20f;
    public float despawnBehindDistance = 25f;
    public float chunkWidth = 15f;
    public int minPerChunk = 3;
    public int maxPerChunk = 5;

    [Header("Y Position - CHINH CAI NAY DE REN RAC")]
    [Tooltip("Y thap nhat co the spawn (vi du: -4 = gan nen)")]
    public float yMin = -4f;
    [Tooltip("Y cao nhat co the spawn (vi du: 3 = gan mat nuoc)")]
    public float yMax = 3f;

    [Header("Scale Range")]
    public float minScale = 0.2f;
    public float maxScale = 0.5f;

    [Header("Spacing")]
    [Tooltip("Tang so nay neu con dinh nhau. Thu 3 -> 5 -> 7")]
    public float extraPaddingBetween = 4f;

    [Header("Damage")]
    public float minDamage = 20f;
    public float maxDamage = 50f;

    [Header("Prefab Info")]
    public float prefabColliderWidth = 1f;

    private float lastSpawnedChunkX;
    private List<GameObject> allCorals = new List<GameObject>();

    private struct PlacedCoral
    {
        public float centerX;
        public float halfWidth;
    }

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

        for (int i = allCorals.Count - 1; i >= 0; i--)
        {
            if (allCorals[i] == null) { allCorals.RemoveAt(i); continue; }
            if (allCorals[i].transform.position.x < camX - despawnBehindDistance)
            {
                Destroy(allCorals[i]);
                allCorals.RemoveAt(i);
            }
        }
    }

    void SpawnChunk(float chunkCenterX)
    {
        int count = Random.Range(minPerChunk, maxPerChunk + 1);
        List<PlacedCoral> placed = new List<PlacedCoral>();
        float halfChunk = chunkWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            float scale = Random.Range(minScale, maxScale);
            float halfW = (prefabColliderWidth * scale) / 2f;

            float spawnX = GetValidX(placed, halfW, chunkCenterX, halfChunk);
            if (float.IsNaN(spawnX)) break;

            // Y hoan toan ngau nhien trong khoang yMin -> yMax
            float spawnY = Random.Range(yMin, yMax);

            GameObject coral = Instantiate(coralHazardPrefab,
                new Vector3(spawnX, spawnY, 0f),
                Quaternion.identity, transform);
            coral.transform.localScale = new Vector3(scale, scale, 1f);

            float t = Mathf.InverseLerp(minScale, maxScale, scale);
            SeaHazard hazard = coral.GetComponent<SeaHazard>();
            if (hazard != null)
                hazard.damage = Mathf.Lerp(minDamage, maxDamage, t);

            placed.Add(new PlacedCoral { centerX = spawnX, halfWidth = halfW });
            allCorals.Add(coral);
        }
    }

    float GetValidX(List<PlacedCoral> placed, float myHalfW, float centerX, float halfRange)
    {
        int maxAttempts = 60;
        for (int i = 0; i < maxAttempts; i++)
        {
            float candidate = centerX + Random.Range(-halfRange + myHalfW, halfRange - myHalfW);
            bool overlap = false;
            foreach (var p in placed)
            {
                if (Mathf.Abs(candidate - p.centerX) < myHalfW + p.halfWidth + extraPaddingBetween)
                { overlap = true; break; }
            }
            if (!overlap) return candidate;
        }
        return float.NaN;
    }

    void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        float camX = cameraTransform.position.x;
        float midY = (yMin + yMax) / 2f;
        float heightRange = yMax - yMin;

        // Vung Y spawn - mau xanh la
        Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.12f);
        Gizmos.DrawCube(new Vector3(camX, midY, 0f),
                        new Vector3(spawnAheadDistance * 2f, heightRange, 1f));
        Gizmos.color = new Color(0.2f, 0.9f, 0.3f, 0.7f);
        Gizmos.DrawWireCube(new Vector3(camX, midY, 0f),
                            new Vector3(spawnAheadDistance * 2f, heightRange, 1f));

        // Duong yMin va yMax
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.9f);
        Gizmos.DrawLine(new Vector3(camX - 15f, yMin, 0f), new Vector3(camX + 15f, yMin, 0f));
        Gizmos.DrawLine(new Vector3(camX - 15f, yMax, 0f), new Vector3(camX + 15f, yMax, 0f));
    }
}