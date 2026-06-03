using UnityEngine;

public class OceanProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float damage = 25f;
    public float lifeTime = 4f;
    public bool isUltimate = false;
    public float knockbackForce = 10f;
    public GameObject hitEffectPrefab;

    private Rigidbody2D rb;
    private float direction = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float dir)
    {
        direction = dir;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;

        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Đạn chạm vào: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Đạn bắn trúng Enemy: " + other.name);

            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning(other.name + " chưa có EnemyHealth!");
            }

            OceanEnemy enemyMove = other.GetComponent<OceanEnemy>();
            if (enemyMove != null)
            {
                enemyMove.Knockback(new Vector2(direction, 0.5f), knockbackForce);
            }

            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null && !isUltimate)
            {
                player.AddEnergy(player.energyGainPerHit);
            }

            Hit();
        }

        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Hit();
        }
    }

    void Hit()
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}