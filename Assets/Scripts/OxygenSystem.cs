using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OxygenSystem : MonoBehaviour
{
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float oxygenDrainPerSecond = 1f;

    public Slider oxygenSlider;
    public TextMeshProUGUI oxyText;

    private PlayerHealth health;

    void Start()
    {
        currentOxygen = maxOxygen;
        health = GetComponent<PlayerHealth>();

        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }

        UpdateUI();
    }

    void Update()
    {
        currentOxygen -= oxygenDrainPerSecond * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        UpdateUI();

        if (currentOxygen <= 0)
        {
            if (health != null)
                health.Die();
        }
    }

    void UpdateUI()
    {
        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        if (oxyText != null)
            oxyText.text = $"{Mathf.CeilToInt(currentOxygen)}/{Mathf.CeilToInt(maxOxygen)}";
    }

    public void AddOxygen(float amount)
    {
        currentOxygen = Mathf.Clamp(currentOxygen + amount, 0, maxOxygen);
        UpdateUI();
    }

    //Hanh them de mua oxy
    public void BuyOxygen()
    {
        if (ScoreManager.instance == null)
        {
            Debug.LogWarning("Không tìm thấy ScoreManager.");
            return;
        }

        if (ScoreManager.instance.SpendPearls(10))
        {
            AddOxygen(50f);
            Debug.Log("Đã mua thêm 50 oxy.");
        }
    }
}