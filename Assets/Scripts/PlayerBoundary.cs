using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBoundary : MonoBehaviour
{
    [Header("Boundary")]
    [SerializeField] private BoxCollider2D levelBounds;

    [Header("Settings")]
    [SerializeField] private float padding = 0.05f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        if (playerCollider == null)
        {
            Debug.LogError(
                "PlayerBoundary: Player cần có Collider2D."
            );
        }
    }

    private void FixedUpdate()
    {
        if (levelBounds == null || playerCollider == null)
        {
            return;
        }

        Bounds mapBounds = levelBounds.bounds;
        Bounds shipBounds = playerCollider.bounds;

        Vector2 position = rb.position;

        // Độ lệch giữa tâm collider và tâm Player
        Vector2 colliderOffset =
            (Vector2)shipBounds.center - rb.position;

        float minX =
            mapBounds.min.x +
            shipBounds.extents.x +
            padding -
            colliderOffset.x;

        float maxX =
            mapBounds.max.x -
            shipBounds.extents.x -
            padding -
            colliderOffset.x;

        float minY =
            mapBounds.min.y +
            shipBounds.extents.y +
            padding -
            colliderOffset.y;

        float maxY =
            mapBounds.max.y -
            shipBounds.extents.y -
            padding -
            colliderOffset.y;

        position.x = minX <= maxX
            ? Mathf.Clamp(position.x, minX, maxX)
            : mapBounds.center.x;

        position.y = minY <= maxY
            ? Mathf.Clamp(position.y, minY, maxY)
            : mapBounds.center.y;

        rb.position = position;
    }
}