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
    [Tooltip("Khoảng cách nhỏ nhất phía trước camera")]
    public float spawnAheadMin = 8f;

    [Tooltip("Khoảng cách lớn nhất phía trước camera")]
    public float spawnAheadMax = 18f;

    [Tooltip("Giới hạn Y thấp nhất mong muốn")]
    public float yMin = -3f;

    [Tooltip("Giới hạn Y cao nhất mong muốn")]
    public float yMax = 3f;

    [Header("Game Bounds")]
    public GameBounds2D gameBounds;

    [Tooltip("Khoảng cách tối thiểu giữa treasure và mép GameBounds")]
    public float boundsPadding = 0.8f;

    [Header("Trail")]
    public TreasureTrailArrow treasureTrail;

    private GameObject spawnedTreasure;
    private bool treasureSpawned;

    private void Start()
    {
        FindReferences();

        if (treasureTrail != null)
        {
            treasureTrail.gameObject.SetActive(false);
        }

        ScoreManager[] managers =
            FindObjectsByType<ScoreManager>(
                FindObjectsSortMode.None
            );

        Debug.Log(
            "Số lượng ScoreManager: " +
            managers.Length
        );

        if (ScoreManager.instance != null)
        {
            Debug.Log(
                "Scene: " + gameObject.scene.name +
                " | Pearl: " +
                ScoreManager.instance.pearls +
                " | Required: " +
                pearlsRequired +
                " | ScoreManager ID: " +
                ScoreManager.instance.GetInstanceID()
            );
        }
    }

    private void FindReferences()
    {
        if (cameraTransform == null &&
            Camera.main != null)
        {
            cameraTransform =
                Camera.main.transform;
        }

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag(
                    "Player"
                );

            if (playerObject != null)
            {
                player =
                    playerObject.transform;
            }
        }

        if (gameBounds == null)
        {
            gameBounds =
                FindFirstObjectByType<GameBounds2D>();
        }
    }

    private void Update()
    {
        if (treasureSpawned)
        {
            return;
        }

        if (ScoreManager.instance == null)
        {
            return;
        }

        int currentPearls =
            ScoreManager.instance.pearls;

        if (currentPearls >= pearlsRequired)
        {
            SpawnTreasure();
        }
    }

    private void SpawnTreasure()
    {
        if (treasureSpawned)
        {
            return;
        }

        if (treasurePrefab == null)
        {
            Debug.LogError(
                "TreasureManager: Chưa gán Treasure Prefab."
            );

            return;
        }

        if (cameraTransform == null)
        {
            Debug.LogError(
                "TreasureManager: Không tìm thấy Main Camera."
            );

            return;
        }

        if (gameBounds == null)
        {
            Debug.LogError(
                "TreasureManager: Chưa gán GameBounds."
            );

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

        Vector2 spawnPosition;

        // Ưu tiên sinh phía trước camera,
        // nhưng phải nằm trong GameBounds.
        bool foundPosition =
            gameBounds.TryGetRandomPointAhead(
                cameraTransform,
                spawnAheadMin,
                spawnAheadMax,
                yMin,
                yMax,
                out spawnPosition,
                boundsPadding
            );

        // Camera đã gần mép cuối map và không còn
        // đủ không gian phía trước.
        if (!foundPosition)
        {
            spawnPosition =
                gameBounds.GetRandomPoint(
                    boundsPadding
                );

            Debug.LogWarning(
                "Không còn vùng hợp lệ phía trước camera. " +
                "Treasure được sinh tại vị trí ngẫu nhiên " +
                "bên trong GameBounds."
            );
        }

        spawnedTreasure = Instantiate(
            treasurePrefab,
            new Vector3(
                spawnPosition.x,
                spawnPosition.y,
                0f
            ),
            Quaternion.identity,
            transform
        );

        // Chỉ đánh dấu đã spawn sau khi Instantiate thành công.
        treasureSpawned =
            spawnedTreasure != null;

        if (!treasureSpawned)
        {
            Debug.LogError(
                "TreasureManager: Không thể tạo Treasure."
            );

            return;
        }

        Debug.Log(
            "Treasure đã sinh tại: " +
            spawnPosition
        );

        SetupTreasureTrail();
    }

    private void SetupTreasureTrail()
    {
        if (treasureTrail == null ||
            spawnedTreasure == null)
        {
            return;
        }

        treasureTrail.player = player;

        treasureTrail.SetTarget(
            spawnedTreasure.transform
        );

        treasureTrail.gameObject.SetActive(
            true
        );
    }
}