using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    [Header("Treasure")]
    public GameObject treasurePrefab;
    public Transform player;
    public Transform cameraTransform;

    [Header("Spawn Condition")]
    public int pearlsRequired = 30;

    [Header("Random Spawn Area")]
    public float spawnAheadMin = 8f;
    public float spawnAheadMax = 18f;
    public float yMin = -3f;
    public float yMax = 3f;

    [Header("Trail")]
    public TreasureTrailArrow treasureTrail;

    private GameObject spawnedTreasure;
    private bool treasureSpawned;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (treasureTrail != null)
        {
            treasureTrail.gameObject.SetActive(false);
        }

        ScoreManager[] managers =
            FindObjectsByType<ScoreManager>(FindObjectsSortMode.None);

        Debug.Log("Số lượng ScoreManager: " + managers.Length);

        if (ScoreManager.instance != null)
        {
            Debug.Log(
                "Scene: " + gameObject.scene.name +
                " | Pearl: " + ScoreManager.instance.pearls +
                " | Required: " + pearlsRequired +
                " | ScoreManager ID: " +
                ScoreManager.instance.GetInstanceID()
            );
        }
    }

    private void Update()
    {
        if (treasureSpawned)
            return;

        if (ScoreManager.instance == null)
            return;

        int currentPearls = ScoreManager.instance.pearls;

        if (currentPearls >= pearlsRequired)
        {
            SpawnTreasure();
        }
    }

    private void SpawnTreasure()
    {
        if (treasurePrefab == null)
        {
            Debug.LogError("Chưa gán Treasure Prefab.");
            return;
        }

        if (cameraTransform == null)
        {
            Debug.LogError("Không tìm thấy Main Camera.");
            return;
        }

        Debug.LogWarning(
            "SPAWN TREASURE | Scene: " +
            gameObject.scene.name +
            " | Pearl: " +
            ScoreManager.instance.pearls +
            " | Required: " +
            pearlsRequired
        );

        treasureSpawned = true;

        float x = cameraTransform.position.x +
                  Random.Range(spawnAheadMin, spawnAheadMax);

        float y = Random.Range(yMin, yMax);

        Vector3 spawnPosition = new Vector3(x, y, 0f);

        spawnedTreasure = Instantiate(
            treasurePrefab,
            spawnPosition,
            Quaternion.identity
        );

        if (treasureTrail != null)
        {
            treasureTrail.player = player;
            treasureTrail.SetTarget(spawnedTreasure.transform);
        }
    }
}