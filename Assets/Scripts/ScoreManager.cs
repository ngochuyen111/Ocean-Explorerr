using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI")]
    public TextMeshProUGUI pearlText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI scoreText;

    [Header("Current Progress")]
    public int pearls;
    public int kills;
    public int score;

    private const string PearlsKey = "SavedPearls";
    private const string KillsKey = "SavedKills";
    private const string ScoreKey = "SavedScore";
    private const string LevelKey = "SavedLevel";
    private const string HasSaveKey = "HasSaveGame";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindSceneUI();
        UpdateUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneUI();
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

    public void SaveCheckpoint(string nextLevelName)
    {
        PlayerPrefs.SetInt(PearlsKey, pearls);
        PlayerPrefs.SetInt(KillsKey, kills);
        PlayerPrefs.SetInt(ScoreKey, score);

        PlayerPrefs.SetString(LevelKey, nextLevelName);

        PlayerPrefs.SetInt(HasSaveKey, 1);
        PlayerPrefs.Save();

        Debug.Log(
            "Checkpoint saved!" +
            "\nNext level: " + nextLevelName +
            "\nPearls: " + pearls +
            "\nKills: " + kills +
            "\nScore: " + score
        );
    }

    public void LoadCheckpoint()
    {
        pearls = PlayerPrefs.GetInt(PearlsKey, 0);
        kills = PlayerPrefs.GetInt(KillsKey, 0);
        score = PlayerPrefs.GetInt(ScoreKey, 0);

        UpdateUI();

        Debug.Log(
            "Checkpoint loaded!" +
            "\nPearls: " + pearls +
            "\nKills: " + kills +
            "\nScore: " + score
        );
    }

    public string GetSavedLevel()
    {
        return PlayerPrefs.GetString(LevelKey, "Level1");
    }

    public bool HasSavedGame()
    {
        return PlayerPrefs.GetInt(HasSaveKey, 0) == 1;
    }

    public void StartNewGame()
    {
        pearls = 0;
        kills = 0;
        score = 0;

        PlayerPrefs.DeleteKey(PearlsKey);
        PlayerPrefs.DeleteKey(KillsKey);
        PlayerPrefs.DeleteKey(ScoreKey);
        PlayerPrefs.DeleteKey(LevelKey);
        PlayerPrefs.DeleteKey(HasSaveKey);

        PlayerPrefs.Save();
        UpdateUI();

        SaveCheckpoint("Level1");

        Debug.Log("A new game has started.");
    }

    private void FindSceneUI()
    {
        GameObject scoreObject = GameObject.Find("ScoreText");
        GameObject pearlObject = GameObject.Find("PearlText");
        GameObject killObject = GameObject.Find("KillText");

        scoreText = scoreObject != null
            ? scoreObject.GetComponent<TextMeshProUGUI>()
            : null;

        pearlText = pearlObject != null
            ? pearlObject.GetComponent<TextMeshProUGUI>()
            : null;

        killText = killObject != null
            ? killObject.GetComponent<TextMeshProUGUI>()
            : null;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (pearlText != null)
        {
            pearlText.text = "Pearls: " + pearls;
        }

        if (killText != null)
        {
            killText.text = "Kills: " + kills;
        }
    }

    public void SaveFinalResult()
    {
        PlayerPrefs.SetInt("FinalPearls", pearls);
        PlayerPrefs.SetInt("FinalKills", kills);
        PlayerPrefs.SetInt("FinalScore", score);

        // Đánh dấu game đã hoàn thành.
        PlayerPrefs.SetInt("GameCompleted", 1);

        // Xóa checkpoint Continue vì game đã hoàn thành.
        PlayerPrefs.DeleteKey("SavedLevel");
        PlayerPrefs.DeleteKey("HasSaveGame");

        PlayerPrefs.Save();

        Debug.Log(
            "Game completed!" +
            "\nFinal Pearls: " + pearls +
            "\nFinal Kills: " + kills +
            "\nFinal Score: " + score
        );
    }
}