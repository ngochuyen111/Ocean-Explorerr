using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnAheadDistance = 15f;
    [SerializeField] private float yMin = -3f;
    [SerializeField] private float yMax = 3f;
    [SerializeField] private int maxEnemies = 10;

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

        Vector3 spawnPosition = new Vector3(
            cameraTransform.position.x + spawnAheadDistance,
            Random.Range(yMin, yMax),
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