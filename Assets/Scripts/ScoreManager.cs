using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI pearlText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI scoreText;

    public int pearls;
    public int kills;
    public int score;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddPearl(int amount)
    {
        pearls += amount;
        score += amount * 10;
        UpdateUI();
    }

    public void AddKill(int amount = 1)
    {
        kills += amount;
        score += amount * 50;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (pearlText != null) pearlText.text = pearls.ToString();
        if (killText != null) killText.text = kills.ToString();
        if (scoreText != null) scoreText.text = score.ToString();
    }
}