using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string levelMenu;
    [SerializeField] private string mainMenuScene;
    public bool isActive { get; private set; }
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private GameObject optionsMenu;

    public void ActivateCanvas ( bool _isActive )
    {
        pauseCanvas.enabled = _isActive;
        isActive = _isActive;
    }

    public async void ReloadCurrentScene ()
    {
        LevelManager.Instance.LevelHasPlayed();
        screenFader.FadeOut();

        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        await Task.Delay(1000);

        // Reload the current scene
        SceneManager.LoadScene(currentScene.buildIndex);

    }

    public void StartNewGame ()
    {
        screenFader.FadeOut();
        LevelManager.Instance.LoadProgress();
        //LevelManager.Instance.SaveProgress();

        SceneManager.LoadScene(levelMenu);
    }

    public void Options ( bool isActive )
    {
        optionsMenu.SetActive(isActive);
    }

    public void QuitLevel ()
    {
        screenFader.FadeOut();
        SceneManager.LoadScene(levelMenu);
    }

    public void ResetLevelsData ()
    {
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

    public void SwitchControls()
    {

    }

}
