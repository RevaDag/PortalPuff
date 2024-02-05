using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }


    public LevelData[] levels;
    public GameObject levelButtonPrefab;

    private void Awake ()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void InitializeLevelMenu ( Transform topRowContainer, Transform bottomRowContainer, int selectedWorld )
    {
        foreach (Transform child in topRowContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in bottomRowContainer)
        {
            Destroy(child.gameObject);
        }

        int levelIndex = 0; 

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].worldNumber == selectedWorld)
            {
                Transform parentContainer = levelIndex < 5 ? topRowContainer : bottomRowContainer;

                GameObject buttonObj = Instantiate(levelButtonPrefab, parentContainer);

                LevelButton button = buttonObj.GetComponent<LevelButton>();

                button.SetLevelData(i + 1, levels[i].isLocked, levels[i].starsEarned, levels[i].sceneName);

                levelIndex++; 

                if (levelIndex >= 10) break;
            }
        }
    }


    public void UpdateLevelData ( int levelIndex, bool isLocked, int starsEarned )
    {
        if (levelIndex < levels.Length)
        {
            levels[levelIndex].isLocked = isLocked;
            levels[levelIndex].starsEarned = starsEarned;
        }
    }

    public void UnlockLevelBySceneName ( string sceneName )
    {
        foreach (LevelData level in levels)
        {
            if (level.sceneName == sceneName)
            {
                level.isLocked = false; 
                break; 
            }
        }
    }
}

[System.Serializable]
public class LevelData
{
    public string sceneName;
    public int worldNumber;
    public bool isLocked = true;
    public int starsEarned = 0;
}