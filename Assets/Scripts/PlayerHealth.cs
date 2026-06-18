using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class PlayerHealth : MonoBehaviour
{
    [Header("Máu")]
    public float maxHealth = 100f;
    public float currentHealth;
    [Header("UI")]
    public Slider healthSlider;
    public TMP_Text healthText; // hiện số máu dạng "70/100"
    public Image damageFlash;
    public Color damageColor = new Color(1, 0, 0, 0.45f);
    public float flashSmooth = 5f;
    [Header("Bất tử sau khi bị đánh")]
    public float invincibleTime = 1f;
    [Header("Hồi máu (chạy thanh máu mượt)")]
    public float healBarSpeed = 0.4f; // thời gian (giây) để thanh máu chạy tới giá trị mới
    private bool damaged;
    private bool invincible;
    private SpriteRenderer sr;
    private Animator anim;
    private Color originalColor;
    private Coroutine healBarCoroutine;
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
        UpdateHealthText();
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
        UpdateHealthText();
        if (anim != null) anim.SetTrigger("Hurt");
        StartCoroutine(InvincibleRoutine());
        if (currentHealth <= 0) Die();
    }
    public void AddHealth(float amount)
    {
        float oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        // DÒNG DEBUG - sẽ xóa sau khi xác nhận hoạt động đúng
        Debug.Log($"[PlayerHealth] AddHealth({amount}): {oldHealth} -> {currentHealth} | healthSlider null? {healthSlider == null}");

        UpdateHealthText();

        if (healthSlider != null)
        {
            if (healBarCoroutine != null) StopCoroutine(healBarCoroutine);
            healBarCoroutine = StartCoroutine(SmoothHealthBar(currentHealth));
        }
    }
    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
    }
    IEnumerator SmoothHealthBar(float targetValue)
    {
        float startValue = healthSlider.value;
        float elapsed = 0f;

        while (elapsed < healBarSpeed)
        {
            elapsed += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / healBarSpeed);
            yield return null;
        }

        healthSlider.value = targetValue;
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