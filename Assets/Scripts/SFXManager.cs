using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Player SFX")]
    public AudioClip shootClip;
    public AudioClip playerHitClip;

    [Header("Enemy SFX")]
    public AudioClip bulletHitClip;
    public AudioClip inkShootClip;
    public AudioClip chargeClip;
    public AudioClip enemyDeathClip;

    [Header("Item SFX")]
    public AudioClip pearlPickupClip;

    private bool sfxEnabled = true;

    private void Awake()
    {
        // Nếu đã có một SFXManager tồn tại thì xóa bản trùng
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Giữ SFXManager khi chuyển scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        sfxEnabled =
            PlayerPrefs.GetInt("SFX", 1) == 1;
    }

    private void PlayClip(AudioClip clip)
    {
        if (!sfxEnabled)
            return;

        if (audioSource == null)
            return;

        if (clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    public void PlayShoot()
    {
        PlayClip(shootClip);
    }

    public void PlayPlayerHit()
    {
        PlayClip(playerHitClip);
    }

    public void PlayBulletHit()
    {
        PlayClip(bulletHitClip);
    }

    public void PlayInkShoot()
    {
        PlayClip(inkShootClip);
    }

    public void PlayCharge()
    {
        PlayClip(chargeClip);
    }

    public void PlayEnemyDeath()
    {
        PlayClip(enemyDeathClip);
    }

    public void PlayPearlPickup()
    {
        PlayClip(pearlPickupClip);
    }
}