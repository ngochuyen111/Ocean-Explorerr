using System.Collections.Generic;
using UnityEngine;

public class TreasureTrailArrow : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform target;
    public GameObject dashDotPrefab;

    [Header("Trail Settings")]
    public float dotSpacing = 0.8f;
    public float startDistanceFromPlayer = 1.2f;
    public float maxTrailLength = 8f;

    [Header("Curve Settings")]
    [Tooltip("Độ cong của đường dẫn")]
    public float curveStrength = 1.5f;

    [Tooltip("1 = cong sang một bên, -1 = cong sang bên ngược lại")]
    public float curveDirection = 1f;

    [Header("Disappear")]
    public float disappearDistance = 0.5f;

    private readonly List<GameObject> dots = new List<GameObject>();

    private bool trailCreated;

    private void OnEnable()
    {
        trailCreated = false;
    }

    private void Update()
    {
        if (player == null || target == null || dashDotPrefab == null)
        {
            return;
        }

        // Chỉ tạo đường dẫn một lần
        if (!trailCreated)
        {
            CreateCurvedTrail();
        }

        // Xóa những chấm Player đã đi qua
        RemovePassedDots();
    }

    private void CreateCurvedTrail()
    {
        ClearDots();

        Vector2 playerPosition = player.position;
        Vector2 targetPosition = target.position;

        Vector2 direction = targetPosition - playerPosition;
        float distanceToTarget = direction.magnitude;

        if (distanceToTarget <= startDistanceFromPlayer)
        {
            trailCreated = true;
            return;
        }

        direction.Normalize();

        // Điểm bắt đầu cách Player một khoảng
        Vector2 startPoint =
            playerPosition + direction * startDistanceFromPlayer;

        float availableDistance =
            distanceToTarget - startDistanceFromPlayer;

        float trailLength =
            Mathf.Min(maxTrailLength, availableDistance);

        // Điểm cuối của đường dẫn
        Vector2 endPoint =
            startPoint + direction * trailLength;

        // Vector vuông góc để làm đường cong
        Vector2 perpendicular =
            new Vector2(-direction.y, direction.x);

        // Điểm điều khiển của đường cong Bezier
        Vector2 controlPoint =
            (startPoint + endPoint) / 2f
            + perpendicular * curveStrength * curveDirection;

        int dotCount =
            Mathf.Max(2, Mathf.CeilToInt(trailLength / dotSpacing));

        for (int i = 0; i <= dotCount; i++)
        {
            float t = (float)i / dotCount;

            // Vị trí trên đường cong Bezier
            Vector2 dotPosition = CalculateBezierPoint(
                startPoint,
                controlPoint,
                endPoint,
                t
            );

            // Lấy một điểm phía trước để xác định hướng xoay
            float nextT = Mathf.Min(t + 0.02f, 1f);

            Vector2 nextPosition = CalculateBezierPoint(
                startPoint,
                controlPoint,
                endPoint,
                nextT
            );

            Vector2 dotDirection =
                (nextPosition - dotPosition).normalized;

            float angle =
                Mathf.Atan2(dotDirection.y, dotDirection.x)
                * Mathf.Rad2Deg
                - 90f;

            Quaternion rotation =
                Quaternion.Euler(0f, 0f, angle);

            Vector3 spawnPosition = new Vector3(
                dotPosition.x,
                dotPosition.y,
                player.position.z
            );

            GameObject dot = Instantiate(
                dashDotPrefab,
                spawnPosition,
                rotation
            );

            dots.Add(dot);
        }

        trailCreated = true;
    }

    private Vector2 CalculateBezierPoint(
        Vector2 start,
        Vector2 control,
        Vector2 end,
        float t)
    {
        float oneMinusT = 1f - t;

        return oneMinusT * oneMinusT * start
               + 2f * oneMinusT * t * control
               + t * t * end;
    }

    private void RemovePassedDots()
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i] == null)
            {
                dots.RemoveAt(i);
                continue;
            }

            float distance = Vector2.Distance(
                player.position,
                dots[i].transform.position
            );

            if (distance <= disappearDistance)
            {
                Destroy(dots[i]);
                dots.RemoveAt(i);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        ClearDots();
        trailCreated = false;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void HideTrail()
    {
        ClearDots();
        trailCreated = false;
        gameObject.SetActive(false);
    }

    private void ClearDots()
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i] != null)
            {
                Destroy(dots[i]);
            }
        }

        dots.Clear();
    }
}