using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        audioSource =
            GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogWarning(
                "MusicManager chưa có AudioSource!"
            );

            return;
        }

        audioSource.loop = true;

        ApplySettings();

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void ApplySettings()
    {
        if (audioSource == null)
            return;

        bool musicEnabled =
            PlayerPrefs.GetInt("Music", 1) == 1;

        // Chỉ tắt riêng AudioSource của nhạc
        audioSource.mute = !musicEnabled;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}