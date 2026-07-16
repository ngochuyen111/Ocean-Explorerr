using UnityEngine;

public class InkProjectile : MonoBehaviour
{
    [Header("Mực của bạch tuộc")]
    public float speed = 5f;
    public float damage = 15f;
    public float lifeTime = 4f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection =
            direction.normalized;

        Destroy(
            gameObject,
            lifeTime
        );
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.linearVelocity =
            moveDirection * speed;
    }

    void OnTriggerEnter2D(
        Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(
                    damage
                );
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground") ||
            other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}