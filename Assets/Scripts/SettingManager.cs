using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    void Start()
    {
        float volume =
            PlayerPrefs.GetFloat("Volume", 1);

        bool music =
            PlayerPrefs.GetInt("Music", 1) == 1;

        bool sfx =
            PlayerPrefs.GetInt("SFX", 1) == 1;

        volumeSlider.value = volume;
        musicToggle.isOn = music;
        sfxToggle.isOn = sfx;

        AudioListener.volume =
            music ? volume : 0;
    }

    public void ChangeVolume()
    {
        PlayerPrefs.SetFloat(
            "Volume",
            volumeSlider.value
        );

        if (musicToggle.isOn)
            AudioListener.volume =
                volumeSlider.value;
    }

    public void ToggleMusic()
    {
        PlayerPrefs.SetInt(
            "Music",
            musicToggle.isOn ? 1 : 0
        );

        AudioListener.volume =
            musicToggle.isOn
            ? volumeSlider.value
            : 0;
    }

    public void ToggleSFX()
    {
        PlayerPrefs.SetInt(
            "SFX",
            sfxToggle.isOn ? 1 : 0
        );
    }
}