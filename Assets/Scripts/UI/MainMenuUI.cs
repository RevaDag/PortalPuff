using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private OptionsMenu optionsMenu;


    public void StartButton ()
    {
        MainMenu.Instance?.StartNewGame();
    }


    public void Options ( bool isActive )
    {
        AudioManager.Instance?.PlaySFX("Click");

        optionsMenu.gameObject.SetActive(isActive);
    }


    public void QuitButton ()
    {
        MainMenu.Instance?.ExitGame();
    }

}
