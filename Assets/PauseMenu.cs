using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isActive { get; private set; }
    [SerializeField] private Canvas pauseCanvas;

    public void ActivateCanvas ( bool _isActive )
    {
        pauseCanvas.enabled = _isActive;
        isActive = _isActive;
    }

    public void ReloadCurrentScene ()
    {
        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Reload the current scene
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitLevel ()
    {
        Debug.Log("Quit Level");
    }
}
