using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnInterval = 3f;
    public int maxObjects = 5;
    public Vector2 randomOffset = new Vector2(3f, 2f);

    private float nextSpawnTime;

    void Update()
    {
        if (prefab == null) return;
        if (Time.time < nextSpawnTime) return;

        GameObject[] objects = GameObject.FindGameObjectsWithTag(prefab.tag);
        if (objects.Length >= maxObjects) return;

        Vector3 pos = transform.position;
        pos.x += Random.Range(-randomOffset.x, randomOffset.x);
        pos.y += Random.Range(-randomOffset.y, randomOffset.y);

        Instantiate(prefab, pos, Quaternion.identity);
        nextSpawnTime = Time.time + spawnInterval;
    }
}