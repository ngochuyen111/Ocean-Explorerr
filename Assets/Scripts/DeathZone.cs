using UnityEngine;
using UnityEngine.UI;

public class DeathZone : MonoBehaviour
{
    [Header("Death Zone Settings")]
    public float damagePerSecond = 50f;
    public float damageTickRate = 0.1f;

    [Header("Visual Settings")]
    public Image darkOverlay;         // Kéo UI Image vào đây
    public float maxAlpha = 0.7f;     // Độ tối tối đa

    private PlayerHealth playerInZone;
    private float tickTimer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = other.GetComponent<PlayerHealth>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = null;
        tickTimer = 0f;
        if (darkOverlay != null)
        {
            Color c = darkOverlay.color;
            c.a = 0f;
            darkOverlay.color = c;
        }
    }

    void Update()
    {
        if (playerInZone == null) return;

        // Damage
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            tickTimer = damageTickRate;
            playerInZone.TakeDamageOverTime(damagePerSecond * damageTickRate);
        }

        // Fade tối dần
        if (darkOverlay != null)
        {
            Color c = darkOverlay.color;
            c.a = Mathf.MoveTowards(c.a, maxAlpha, Time.deltaTime * 0.5f);
            darkOverlay.color = c;
        }
    }
}