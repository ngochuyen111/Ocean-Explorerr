using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GameBounds2D : MonoBehaviour
{
    [Header("Khoảng cách mặc định với mép map")]
    [SerializeField] private float defaultPadding = 0.5f;

    private BoxCollider2D boundsCollider;

    public Bounds WorldBounds
    {
        get
        {
            if (boundsCollider == null)
            {
                boundsCollider = GetComponent<BoxCollider2D>();
            }

            return boundsCollider.bounds;
        }
    }

    private void Awake()
    {
        boundsCollider = GetComponent<BoxCollider2D>();
    }

    // Lấy vị trí ngẫu nhiên trong toàn bộ GameBounds
    public Vector2 GetRandomPoint(float padding = -1f)
    {
        padding = GetPadding(padding);

        Bounds bounds = WorldBounds;

        float minX = bounds.min.x + padding;
        float maxX = bounds.max.x - padding;
        float minY = bounds.min.y + padding;
        float maxY = bounds.max.y - padding;

        if (minX > maxX)
        {
            minX = maxX = bounds.center.x;
        }

        if (minY > maxY)
        {
            minY = maxY = bounds.center.y;
        }

        return new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }

    // Lấy vị trí phía trước camera nhưng vẫn trong GameBounds
    public bool TryGetRandomPointAhead(
        Transform cameraTransform,
        float aheadMin,
        float aheadMax,
        float requestedYMin,
        float requestedYMax,
        out Vector2 point,
        float padding = -1f)
    {
        point = Vector2.zero;

        if (cameraTransform == null)
        {
            return false;
        }

        padding = GetPadding(padding);

        Bounds bounds = WorldBounds;

        float minX = Mathf.Max(
            cameraTransform.position.x + aheadMin,
            bounds.min.x + padding
        );

        float maxX = Mathf.Min(
            cameraTransform.position.x + aheadMax,
            bounds.max.x - padding
        );

        float minY = Mathf.Max(
            requestedYMin,
            bounds.min.y + padding
        );

        float maxY = Mathf.Min(
            requestedYMax,
            bounds.max.y - padding
        );

        // Không còn vùng hợp lệ phía trước camera
        if (minX > maxX || minY > maxY)
        {
            return false;
        }

        point = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        return true;
    }

    // Kiểm tra một điểm có nằm trong GameBounds hay không
    public bool ContainsPoint(
        Vector2 point,
        float padding = 0f)
    {
        Bounds bounds = WorldBounds;

        return
            point.x >= bounds.min.x + padding &&
            point.x <= bounds.max.x - padding &&
            point.y >= bounds.min.y + padding &&
            point.y <= bounds.max.y - padding;
    }

    // Ép vị trí quay trở lại trong GameBounds
    public Vector2 ClampPoint(
        Vector2 point,
        float padding = -1f)
    {
        padding = GetPadding(padding);

        Bounds bounds = WorldBounds;

        float minX = bounds.min.x + padding;
        float maxX = bounds.max.x - padding;
        float minY = bounds.min.y + padding;
        float maxY = bounds.max.y - padding;

        point.x = minX <= maxX
            ? Mathf.Clamp(point.x, minX, maxX)
            : bounds.center.x;

        point.y = minY <= maxY
            ? Mathf.Clamp(point.y, minY, maxY)
            : bounds.center.y;

        return point;
    }

    private float GetPadding(float padding)
    {
        return padding < 0f
            ? defaultPadding
            : padding;
    }
}