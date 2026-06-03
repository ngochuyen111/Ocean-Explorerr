using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 60f;
    public float currentHealth;
    public Slider healthBar;
    public GameObject dropItemPrefab;
    public int killScore = 1;

    [Header("Hit Effect")]
    public Color hitColor = Color.red;
    public float flashTime = 0.15f;
    public float jumpForce = 3f;

    private Animator anim;
    private bool dead;
    private SpriteRenderer sr;
    private Color originalColor;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (sr != null)
            originalColor = sr.color;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " bị bắn, còn máu: " + currentHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (anim != null)
            anim.SetTrigger("Hurt");

        StartCoroutine(HitEffect());

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator HitEffect()
    {
        if (sr == null) yield break;

        sr.color = hitColor;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }

    void Die()
    {
        dead = true;
        Debug.Log(gameObject.name + " chết!");

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

        Destroy(gameObject, 0.2f);
    }
}