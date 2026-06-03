using UnityEngine;
using UnityEngine.SceneManagement;

public class SeaHazard : MonoBehaviour
{
    public string gameOverSceneName = "GameOver";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneManager.LoadScene(gameOverSceneName);
    }
}


