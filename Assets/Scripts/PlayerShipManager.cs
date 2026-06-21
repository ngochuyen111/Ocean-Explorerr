using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShipManager : MonoBehaviour
{
    public Sprite level1Ship;
    public Sprite level2Ship;
    public Sprite level3Ship;
    public Sprite level4Ship;

    void Start()
    {
        SpriteRenderer sr =
            transform.Find("Visual")
            .GetComponent<SpriteRenderer>();

        string scene =
            SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Level1":
                sr.sprite = level1Ship;
                break;

            case "Level2":
                sr.sprite = level2Ship;
                break;

            case "Level3":
                sr.sprite = level3Ship;
                break;

            case "Level4":
                sr.sprite = level4Ship;
                break;
        }
    }
}