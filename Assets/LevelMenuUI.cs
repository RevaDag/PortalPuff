using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuUI : MonoBehaviour
{
    [SerializeField] private Transform topRow;
    [SerializeField] private Transform bottomRow;
    [SerializeField] private int selectedWorld;

    [SerializeField] private ScreenFader screenFader;

    private void Start ()
    {
        LevelManager.Instance.InitializeLevelMenu(topRow, bottomRow, selectedWorld);
    }

    public void BackToMainMenu ()
    {
        screenFader.FadeOut();
        SceneManager.LoadScene("Main Menu");
    }

}
