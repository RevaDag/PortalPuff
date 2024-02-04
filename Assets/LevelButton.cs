using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public GameObject lockedIcon;
    public Image[] stars;
    private string sceneName;

    public void SetLevelData ( int levelNumber, bool isLocked, int starsEarned, string _sceneName )
    {
        levelText.text = levelNumber.ToString();
        lockedIcon.SetActive(isLocked);
        this.sceneName = _sceneName;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = i < starsEarned ? new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, 1f) : new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, 0f);
        }

        Button button = GetComponent<Button>();
        button.onClick.AddListener(LoadLevel);

        if (lockedIcon.activeSelf)
        {
            button.interactable = false;
            levelText.enabled = false;
        }

    }

    public void LoadLevel ()
    {

        Debug.Log("Button clicked, attempting to load scene: " + sceneName);

        // Check if the level is locked
        if (!lockedIcon.activeSelf)
        {
            SceneManager.LoadScene(sceneName); // Load the scene by index. You can also use a scene name string here.
        }
    }
}
