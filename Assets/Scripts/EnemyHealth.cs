using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;
    public Image healthFill;
    public GameObject dropItemPrefab;
    public int killScore = 1;
    public float healPlayerWhenDead = 25f;

    [Header("Hit Effect")]
    public Color hitColor = Color.red;
    public float flashTime = 0.15f;
    public float jumpForce = 3f;

    [Header("Health Bar Effect")]
    public float healthSmoothSpeed = 6f;
    public Color normalHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float lowHealthPercent = 0.3f;

    [Header("Health Regen")]
    public bool canRegen = false;
    public float regenDelay = 3f;
    public float regenAmountPerSecond = 5f;

    private Animator anim;
    private bool dead;
    private SpriteRenderer sr;
    private Color originalColor;
    private Rigidbody2D rb;
    private float displayedHealth;
    private float lastDamageTime;

    void Start()
    {
        currentHealth = maxHealth;
        displayedHealth = maxHealth;

        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (sr != null)
            originalColor = sr.color;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = displayedHealth;
        }

        UpdateHealthColor();
    }

    void UpdateHealthColor()
    {
        if (healthFill == null) return;

        float percent = displayedHealth / maxHealth;

        if (percent <= lowHealthPercent)
            healthFill.color = lowHealthColor;
        else
            healthFill.color = normalHealthColor;
    }

    void Update()
    {
        UpdateSmoothHealthBar();
        RegenerateHealth();
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime = Time.time;

        if (anim != null)
            anim.SetTrigger("Hurt");

        StartCoroutine(HitEffect());

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAfterHealthBarEmpty());
        }
    }

    void UpdateSmoothHealthBar()
    {
        if (healthBar == null) return;

        displayedHealth = Mathf.Lerp(displayedHealth, currentHealth, healthSmoothSpeed * Time.deltaTime);
        healthBar.value = displayedHealth;

        UpdateHealthColor();
    }


    void RegenerateHealth()
    {
        if (!canRegen) return;
        if (dead) return;
        if (currentHealth <= 0) return;
        if (currentHealth >= maxHealth) return;
        if (Time.time < lastDamageTime + regenDelay) return;

        currentHealth += regenAmountPerSecond * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    IEnumerator HitEffect()
    {
        if (sr == null) yield break;

        sr.color = hitColor;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    IEnumerator DieAfterHealthBarEmpty()
    {
        if (dead) yield break;
        dead = true;

        while (displayedHealth > 1f)
        {
            yield return null;
        }

        Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " chết!");

        //Hanh
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.AddHealth(healPlayerWhenDead);
        }
        //
        if (ScoreManager.instance != null)
            ScoreManager.instance.AddKill(killScore);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = false;

        if (dropItemPrefab != null)
            Instantiate(dropItemPrefab, transform.position + Vector3.up, Quaternion.identity);

        StartCoroutine(DeathEffect());
    }

    IEnumerator DeathEffect()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}