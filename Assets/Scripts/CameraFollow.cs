using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private BoxCollider2D levelBounds;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null || levelBounds == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;

        // Kích thước một nửa màn hình camera
        float cameraHalfHeight = mainCamera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        Bounds bounds = levelBounds.bounds;

        // Giới hạn tâm camera để camera không nhìn ra ngoài GameBounds
        float minCameraX = bounds.min.x + cameraHalfWidth;
        float maxCameraX = bounds.max.x - cameraHalfWidth;

        float minCameraY = bounds.min.y + cameraHalfHeight;
        float maxCameraY = bounds.max.y - cameraHalfHeight;

        // Nếu bản đồ nhỏ hơn màn hình thì đặt camera vào giữa
        if (minCameraX > maxCameraX)
        {
            desiredPosition.x = bounds.center.x;
        }
        else
        {
            desiredPosition.x = Mathf.Clamp(
                desiredPosition.x,
                minCameraX,
                maxCameraX
            );
        }

        if (minCameraY > maxCameraY)
        {
            desiredPosition.y = bounds.center.y;
        }
        else
        {
            desiredPosition.y = Mathf.Clamp(
                desiredPosition.y,
                minCameraY,
                maxCameraY
            );
        }

        // Giữ nguyên trục Z của camera
        desiredPosition.z = transform.position.z;

        float smoothAmount =
            1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothAmount
        );
    }
}