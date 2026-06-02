using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public string winSceneName = "Win";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int current =
                SceneManager.GetActiveScene().buildIndex;

            int unlocked =
                PlayerPrefs.GetInt("UnlockedLevel", 1);

            if (current >= unlocked)
            {
                PlayerPrefs.SetInt(
                    "UnlockedLevel",
                    current
                );
            }

            SceneManager.LoadScene(winSceneName);
        }
    }
}