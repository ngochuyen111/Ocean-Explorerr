using UnityEngine;

/// <summary>
/// Bình dưỡng khí trong level 2D. Chạm vào để hồi 1 phần oxy (không hồi đầy).
/// Đặt ít, có chủ đích - gần điểm rẽ nhánh hoặc khu vực nguy hiểm.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class OxygenPickup : MonoBehaviour
{
    [Tooltip("Lượng oxy hồi khi nhặt (+30)")]
    public float refillAmount = 30f;

    public GameObject pickupVFX;
    public AudioClip pickupSFX;
    public OxygenPickupSpawner spawner;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var oxygenSystem = other.GetComponent<OxygenSystem>();
        if (oxygenSystem == null) return;

        oxygenSystem.AddOxygen(refillAmount);

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddOxygenPickup();
        }

        if (pickupVFX != null)
            Instantiate(pickupVFX, transform.position, Quaternion.identity);

        if (pickupSFX != null)
            AudioSource.PlayClipAtPoint(pickupSFX, transform.position);

        if (spawner != null)
        {
            spawner.PickupCollected();
        }

        Destroy(gameObject);
    }
}