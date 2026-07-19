using UnityEngine;

/// <summary>
/// Vùng nguy hiểm 2D (túi khí độc, dòng xoáy) - tăng tốc độ hao oxy khi Player trong vùng.
/// Khác với DeathZone: DeathZone giết ngay bằng máu, HazardZone chỉ đẩy nhanh việc hết oxy.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HazardZone : MonoBehaviour
{
    public enum HazardType { ToxicPocket, Whirlpool }

    [Header("Loại vùng nguy hiểm")]
    public HazardType hazardType = HazardType.ToxicPocket;
    [Tooltip("Hệ số nhân tốc độ hao oxy khi ở trong vùng (1.8 = hao gấp 1.8 lần)")]
    public float hazardMultiplier = 1.8f;

    [Header("Đẩy player (dùng cho dòng xoáy)")]
    public bool appliesPushForce = false;
    public Vector2 pushDirection = Vector2.up;
    public float pushForce = 5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var oxygenSystem = other.GetComponent<OxygenSystem>();
        if (oxygenSystem != null)
            oxygenSystem.SetHazardMultiplier(hazardMultiplier);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (appliesPushForce)
        {
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(pushDirection.normalized * pushForce, ForceMode2D.Force);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var oxygenSystem = other.GetComponent<OxygenSystem>();
        if (oxygenSystem != null)
            oxygenSystem.ResetHazardMultiplier();
    }
}