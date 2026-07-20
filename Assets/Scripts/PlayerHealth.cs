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
    //Hanh update level3
    private static float savedHealth;
    private static float savedMaxHealth;
    private static bool hasSavedHealth = false;
    //
    [Header("UI")]
    public Slider healthSlider;
    public TMP_Text healthText; // hiện số máu dạng "70/100"
    public Image damageFlash;
    public Color damageColor = new Color(1, 0, 0, 0.45f);
    public float flashSmooth = 5f;
    [Header("Bất tử sau khi bị đánh")]
    public float invincibleTime = 1f;
    [Header("Hiệu ứng nháy đỏ")]
    public Color hitFlashColor = Color.red;
    public float redDuration = 0.15f;
    public float normalDuration = 0.05f;
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
        string currentScene = SceneManager.GetActiveScene().name;

        // Vào Level 1 thì bắt đầu game mới và hồi đầy máu
        if (currentScene == "Level1")
        {
            currentHealth = maxHealth;

            savedHealth = currentHealth;
            savedMaxHealth = maxHealth;
            hasSavedHealth = true;
        }
        // Sang level mới thì lấy máu cũ cộng phần Max Health được tăng thêm
        else if (hasSavedHealth)
        {
            float increasedHealth = Mathf.Max(0f, maxHealth - savedMaxHealth);

            currentHealth = Mathf.Clamp(
                savedHealth + increasedHealth,
                0f,
                maxHealth
            );

            savedHealth = currentHealth;
            savedMaxHealth = maxHealth;
        }
        // Trường hợp mở trực tiếp Level 2, 3 hoặc 4 để kiểm tra
        else
        {
            currentHealth = maxHealth;
            savedHealth = currentHealth;
            savedMaxHealth = maxHealth;
            hasSavedHealth = true;
        }

        Transform visual = transform.Find("Visual");

        if (visual != null)
        {
            sr = visual.GetComponent<SpriteRenderer>();
            anim = visual.GetComponent<Animator>();
        }
        else
        {
            // Dùng component trên Player nếu không có object con Visual
            sr = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();

            Debug.LogWarning(
                "Player không có object con Visual. Đang tìm SpriteRenderer và Animator trên Player."
            );
        }

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
        // Kiểm tra trước, tránh gọi hiệu ứng và âm thanh liên tục
        if (invincible || damage <= 0f)
        {
            return;
        }

        // Bật ngay để tránh nhận nhiều damage trong cùng một frame
        invincible = true;

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayPlayerHit();
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        savedHealth = currentHealth;
        savedMaxHealth = maxHealth;

        damaged = true;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        UpdateHealthText();

        if (anim != null)
        {
            anim.SetTrigger("Hurt");
        }

        StartCoroutine(InvincibleRoutine());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void TakeDamageOverTime(float damage)
    {
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        savedHealth = currentHealth;
        savedMaxHealth = maxHealth;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }
    public void AddHealth(float amount)
    {
        float oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        savedHealth = currentHealth;
        savedMaxHealth = maxHealth;

        // DÒNG DEBUG - sẽ xóa sau khi xác nhận hoạt động đúng
        Debug.Log($"[PlayerHealth] AddHealth({amount}): {oldHealth} -> {currentHealth} | healthSlider null? {healthSlider == null}");

        UpdateHealthText();

        if (healthSlider != null)
        {
            if (healBarCoroutine != null) StopCoroutine(healBarCoroutine);
            healBarCoroutine = StartCoroutine(SmoothHealthBar(currentHealth));
        }
    }
    //Hanh them de mua mau
    public void BuyHealth()
    {
        if (ScoreManager.instance == null)
        {
            Debug.LogWarning("Không tìm thấy ScoreManager.");
            return;
        }

        if (currentHealth >= maxHealth)
        {
            Debug.Log("Máu đang đầy, không cần mua.");
            return;
        }

        if (ScoreManager.instance.SpendPearls(15))
        {
            AddHealth(50f);
            Debug.Log("Đã dùng 15 ngọc trai để hồi 50 máu.");
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
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            // Đổi tàu sang màu đỏ
            if (sr != null)
            {
                sr.color = hitFlashColor;
            }

            yield return new WaitForSeconds(redDuration);
            elapsed += redDuration;

            // Trở về màu ban đầu
            if (sr != null)
            {
                sr.color = originalColor;
            }

            if (elapsed >= invincibleTime)
            {
                break;
            }

            yield return new WaitForSeconds(normalDuration);
            elapsed += normalDuration;
        }

        if (sr != null)
        {
            sr.color = originalColor;
        }

        invincible = false;
    }
    public void Die()
    {
        hasSavedHealth = false;
        savedHealth = 0f;
        savedMaxHealth = 0f;

        string currentLevel =
            SceneManager.GetActiveScene().name;

        PlayerPrefs.SetString(
            "LastLevel",
            currentLevel
        );

        PlayerPrefs.Save();

        Time.timeScale = 1f;

        SceneManager.LoadScene("GameOver");
    }
}