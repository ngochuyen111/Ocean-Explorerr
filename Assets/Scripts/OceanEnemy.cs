using UnityEngine;

public class OceanEnemy : MonoBehaviour
{
    [Header("Di chuyển tự do")]
    public float speed = 2f;
    public float moveRadiusX = 4f;
    public float moveRadiusY = 1.5f;
    public float arriveDistance = 0.2f;
    public float waitTimeAtPoint = 0.5f;

    [Header("Đuổi Player")]
    public float detectRange = 3f;
    public float stopChaseRange = 4.5f;
    public float chaseSpeed = 2.5f;

    [Header("Tấn công")]
    public float contactDamage = 15f;
    public float damageCooldown = 1f;
    public float pushPlayerForce = 1.5f;

    [Header("Knockback")]
    public float knockbackTime = 0.25f;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float waitTimer;
    private float nextDamageTime;
    private bool knocked;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        PickNewTarget();
    }

    void FixedUpdate()
    {
        if (rb == null || knocked) return;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectRange)
            {
                ChasePlayer();
                return;
            }
        }

        MoveAround();
    }

    void MoveAround()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = targetPosition - rb.position;

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

    void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        FlipByDirection(direction.x);
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

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);
            PushPlayer(other);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    void PushPlayer(Collider2D other)
    {
        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        Vector2 pushDir = (other.transform.position - transform.position).normalized;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.AddForce(pushDir * pushPlayerForce, ForceMode2D.Impulse);
    }

    public void Knockback(Vector2 dir, float force)
    {
        if (rb == null) return;

        knocked = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);
        Invoke(nameof(EndKnockback), knockbackTime);
    }

    void EndKnockback()
    {
        knocked = false;
    }
}