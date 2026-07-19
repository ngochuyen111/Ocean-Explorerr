using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        float volume =
            PlayerPrefs.GetFloat("Volume", 1f);

        bool music =
            PlayerPrefs.GetInt("Music", 1) == 1;

        bool sfx =
            PlayerPrefs.GetInt("SFX", 1) == 1;

        if (volumeSlider != null)
            volumeSlider.value = volume;

        if (musicToggle != null)
            musicToggle.isOn = music;

        if (sfxToggle != null)
            sfxToggle.isOn = sfx;

        // Slider điều chỉnh âm lượng tổng
        AudioListener.volume = volume;

        if (MusicManager.Instance != null)
            MusicManager.Instance.ApplySettings();

        if (SFXManager.Instance != null)
            SFXManager.Instance.ApplySettings();
    }

    public void ChangeVolume()
    {
        if (volumeSlider == null)
            return;

        float volume = volumeSlider.value;

        PlayerPrefs.SetFloat(
            "Volume",
            volume
        );

        PlayerPrefs.Save();

        // Điều chỉnh âm lượng tổng của game
        AudioListener.volume = volume;
    }

    public void ToggleMusic()
    {
        if (musicToggle == null)
            return;

        PlayerPrefs.SetInt(
            "Music",
            musicToggle.isOn ? 1 : 0
        );

        PlayerPrefs.Save();

        if (MusicManager.Instance != null)
            MusicManager.Instance.ApplySettings();
    }

    public void ToggleSFX()
    {
        if (sfxToggle == null)
            return;

        PlayerPrefs.SetInt(
            "SFX",
            sfxToggle.isOn ? 1 : 0
        );

        PlayerPrefs.Save();

        if (SFXManager.Instance != null)
            SFXManager.Instance.ApplySettings();
    }
}