using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField] private string levelMenu;
    [SerializeField] private string mainMenuScene;
    public bool isActive { get; private set; } = true;
    private Canvas pauseCanvas;

    [SerializeField] private GameObject resumeButton;

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

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowPauseMenu(!isActive);
    }


    public void ShowPauseMenu ( bool _isActive )
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == levelMenu || activeSceneName == mainMenuScene)
            return;


        AudioManager.Instance?.PlaySFX("Click");

        if (_isActive)
        {
            AudioManager.Instance?.StopMusic();
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
        else
            AudioManager.Instance?.PlayMusic("TrickyFox");

        PlayersManager.Instance?.ActivateInputs(!_isActive);
        TouchController.Instance?.ResetTouchControls();
        pauseCanvas.enabled = _isActive;
        isActive = _isActive;
    }

    public async void ReloadCurrentScene ()
    {
        AudioManager.Instance?.PlaySFX("Click");

        ShowPauseMenu(false);

        ScreenFader.Instance?.FadeOut();

        Scene currentScene = SceneManager.GetActiveScene();

        await Task.Delay(1000);

        SceneManager.LoadScene(currentScene.buildIndex);

    }

    public void StartNewGame ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        ScreenFader.Instance?.FadeOut();
        LevelManager.Instance.LoadProgress();
        isActive = false;

        SceneManager.LoadScene(levelMenu);
    }

    public void QuitLevel ()
    {
        AudioManager.Instance?.PlaySFX("Click");

        ShowPauseMenu(false);
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
