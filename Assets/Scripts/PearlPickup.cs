using UnityEngine;

public class PearlPickup : MonoBehaviour
{
    [Header("Điểm/số ngọc trai cộng vào ScoreManager")]
    public int value = 1;

    [Header("Lượng máu hồi khi ăn viên này (ngọc nhỏ = 3, ngọc lớn = 5)")]
    public float healAmount = 3f;

    public GameObject collectEffectPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (ScoreManager.instance != null)
            ScoreManager.instance.AddPearl(value);

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        // DÒNG DEBUG - sẽ xóa sau khi xác nhận hoạt động đúng
        Debug.Log($"[Pearl] Tìm PlayerHealth trên '{other.name}': {(playerHealth != null ? "TÌM THẤY" : "KHÔNG TÌM THẤY")}");

        if (playerHealth != null)
            playerHealth.AddHealth(healAmount);

        if (collectEffectPrefab != null)
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}