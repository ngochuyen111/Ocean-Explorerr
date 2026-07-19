using UnityEngine;
using System.Collections;

public class OxygenPickupSpawner : MonoBehaviour

{
    [Header("Prefab")]
    public GameObject oxygenPickupPrefab;

    [Header("Player")]
    public Transform player;

    [Header("Spawn Time")]
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 5f;

    [Header("Spawn Distance")]
    public float minDistanceFromPlayer = 10f;
    public float maxDistanceFromPlayer = 15f;

    [Header("Map Boundary")]
    public float minX = -18f;
    public float maxX = 18f;
    public float minY = -10f;
    public float maxY = 6f;

    [Header("First Spawn")]
    public float firstSpawnMinTime = 15f;
    public float firstSpawnMaxTime = 20f;

    [Header("Obstacle")]
    public LayerMask obstacleLayer;

    private GameObject currentPickup;
    public void PickupCollected()
    {
        Debug.Log("Oxygen collected -> Ready to spawn new one");
        currentPickup = null;
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SpawnLoop());
    }
    IEnumerator FirstSpawn()
    {
        float wait = Random.Range(firstSpawnMinTime, firstSpawnMaxTime);

        yield return new WaitForSeconds(wait);

        SpawnPickup();

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        // Chờ lần đầu
        float firstWait = Random.Range(firstSpawnMinTime, firstSpawnMaxTime);

        Debug.Log("First oxygen spawn after: " + firstWait);

        yield return new WaitForSeconds(firstWait);


        while (true)
        {
            if (currentPickup == null)
            {
                SpawnPickup();
            }

            // đợi người chơi nhặt
            yield return new WaitUntil(() => currentPickup == null);


            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);

            Debug.Log("Next oxygen after: " + wait);

            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnPickup()
    {
        if (oxygenPickupPrefab == null || player == null)
            return;

        for (int i = 0; i < 50; i++)
        {
            // Ưu tiên spawn phía trước Player
            float angle = Random.Range(-60f, 60f);

            // Nếu Player quay trái thì spawn bên trái
            float direction = player.localScale.x >= 0 ? 1 : -1;

            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right * direction;

            float distance = Random.Range(5f, 8f);

            Vector2 spawnPos = (Vector2)player.position + dir.normalized * distance;

            // Không cho ra ngoài map
            spawnPos.x = Mathf.Clamp(spawnPos.x, minX, maxX);
            spawnPos.y = Mathf.Clamp(spawnPos.y, minY, maxY);

            // Kiểm tra có vật cản không
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.6f, obstacleLayer);

            if (hit != null)
                continue;

            currentPickup = Instantiate(
                oxygenPickupPrefab,
                spawnPos,
                Quaternion.identity
            );

            currentPickup.GetComponent<OxygenPickup>().spawner = this;

            Debug.Log("Spawn Oxygen: " + spawnPos);

            return;
        }

        Debug.Log("Không tìm được vị trí spawn phù hợp.");
    }

    void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, maxDistanceFromPlayer);
    }
}