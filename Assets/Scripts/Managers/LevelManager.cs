using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }


    public LevelData[] levels;
    public GameObject levelButtonPrefab;

    public int currentLevelIndex = -1; // Tracks the current level index. Set this when a level starts.
    private int currentLevelStarsCollected;
    private int totalStarsInLevel = 3;


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

    private void Start ()
    {
        LoadProgress();
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

    public void InitializeLevelStars ( int levelIndex )
    {
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            currentLevelIndex = levelIndex;
            currentLevelStarsCollected = 0;
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

        SaveProgress();
    }

    public void SaveProgress ()
    {
        LevelDataCollection collection = new LevelDataCollection { levels = levels };
        string json = JsonUtility.ToJson(collection);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }


    public void LoadProgress ()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            LevelDataCollection collection = JsonUtility.FromJson<LevelDataCollection>(json);
            levels = collection.levels;
        }
    }

    public void DeleteSaveFile ()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }


    public void CollectStar ()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length) return; // Sanity check

        // Increase the star count for the current level
        currentLevelStarsCollected++;
    }

    public void CompleteLevel ()
    {
        levels[currentLevelIndex].starsEarned = Mathf.Clamp(levels[currentLevelIndex].starsEarned + 1, 0, totalStarsInLevel);
        currentLevelStarsCollected = 0;
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

[System.Serializable]
public class LevelDataCollection
{
    public LevelData[] levels;
}
