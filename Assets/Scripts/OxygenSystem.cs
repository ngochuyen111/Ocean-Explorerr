using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hệ thống oxy 2D:
/// - Dưới nước hao oxy, càng sâu càng nhanh
/// - Trên mặt nước hồi đầy
/// - Nhặt bình oxy hồi 1 phần
/// - Khi oxy < 30%: TÀU NHẤP NHÁY ĐỎ + nhịp tim đập nhanh
/// - Oxy càng thấp nhấp nháy càng nhanh
/// </summary>
public class OxygenSystem : MonoBehaviour
{
    [Header("Thông số cơ bản")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float baseDrainPerSecond = 2f;

    [Header("Độ sâu (2D - trục Y)")]
    public float waterSurfaceY = 0f;
    public AnimationCurve depthDrainCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(3f, 1f),
        new Keyframe(6f, 1.3f),
        new Keyframe(10f, 1.7f),
        new Keyframe(15f, 2.2f)
    );

    [Header("Ngưỡng cảnh báo")]
    [Range(0f, 1f)] public float warningThreshold = 0.3f;
    [Range(0f, 1f)] public float mediumThreshold = 0.5f;

    [Header("UI thanh oxy")]
    public Slider oxygenSlider;
    public Image oxygenFillImage;
    public TextMeshProUGUI oxyText;
    public Color colorSafe = new Color(0.2f, 0.8f, 0.9f);
    public Color colorMedium = new Color(0.95f, 0.85f, 0.2f);
    public Color colorDanger = new Color(0.9f, 0.2f, 0.2f);

    [Header("Nhấp nháy tàu khi oxy thấp")]
    [Tooltip("SpriteRenderer của tàu. Để trống sẽ tự tìm 'Visual' con của Player.")]
    public SpriteRenderer playerSpriteRenderer;
    public Color flashColor = new Color(1f, 0.2f, 0.2f);
    [Tooltip("Tần suất nhấp nháy (Hz) khi vừa chạm ngưỡng cảnh báo")]
    public float flashSpeedMin = 4f;
    [Tooltip("Tần suất nhấp nháy khi oxy gần về 0 - nhanh dồn dập")]
    public float flashSpeedMax = 10f;
    [Range(0f, 1f)]
    [Tooltip("Cường độ tô đỏ tối đa (1 = đỏ hoàn toàn, 0.7 = pha 70%)")]
    public float flashIntensity = 0.8f;

    [Header("Âm thanh nhịp tim")]
    public AudioSource heartbeatAudioSource;
    public float heartbeatPitchSafe = 0.8f;
    public float heartbeatPitchDanger = 1.6f;

    [Header("Ngộp thở khi oxy = 0")]
    [Tooltip("Sát thương/giây khi player hết oxy. 20 = chết trong 5s (nếu max HP = 100), 10 = chết trong 10s")]
    public float drowningDamagePerSecond = 15f;

    [Header("Sự kiện")]
    public Action OnOxygenDepleted;
    public Action<float> OnOxygenChanged;

    private float hazardMultiplier = 1f;
    private PlayerHealth health;
    private bool isDrowning = false;
    private Color originalSpriteColor;
    private bool hasSpriteColor = false;

    void Start()
    {
        currentOxygen = maxOxygen;
        health = GetComponent<PlayerHealth>();

        // Auto-find SpriteRenderer từ child "Visual" nếu chưa gán
        if (playerSpriteRenderer == null)
        {
            Transform visual = transform.Find("Visual");
            if (visual != null)
                playerSpriteRenderer = visual.GetComponent<SpriteRenderer>();
        }

        if (playerSpriteRenderer != null)
        {
            originalSpriteColor = playerSpriteRenderer.color;
            hasSpriteColor = true;
        }

        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }

        UpdateUI();
    }

    void Update()
    {
        // Luôn tiêu hao oxy theo thời gian (dù ở mặt nước hay dưới sâu)
        // Chỉ khác nhau ở tốc độ: càng sâu hao càng nhanh
        float depthMultiplier = GetDepthMultiplier();
        float drain = baseDrainPerSecond * depthMultiplier * hazardMultiplier * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen - drain, 0f, maxOxygen);

        UpdateUI();
        UpdateWarningEffects();

        // Ngộp thở: khi hết oxy, mất máu dần cho tới khi HP = 0 (PlayerHealth.Die() sẽ tự trigger)
        if (currentOxygen <= 0f)
        {
            if (!isDrowning)
            {
                isDrowning = true;
                OnOxygenDepleted?.Invoke();
            }

            if (health != null)
                health.TakeDamageOverTime(drowningDamagePerSecond * Time.deltaTime);
        }
        else
        {
            isDrowning = false;
        }
    }

    float GetDepthMultiplier()
    {
        float depth = Mathf.Max(0f, waterSurfaceY - transform.position.y);
        return depthDrainCurve.Evaluate(depth);
    }

    public void SetHazardMultiplier(float multiplier) => hazardMultiplier = multiplier;
    public void ResetHazardMultiplier() => hazardMultiplier = 1f;

    public void AddOxygen(float amount)
    {
        currentOxygen = Mathf.Clamp(currentOxygen + amount, 0f, maxOxygen);
        UpdateUI();
        OnOxygenChanged?.Invoke(currentOxygen / maxOxygen);
    }

    void UpdateUI()
    {
        float ratio = currentOxygen / maxOxygen;

        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        if (oxyText != null)
            oxyText.text = $"{Mathf.CeilToInt(currentOxygen)}/{Mathf.CeilToInt(maxOxygen)}";

        if (oxygenFillImage != null)
            oxygenFillImage.color = GetOxygenColor(ratio);
    }

    Color GetOxygenColor(float ratio)
    {
        if (ratio > mediumThreshold) return colorSafe;
        if (ratio > warningThreshold) return colorMedium;
        return colorDanger;
    }

    void UpdateWarningEffects()
    {
        float ratio = currentOxygen / maxOxygen;
        bool isWarning = ratio < warningThreshold;

        // Nhấp nháy tàu (hiệu ứng chính)
        if (hasSpriteColor && playerSpriteRenderer != null)
        {
            if (isWarning)
            {
                // t = 0 khi ratio = warningThreshold, t = 1 khi ratio = 0
                float t = 1f - Mathf.InverseLerp(0f, warningThreshold, ratio);
                float speed = Mathf.Lerp(flashSpeedMin, flashSpeedMax, t);

                // Cường độ tăng dần theo t: từ 40% đỏ (ngưỡng đầu) → 90% đỏ (gần chết)
                float baseIntensity = Mathf.Lerp(0.4f, 0.7f, t);
                float peakIntensity = Mathf.Lerp(0.7f, 1.0f, t);

                // Sóng sin dao động 0-1
                float pulse = (Mathf.Sin(Time.time * speed * Mathf.PI * 2f) + 1f) * 0.5f;
                // Cường độ đỏ luôn trong khoảng [base, peak], không về 0
                float redAmount = Mathf.Lerp(baseIntensity, peakIntensity, pulse) * flashIntensity;

                playerSpriteRenderer.color = Color.Lerp(originalSpriteColor, flashColor, redAmount);
            }
            else
            {
                // Trả về màu gốc khi hết cảnh báo
                playerSpriteRenderer.color = originalSpriteColor;
            }
        }

        // Nhịp tim
        if (heartbeatAudioSource != null)
        {
            if (isWarning)
            {
                if (!heartbeatAudioSource.isPlaying)
                    heartbeatAudioSource.Play();

                float t = 1f - Mathf.InverseLerp(0f, warningThreshold, ratio);
                heartbeatAudioSource.pitch = Mathf.Lerp(heartbeatPitchSafe, heartbeatPitchDanger, t);
            }
            else if (heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.Stop();
            }
        }
    }

    void OnDisable()
    {
        // Khôi phục màu gốc khi disable, tránh tàu giữ màu đỏ nếu chết
        if (hasSpriteColor && playerSpriteRenderer != null)
            playerSpriteRenderer.color = originalSpriteColor;
    }
}