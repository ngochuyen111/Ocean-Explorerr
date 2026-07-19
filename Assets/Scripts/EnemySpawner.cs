using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnAheadDistance = 15f;
    [SerializeField] private float xMin = -20f;
    [SerializeField] private float xMax = 80f;
    [SerializeField] private float yMin = -4f;
    [SerializeField] private float yMax = 4f;
    [SerializeField] private int maxEnemies = 15;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (transform.childCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null ||
            enemyPrefabs.Length == 0 ||
            cameraTransform == null)
        {
            return;
        }

        GameObject selectedPrefab =
            enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        float randomX = Random.Range(xMin, xMax);
        float randomY = Random.Range(yMin, yMax);

        Vector3 spawnPosition = new Vector3(
            randomX,
            randomY,
            0f
        );

        Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity,
            transform
        );
    }
}