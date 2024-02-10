using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }


    public LevelData[] levels;
    public WorldData[] worlds;

    public GameObject levelButtonPrefab;

    public int currentLevelIndex = -1;
    private int currentLevelStarsCollected;

    public LevelSummary levelSummary;


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


    public void UpdateLevelData ( int levelIndex, bool isLocked, int starsEarned )
    {
        if (levelIndex < levels.Length)
        {
            levels[levelIndex].isLocked = isLocked;
            levels[levelIndex].starsEarned = starsEarned;
        }
    }

    public void UpdateLevelSummary ()
    {
        if (levelSummary == null) return;

        string levelText = "LEVEL " + (currentLevelIndex + 1);
        levelSummary.UpdateLevelText(levelText);
        levelSummary.SetStars(currentLevelStarsCollected);
        levelSummary.SetNextLevel(levels[currentLevelIndex + 1].sceneName);
        levelSummary.ActivateCanvas(true);
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

    public void SaveProgress ()
    {
        SaveDataCollection collection = new SaveDataCollection
        {
            levels = levels,
            worlds = worlds
        };

        string json = JsonUtility.ToJson(collection);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }


    public void LoadProgress ()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            SaveDataCollection collection = JsonUtility.FromJson<SaveDataCollection>(json);
            levels = collection.levels;
            worlds = collection.worlds;
        }
    }

    public void DeleteSaveFile ()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            LoadProgress();
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }

    public void ResetLevelsData ()
    {
        if (levels == null || levels.Length == 0) return; // Check if there are any levels to reset

        // Loop through all levels
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].isLocked = true; // Lock each level
            levels[i].starsEarned = 0; // Reset stars earned to 0
        }

        // Unlock the first level
        if (levels.Length > 0)
        {
            levels[0].isLocked = false; // Unlock the first level
        }

        // Save the updated progress
        SaveProgress();
        LoadProgress();

        Debug.Log("Levels have been reset. Only the first level is unlocked, and no stars are earned.");
    }



    public void CollectStar ()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length) return; // Sanity check

        // Increase the star count for the current level
        currentLevelStarsCollected++;
    }

    public void CompleteLevel ()
    {
        if (currentLevelStarsCollected > levels[currentLevelIndex].starsEarned)
            levels[currentLevelIndex].starsEarned = currentLevelStarsCollected;

        switch (currentLevelIndex)
        {
            case 9:
                UnlockWorld(1);
                break;

            case 19:
                UnlockWorld(2);
                break;
        }


        UpdateLevelSummary();
        SaveProgress();
    }

    public void NextLevel ()
    {
        currentLevelIndex++;
        ResetStars();
    }

    public void ResetStars ()
    {
        currentLevelStarsCollected = 0;
    }

    public void UnlockWorld ( int worldToUnlock )
    {
        worlds[worldToUnlock].isLocked = false;
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
public class WorldData
{
    public int worldNumber;
    public bool isLocked = true;
}

[System.Serializable]
public class SaveDataCollection
{
    public LevelData[] levels;
    public WorldData[] worlds;
}
