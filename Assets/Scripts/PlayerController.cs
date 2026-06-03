using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 7f;
    public float dashForce = 14f;
    public float dashCooldown = 1f;

    [Header("Tấn công")]
    public GameObject bubbleBulletPrefab;
    public GameObject waveSkillPrefab;
    public Transform shootPoint;
    public float fireRate = 0.35f;

    [Header("Năng lượng skill")]
    public float currentEnergy = 0f;
    public float maxEnergy = 100f;
    public float energyGainPerHit = 10f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private float moveX;
    private float moveY;

    private bool facingRight = true;
    private bool canDash = true;
    private bool isAttacking = false;
    private float nextFireTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Di chuyển A/D/W/S hoặc phím mũi tên
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        // Dash bằng Shift trái
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Bắn thường bằng Space
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime && !isAttacking)
        {
            StartCoroutine(Shoot(false));
        }

        // Skill bằng R, cần đủ năng lượng
        if (Input.GetKeyDown(KeyCode.R) && currentEnergy >= maxEnergy && !isAttacking)
        {
            StartCoroutine(Shoot(true));
        }

        UpdateFacing();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.linearVelocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);
        }
    }

    IEnumerator Shoot(bool ultimate)
    {
        isAttacking = true;
        nextFireTime = Time.time + fireRate;

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(0.1f);

        GameObject prefab = ultimate ? waveSkillPrefab : bubbleBulletPrefab;

        if (prefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(prefab, shootPoint.position, Quaternion.identity);

            OceanProjectile projectile = bullet.GetComponent<OceanProjectile>();

            if (projectile != null)
            {
                projectile.Initialize(facingRight ? 1f : -1f);
                projectile.isUltimate = ultimate;
            }
        }
        else
        {
            Debug.LogWarning("Thiếu prefab đạn hoặc ShootPoint trong PlayerController.");
        }

        if (ultimate)
        {
            currentEnergy = 0f;
        }

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
    }

    IEnumerator Dash()
    {
        canDash = false;

        float dashDirection = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetTrigger("Dash");
        }

        yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void UpdateFacing()
    {
        if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveX < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f;
        anim.SetBool("Swim", isMoving);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    public void AddEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
    }
}