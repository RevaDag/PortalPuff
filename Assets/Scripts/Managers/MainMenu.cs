using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarodevController;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField] private string levelMenu;
    [SerializeField] private string mainMenuScene;
    public bool isActive { get; private set; } = true;
    private Canvas pauseCanvas;

    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start ()
    {
        pauseCanvas = GetComponent<Canvas>();
    }

    public void ActivateCanvas ( bool _isActive )
    {
        AudioManager.Instance?.PlaySFX("Click");

        if (_isActive)
            AudioManager.Instance?.StopMusic();
        else
            AudioManager.Instance?.PlayMusic("TrickyFox");

        PlayersManager.Instance?.ActivateInputs(!_isActive);
        pauseCanvas.enabled = _isActive;
        isActive = _isActive;
    }

    public async void ReloadCurrentScene ()
    {
        AudioManager.Instance?.PlaySFX("Click");

        ActivateCanvas(false);

        // LevelManager.Instance?.LevelHasPlayed();
        ScreenFader.Instance?.FadeOut();

        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        await Task.Delay(1000);

        // Reload the current scene
        SceneManager.LoadScene(currentScene.buildIndex);

    }

    public void StartNewGame ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        ScreenFader.Instance?.FadeOut();
        LevelManager.Instance.LoadProgress();
        //LevelManager.Instance.SaveProgress();

        SceneManager.LoadScene(levelMenu);
    }

    public void QuitLevel ()
    {
        AudioManager.Instance?.PlaySFX("Click");

        ActivateCanvas(false);
        TouchController.Instance?.ActivateTouch(false);
        ScreenFader.Instance?.FadeOut();
        SceneManager.LoadScene(levelMenu);
        AudioManager.Instance?.PlayMusic("TrickyFox");

    }

    public void ResetLevelsData ()
    {
        AudioManager.Instance?.PlaySFX("Click");

        LevelManager.Instance.ResetLevelsData();
    }

    public void ExitGame ()
    {
#if UNITY_STANDALONE
    // Quit the application
    Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID || UNITY_IOS
        Application.Quit();
#endif
    }


}
