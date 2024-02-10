using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI worldTitle;
    [SerializeField] private TextMeshProUGUI worldName;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Sprite[] backgroundImages;

    [SerializeField] private Transform topRow;
    [SerializeField] private Transform bottomRow;
    [SerializeField] private int selectedWorld;

    [SerializeField] private ScreenFader screenFader;

    [SerializeField] private Button previousWorldButton;
    [SerializeField] private Button nextWorldButton;

    private void Start ()
    {
        LevelManager.Instance.InitializeLevelMenu(topRow, bottomRow, selectedWorld);
        UpdateWorldUI();
    }

    public void BackToMainMenu ()
    {
        screenFader.FadeOut();
        SceneManager.LoadScene("Main Menu");
    }

    public void NextWorld ()
    {
        selectedWorld++;
        LevelManager.Instance.InitializeLevelMenu(topRow, bottomRow, selectedWorld);
        UpdateWorldUI();
    }

    public void PreviousWorld ()
    {
        selectedWorld--;
        LevelManager.Instance.InitializeLevelMenu(topRow, bottomRow, selectedWorld);
        UpdateWorldUI();
    }

    public void UpdateWorldUI ()
    {
        backgroundImage.sprite = backgroundImages[selectedWorld - 1];

        switch (selectedWorld)
        {
            case 1:
                worldTitle.text = "WORLD 1";
                worldName.text = "The Laboratory";
                previousWorldButton.gameObject.SetActive(false);
                if (LevelManager.Instance.worlds[1].isLocked)
                    nextWorldButton.interactable = false;
                else
                    nextWorldButton.interactable = true;
                break;

            case 2:
                worldTitle.text = "WORLD 2";
                worldName.text = "The Sewers";

                previousWorldButton.gameObject.SetActive(true);
                previousWorldButton.interactable = true;

                if (LevelManager.Instance.worlds[2].isLocked)
                    nextWorldButton.interactable = false;
                else
                    nextWorldButton.interactable = true;

                nextWorldButton.enabled = true;
                break;

            case 3:
                worldTitle.text = "WORLD 3";
                worldName.text = "The Swamp";

                previousWorldButton.interactable = true;

                nextWorldButton.gameObject.SetActive(false);
                ;
                break;

            default:
                worldTitle.text = "UNKNOWN WORLD";
                worldName.text = "Unknown";
                break;
        }
    }


}
