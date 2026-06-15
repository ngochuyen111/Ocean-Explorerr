using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Máu")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public Image damageFlash;
    public Color damageColor = new Color(1, 0, 0, 0.45f);
    public float flashSmooth = 5f;

    [Header("Bất tử sau khi bị đánh")]
    public float invincibleTime = 1f;

    private bool damaged;
    private bool invincible;
    private SpriteRenderer sr;
    private Animator anim;
    private Color originalColor;
    void Start()
    {
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (sr != null)
            originalColor = sr.color;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        if (damageFlash == null) return;

        if (damaged)
            damageFlash.color = damageColor;
        else
            damageFlash.color = Color.Lerp(damageFlash.color, Color.clear, flashSmooth * Time.deltaTime);

        damaged = false;
    }

    public void TakeDamage(float damage)
    {
        if (invincible || damage <= 0) return;

        currentHealth -= damage;
        damaged = true;

        if (healthSlider != null) healthSlider.value = currentHealth;
        if (anim != null) anim.SetTrigger("Hurt");

        StartCoroutine(InvincibleRoutine());

        if (currentHealth <= 0) Die();
    }

    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (healthSlider != null) healthSlider.value = currentHealth;
    }

    IEnumerator InvincibleRoutine()
    {
        invincible = true;

        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            if (sr != null)
            {
                sr.color = Color.red;
            }

            yield return new WaitForSeconds(0.1f);

            if (sr != null)
            {
                sr.color = originalColor;
            }

            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        if (sr != null)
        {
            sr.color = originalColor;
        }

        invincible = false;
    }
    public void Die()
    {
        PlayerPrefs.SetString("LastLevel",
        SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("GameOver");
    }
}