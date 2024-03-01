using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;

    public float MusicVolume { get; private set; } = 0.5f; // Default value
    public bool JoystickEnabled { get; private set; } = false; // Default value

    [SerializeField] private AudioSource _backgroundMusic;

    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }


    public void SaveSettings ( float musicVolume, bool controlsEnabled )
    {
        MusicVolume = musicVolume;
        JoystickEnabled = controlsEnabled;

        _backgroundMusic.volume = musicVolume;

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("ControlsEnabled", controlsEnabled ? 1 : 0);

        PlayerPrefs.Save();
    }

    private void LoadSettings ()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        JoystickEnabled = PlayerPrefs.GetInt("ControlsEnabled", 1) == 1;

        _backgroundMusic.volume = MusicVolume;


    }


}
