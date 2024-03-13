using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider musicVolumeSlider;
    //public Toggle controlsToggle;

    private void Start ()
    {
        musicVolumeSlider.value = OptionsManager.Instance.MusicVolume;
        //controlsToggle.isOn = OptionsManager.Instance.JoystickEnabled;
    }


    public void OnMusicVolumeChanged ()
    {
        OptionsManager.Instance.SaveSettings(musicVolumeSlider.value);
    }

    public void OnControlsEnabledChanged ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        OptionsManager.Instance.SaveSettings(OptionsManager.Instance.MusicVolume);
    }

    public void ResetGame ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        LevelManager.Instance.ResetLevelsData();
    }
}
