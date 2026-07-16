using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    [Header("Treasure")]
    public GameObject treasurePrefab;
    public Transform player;
    public Transform cameraTransform;

    [Header("Spawn Condition")]
    public int pearlsRequired = 10;

    [Header("Random Spawn Area")]
    public float spawnAheadMin = 8f;
    public float spawnAheadMax = 18f;

    [Header("Bottom Spawn")]
    public float bottomMinY = -4.8f;
    public float bottomMaxY = -3.8f;

    [Header("Trail")]
    public TreasureTrailArrow treasureTrail;

    private GameObject spawnedTreasure;
    private bool treasureSpawned = false;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (treasureTrail != null)
            treasureTrail.gameObject.SetActive(false);
    }

    void Update()
    {
        if (treasureSpawned) return;
        if (ScoreManager.instance == null) return;

        if (ScoreManager.instance.pearls >= pearlsRequired)
        {
            SpawnTreasure();
        }
    }

    void SpawnTreasure()
    {
        treasureSpawned = true;

        float x = cameraTransform.position.x + Random.Range(spawnAheadMin, spawnAheadMax);
        float y = Random.Range(bottomMinY, bottomMaxY);

        Vector3 spawnPos = new Vector3(x, y, 0f);

        spawnedTreasure = Instantiate(treasurePrefab, spawnPos, Quaternion.identity);

        if (treasureTrail != null)
        {
            treasureTrail.player = player;
            treasureTrail.SetTarget(spawnedTreasure.transform);
        }
    }
}