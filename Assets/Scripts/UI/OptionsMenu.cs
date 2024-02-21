using UnityEngine;
using UnityEngine.UI; // Don't forget to include UI namespace

public class OptionsMenu : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Toggle controlsToggle;

    private void Start ()
    {
        // Initialize UI elements with saved settings
        musicVolumeSlider.value = OptionsManager.Instance.MusicVolume;
        controlsToggle.isOn = OptionsManager.Instance.JoystickEnabled;
    }

    public void OnMusicVolumeChanged ()
    {
        OptionsManager.Instance.SaveSettings(musicVolumeSlider.value, OptionsManager.Instance.JoystickEnabled);
    }

    public void OnControlsEnabledChanged ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        OptionsManager.Instance.SaveSettings(OptionsManager.Instance.MusicVolume, controlsToggle.isOn);
    }

    public void ResetGame ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        LevelManager.Instance.ResetLevelsData();
    }
}
