using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject musicButton;


    private void Start ()
    {
        SelectButton(startButton);
    }

    private void SelectButton ( GameObject gameObjectToSelect )
    {
        EventSystem.current.SetSelectedGameObject(gameObjectToSelect);
    }

    public void StartButton ()
    {
        MainMenu.Instance?.StartNewGame();
    }


    public void Options ( bool isActive )
    {
        AudioManager.Instance?.PlaySFX("Click");
        mainMenu.SetActive(!isActive);
        optionsMenu.SetActive(isActive);

        if (isActive)
        {
            SelectButton(musicButton);
        }
        else
            SelectButton(startButton);

    }


    public void QuitButton ()
    {
        MainMenu.Instance?.ExitGame();
    }

}
