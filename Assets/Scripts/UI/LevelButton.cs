using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    private Button levelButton;
    public TextMeshProUGUI levelText;
    public GameObject lockedIcon;
    public Image[] stars;
    private string sceneName;
    private int levelIndex = -1;

    [SerializeField] private Button unlockLevelButton;

    private void Awake ()
    {
        levelButton = GetComponent<Button>();
    }

    public void SetLevelData ( int worldIndex, int levelNumber, bool isLocked, int starsEarned, string _sceneName )
    {
        levelIndex = levelNumber - 1;
        //levelText.text = levelNumber.ToString();

        if (worldIndex > 0)
            levelText.text = $"{worldIndex}{levelNumber}";
        else
            levelText.text = $"{levelNumber}";


        lockedIcon.SetActive(isLocked);
        this.sceneName = _sceneName;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = i < starsEarned ? new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, 1f) : new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, 0f);
        }

        levelButton.onClick.AddListener(LoadLevel);

        if (lockedIcon.activeSelf)
        {
            levelButton.interactable = false;
            levelText.enabled = false;
            unlockLevelButton.gameObject.SetActive(true);
        }

    }

    public void LoadLevel ()
    {
        if (!lockedIcon.activeSelf)
        {
            LevelManager.Instance.currentLevelIndex = levelIndex;
            LevelManager.Instance.UpdateNextLevelName();
            SceneManager.LoadScene(sceneName); // Load the scene by index. You can also use a scene name string here.
        }
    }

    public void UnlockLevel ()
    {
        LevelManager.Instance.UnlockLevelBySceneName(sceneName);

        levelButton.interactable = true;
        levelText.enabled = true;
        lockedIcon.gameObject.SetActive(false);

        unlockLevelButton.gameObject.SetActive(false);
    }
}
