using System.Collections;
using UnityEngine;

public class OceanEnemy : MonoBehaviour
{
    // Chọn loại enemy trong Inspector
    public enum EnemyType
    {
        Shark,
        Octopus,
        Barracuda,
        AnglerBoss
    }

    [Header("Loại Enemy")]
    public EnemyType enemyType = EnemyType.Shark;

    [Header("Di chuyển tự do")]
    public float speed = 2f;
    public float moveRadiusX = 4f;
    public float moveRadiusY = 1.5f;
    public float arriveDistance = 0.2f;
    public float waitTimeAtPoint = 0.5f;

    [Header("Đuổi Player")]
    public float detectRange = 3f;
    public float chaseSpeed = 2.5f;

    [Header("Tấn công khi chạm")]
    public float contactDamage = 15f;
    public float damageCooldown = 1f;
    public float pushPlayerForce = 1.5f;

    [Header("Knockback")]
    public float knockbackTime = 0.25f;

    // BẠCH TUỘC PHUN MỰC

    [Header("Bạch tuộc - Phun mực")]
    public GameObject inkProjectilePrefab;
    public Transform inkShootPoint;
    public float inkAttackRange = 6f;
    public float inkCooldown = 3f;

    // CÁ NHỒNG LAO NHANH

    [Header("Cá nhồng - Lao nhanh")]
    public float chargeSpeed = 9f;
    public float chargeTime = 0.5f;
    public float chargeCooldown = 3f;
    public float prepareChargeTime = 0.4f;

    // CÁ CẦN CÂU BOSS PHÁT SÁNG

    [Header("Cá cần câu Boss - Phát sáng")]
    public SpriteRenderer lureGlow;
    public float glowDuration = 2f;
    public float normalDuration = 3f;
    public float glowingDamageMultiplier = 2f;
    public float glowPulseSpeed = 5f;

    [Header("Hiệu ứng vòng sáng")]
    public float minGlowScale = 0.7f;
    public float maxGlowScale = 1.2f;
    public float minGlowAlpha = 0.25f;
    public float maxGlowAlpha = 0.8f;

    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private Vector2 startPosition;
    private Vector2 targetPosition;

    private float waitTimer;
    private float nextDamageTime;
    private float nextSpecialTime;

    private bool knocked;
    private bool usingSpecial;
    private bool isGlowing;

    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Trường hợp SpriteRenderer nằm trong object con
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (enemyType == EnemyType.AnglerBoss && lureGlow == null)
        {
            Transform glowObject = transform.Find("LureLight");

            if (glowObject != null)
            {
                lureGlow = glowObject.GetComponent<SpriteRenderer>();
            }
        }

        if (lureGlow != null)
        {
            lureGlow.gameObject.SetActive(false);
        }

        startPosition = transform.position;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        PickNewTarget();

        // Cá cần câu bắt đầu chu kỳ phát sáng
        if (enemyType == EnemyType.AnglerBoss)
        {
            StartCoroutine(AnglerGlowRoutine());
        }
    }

    void Update()
    {
        if (enemyType != EnemyType.AnglerBoss)
            return;

        if (lureGlow == null)
            return;

        if (isGlowing)
        {
            if (!lureGlow.gameObject.activeSelf)
            {
                lureGlow.gameObject.SetActive(true);
            }

            float pulse = Mathf.PingPong(
                Time.time * glowPulseSpeed,
                1f
            );

            float glowScale = Mathf.Lerp(
                minGlowScale,
                maxGlowScale,
                pulse
            );

            lureGlow.transform.localScale =
                new Vector3(
                    glowScale,
                    glowScale,
                    1f
                );

            Color glowCurrentColor =
                lureGlow.color;

            glowCurrentColor.a =
                Mathf.Lerp(
                    minGlowAlpha,
                    maxGlowAlpha,
                    pulse
                );

            lureGlow.color =
                glowCurrentColor;
        }
        else
        {
            if (lureGlow.gameObject.activeSelf)
            {
                lureGlow.gameObject.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (knocked || usingSpecial)
            return;

        if (player == null)
        {
            MoveAround();
            return;
        }

        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );

        // Bạch tuộc phun mực khi Player vào tầm
        if (enemyType == EnemyType.Octopus)
        {
            if (distanceToPlayer <= inkAttackRange &&
                Time.time >= nextSpecialTime)
            {
                ShootInk();
                return;
            }
        }

        // Cá nhồng lao nhanh khi Player vào tầm
        if (enemyType == EnemyType.Barracuda)
        {
            if (distanceToPlayer <= detectRange &&
                Time.time >= nextSpecialTime)
            {
                StartCoroutine(ChargePlayer());
                return;
            }
        }

        // Các enemy đều có thể đuổi Player
        if (distanceToPlayer <= detectRange)
        {
            ChasePlayer();
            return;
        }

        MoveAround();
    }

    // DI CHUYỂN CHUNG

    void ChasePlayer()
    {
        if (player == null)
            return;

        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity =
            direction * chaseSpeed;

        FlipByDirection(direction.x);
    }

    void MoveAround()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction =
            targetPosition - rb.position;

        if (direction.magnitude <= arriveDistance)
        {
            rb.linearVelocity = Vector2.zero;

            waitTimer = waitTimeAtPoint;

            PickNewTarget();

            return;
        }

        Vector2 moveDirection =
            direction.normalized;

        rb.linearVelocity =
            moveDirection * speed;

        FlipByDirection(moveDirection.x);
    }

    void PickNewTarget()
    {
        float randomX =
            Random.Range(
                -moveRadiusX,
                moveRadiusX
            );

        float randomY =
            Random.Range(
                -moveRadiusY,
                moveRadiusY
            );

        targetPosition =
            startPosition +
            new Vector2(randomX, randomY);
    }

    void FlipByDirection(float xDirection)
    {
        if (Mathf.Abs(xDirection) < 0.05f)
            return;

        Vector3 scale =
            transform.localScale;

        if (xDirection > 0)
        {
            scale.x =
                Mathf.Abs(scale.x);
        }
        else
        {
            scale.x =
                -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    // BẠCH TUỘC PHUN MỰC

    void ShootInk()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayInkShoot();
        }

        if (inkProjectilePrefab == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " chưa gắn Ink Projectile Prefab!"
            );

            return;
        }

        if (player == null)
            return;

        nextSpecialTime =
            Time.time + inkCooldown;

        rb.linearVelocity =
            Vector2.zero;

        Vector3 shootPosition =
            transform.position;

        if (inkShootPoint != null)
        {
            shootPosition =
                inkShootPoint.position;
        }

        Vector2 direction =
            ((Vector2)player.position -
             (Vector2)shootPosition).normalized;

        GameObject ink =
            Instantiate(
                inkProjectilePrefab,
                shootPosition,
                Quaternion.identity
            );

        InkProjectile projectile =
            ink.GetComponent<InkProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(direction);
        }

        FlipByDirection(direction.x);
    }

    // CÁ NHỒNG LAO VỀ PLAYER

    IEnumerator ChargePlayer()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayCharge();
        }

        if (player == null)
            yield break;

        usingSpecial = true;

        nextSpecialTime =
            Time.time + chargeCooldown;

        // Dừng lại báo hiệu trước khi lao
        rb.linearVelocity =
            Vector2.zero;

        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                new Color(1f, 0.5f, 0.5f, 1f);
        }

        yield return
            new WaitForSeconds(
                prepareChargeTime
            );

        if (player == null)
        {
            usingSpecial = false;
            yield break;
        }

        Vector2 chargeDirection =
            ((Vector2)player.position -
             rb.position).normalized;

        FlipByDirection(chargeDirection.x);

        rb.linearVelocity =
            chargeDirection * chargeSpeed;

        yield return
            new WaitForSeconds(chargeTime);

        rb.linearVelocity =
            Vector2.zero;

        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                originalColor;
        }

        usingSpecial = false;
    }

    // CÁ CẦN CÂU PHÁT SÁNG

    IEnumerator AnglerGlowRoutine()
    {
        while (true)
        {
            isGlowing = false;

            if (lureGlow != null)
            {
                lureGlow.gameObject.SetActive(false);
            }

            yield return
                new WaitForSeconds(
                    normalDuration
                );

            isGlowing = true;

            if (lureGlow != null)
            {
                lureGlow.gameObject.SetActive(true);
            }

            yield return
                new WaitForSeconds(
                    glowDuration
                );
        }
    }

    // VA CHẠM PLAYER

    private void OnCollisionStay2D(Collision2D collision)
    {
        DamagePlayer(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DamagePlayer(other.gameObject);
    }

    private void DamagePlayer(GameObject hitObject)
    {
        // Tìm Rigidbody2D của Player kể cả collider nằm ở object con
        Rigidbody2D playerRb =
            hitObject.GetComponentInParent<Rigidbody2D>();

        if (playerRb == null)
            return;

        if (!playerRb.CompareTag("Player"))
            return;

        if (Time.time < nextDamageTime)
            return;

        PlayerHealth playerHealth =
            playerRb.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth =
                playerRb.GetComponentInChildren<PlayerHealth>();
        }

        if (playerHealth == null)
            return;

        float finalDamage = contactDamage;

        // Cá cần câu đang sáng gây gấp đôi damage
        if (enemyType == EnemyType.AnglerBoss &&
            isGlowing)
        {
            finalDamage =
                contactDamage * glowingDamageMultiplier;
        }

        playerHealth.TakeDamage(finalDamage);

        PushPlayer(playerRb);

        nextDamageTime =
            Time.time + damageCooldown;
    }

    private void PushPlayer(Rigidbody2D playerRb)
    {
        if (playerRb == null)
            return;

        Vector2 pushDirection =
            ((Vector2)playerRb.transform.position -
             (Vector2)transform.position).normalized;

        playerRb.linearVelocity = Vector2.zero;

        playerRb.AddForce(
            pushDirection * pushPlayerForce,
            ForceMode2D.Impulse
        );
    }

    // ENEMY BỊ ĐẠN ĐẨY LÙI

    public void Knockback(
        Vector2 direction,
        float force)
    {
        if (rb == null)
            return;

        // Boss khó bị knockback hơn
        if (enemyType == EnemyType.AnglerBoss)
        {
            force *= 0.35f;
        }

        knocked = true;

        rb.linearVelocity =
            Vector2.zero;

        rb.AddForce(
            direction.normalized * force,
            ForceMode2D.Impulse
        );

        Invoke(
            nameof(EndKnockback),
            knockbackTime
        );
    }

    void EndKnockback()
    {
        knocked = false;
    }
}