using UnityEngine;

public class OceanEnemy : MonoBehaviour
{
    [Header("Di chuyển tự do")]
    public float speed = 2f;
    public float moveRadiusX = 5f;
    public float moveRadiusY = 2f;
    public float arriveDistance = 0.2f;
    public float waitTimeAtPoint = 0.5f;

    [Header("Tấn công")]
    public float contactDamage = 15f;
    public float damageCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float waitTimer;
    private float nextDamageTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        PickNewTarget();
    }

    void FixedUpdate()
    {
        MoveAround();
    }

    void MoveAround()
    {
        if (rb == null) return;

        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 direction = targetPosition - currentPosition;

        if (direction.magnitude <= arriveDistance)
        {
            rb.linearVelocity = Vector2.zero;
            waitTimer = waitTimeAtPoint;
            PickNewTarget();
            return;
        }

        Vector2 moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * speed;

        FlipByDirection(moveDirection.x);
    }

    void PickNewTarget()
    {
        float randomX = Random.Range(-moveRadiusX, moveRadiusX);
        float randomY = Random.Range(-moveRadiusY, moveRadiusY);

        targetPosition = startPosition + new Vector2(randomX, randomY);
    }

    void FlipByDirection(float xDirection)
    {
        if (Mathf.Abs(xDirection) < 0.05f) return;

        Vector3 scale = transform.localScale;

        if (xDirection > 0)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        DamagePlayer(collision.collider);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        DamagePlayer(other);
    }

    void DamagePlayer(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextDamageTime) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(contactDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void Knockback(Vector2 dir, float force)
    {
        if (rb == null) return;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);
    }
}