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

    [Header("Disappear")]
    public float disappearDistance = 0.5f;

    private List<GameObject> dots = new List<GameObject>();

    void OnEnable()
    {
        ClearDots();
    }

    void Update()
    {
        if (player == null || target == null || dashDotPrefab == null)
        {
            ClearDots();
            return;
        }

        BuildTrail();

        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i] == null)
            {
                dots.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(player.position, dots[i].transform.position);

            if (dist <= disappearDistance)
            {
                Destroy(dots[i]);
                dots.RemoveAt(i);
            }
        }
    }

    void BuildTrail()
    {
        ClearDots();

        Vector3 dir = target.position - player.position;
        float distanceToTarget = dir.magnitude;

        if (distanceToTarget <= startDistanceFromPlayer) return;

        dir.Normalize();

        float length = Mathf.Min(maxTrailLength, distanceToTarget - startDistanceFromPlayer);
        int dotCount = Mathf.FloorToInt(length / dotSpacing);

        for (int i = 0; i < dotCount; i++)
        {
            Vector3 pos = player.position + dir * (startDistanceFromPlayer + i * dotSpacing);

            GameObject dot = Instantiate(dashDotPrefab, pos, Quaternion.identity);
            dots.Add(dot);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        gameObject.SetActive(true);
    }

    public void HideTrail()
    {
        ClearDots();
        gameObject.SetActive(false);
    }

    void ClearDots()
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i] != null)
                Destroy(dots[i]);
        }

        dots.Clear();
    }
}