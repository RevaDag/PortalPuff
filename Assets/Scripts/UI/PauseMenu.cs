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
    [SerializeField] private string firstScene;
    [SerializeField] private string mainMenuScene;
    public bool isActive { get; private set; }
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private ScreenFader screenFader;

    public void ActivateCanvas ( bool _isActive )
    {
        pauseCanvas.enabled = _isActive;
        isActive = _isActive;
    }

    public async void ReloadCurrentScene ()
    {
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
        SceneManager.LoadScene(firstScene);
    }

    public void QuitLevel ()
    {
        screenFader.FadeOut();
        SceneManager.LoadScene(mainMenuScene);
    }

    public void ExitGame ()
    {
        // If we are running in a standalone build of the game
#if UNITY_STANDALONE
        // Quit the application
        Application.Quit();
#endif

        // If we are running in the editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
