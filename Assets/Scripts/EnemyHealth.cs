using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 60f;
    public float currentHealth;
    public Slider healthBar;
    public GameObject dropItemPrefab;
    public int killScore = 1;

    private Animator anim;
    private bool dead;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();

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

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (anim != null)
            anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        dead = true;

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

        if (anim != null)
            anim.SetTrigger("Dead");

        Destroy(gameObject, 1.2f);
    }
}