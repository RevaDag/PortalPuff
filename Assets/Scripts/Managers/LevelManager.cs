using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private WorldDefaultState worldsDefaultState;

    public List<WorldData> _worlds;

    public GameObject levelButtonPrefab;

    public int currentLevelIndex = -1;
    public int currentWorldIndex = 0;
    private int currentLevelStarsCollected;

    public LevelSummary levelSummary;


    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void InitializeLevelMenu ( Transform topRowContainer, Transform bottomRowContainer, int selectedWorld )
    {
        if (selectedWorld < 0 || selectedWorld >= _worlds.Count || !_worlds[selectedWorld - 1].levels.Any())
        {
            Debug.LogWarning("Selected world is out of range or contains no levels.");
            return;
        }


        currentWorldIndex = selectedWorld - 1;

        foreach (Transform child in topRowContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in bottomRowContainer)
        {
            Destroy(child.gameObject);
        }

        int levelIndex = 0;

        for (int i = 0; i < _worlds[currentWorldIndex].levels.Count; i++)
        {
            if (_worlds[currentWorldIndex].levels[i].worldNumber == selectedWorld)
            {
                Transform parentContainer = levelIndex < 5 ? topRowContainer : bottomRowContainer;

                GameObject buttonObj = Instantiate(levelButtonPrefab, parentContainer);

                LevelButton button = buttonObj.GetComponent<LevelButton>();

                button.SetLevelData(currentWorldIndex, i + 1, _worlds[currentWorldIndex].levels[i].isLocked, _worlds[currentWorldIndex].levels[i].starsEarned, _worlds[currentWorldIndex].levels[i].sceneName);

                levelIndex++;

                if (levelIndex >= 10) break;
            }
        }
    }

    public void UpdateLevelSummary ( string nextLevelName, int _currentLevelIndex )
    {
        if (levelSummary == null) return;

        string levelText = "NO LEVEL";

        if (currentWorldIndex > 0)
            levelText = $"LEVEL {currentWorldIndex}{_currentLevelIndex + 1}";
        else
            levelText = $"LEVEL {_currentLevelIndex + 1}";

        levelSummary.UpdateLevelText(levelText);
        levelSummary.SetStars(currentLevelStarsCollected);

        levelSummary.SetNextLevel(nextLevelName);


        levelSummary.ActivateCanvas(true);
    }

    public void UnlockLevelBySceneName ( string sceneName )
    {
        foreach (LevelData level in _worlds[currentWorldIndex].levels)
        {
            if (level.sceneName == sceneName)
            {
                level.isLocked = false;
                break;
            }
        }

        SaveProgress();
    }

    public void LoadProgress ()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        Debug.Log($"Attempting to load progress from {path}");

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            SaveDataCollection collection = JsonUtility.FromJson<SaveDataCollection>(json);

            // Log loaded JSON for inspection
            Debug.Log($"Loaded JSON: {json}");


            _worlds = collection.worlds;
            // Confirm worlds are loaded
            Debug.Log($"Loaded {_worlds.Count} worlds.");

        }
        else
        {
            Debug.LogWarning("Save file not found, loading default data.");
            // Handle case where no save file exists
        }
    }

    public void SaveProgress ()
    {
        SaveDataCollection collection = new SaveDataCollection
        {
            worlds = _worlds,
        };

        string json = JsonUtility.ToJson(collection);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

        // Log for debugging
        Debug.Log($"Saved data to {Application.persistentDataPath}/savefile.json");
        Debug.Log($"Saved JSON: {json}");
    }

    public void DeleteSaveFile ()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            ResetLevelsData();

            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }

    public void ResetLevelsData ()
    {
        _worlds = worldsDefaultState.worlds;

        SaveProgress();
        Debug.Log("Levels have been reset.");
    }

    public void SetWorldsDefaultState ( WorldDefaultState _defaultState )
    {
        worldsDefaultState = _defaultState;
    }


    public void CollectStar ()
    {
        currentLevelStarsCollected++;
    }

    public void CompleteLevel ( string nextLevelName )
    {
        if (currentLevelStarsCollected > _worlds[currentWorldIndex].levels[currentLevelIndex].starsEarned)
            _worlds[currentWorldIndex].levels[currentLevelIndex].starsEarned = currentLevelStarsCollected;

        UpdateLevelSummary(nextLevelName, currentLevelIndex);

        switch (currentLevelIndex)
        {
            case 9:
                UnlockWorld(1);
                currentWorldIndex = 1;
                break;

            case 19:
                UnlockWorld(2);
                currentWorldIndex = 2;
                break;
        }


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
        _worlds[worldToUnlock].isLocked = false;
    }
}



[System.Serializable]
public class SaveDataCollection
{
    public List<WorldData> worlds;
}

[System.Serializable]
public class WorldData
{
    public int worldNumber;
    public List<LevelData> levels;
    public bool isLocked = true;

}

[System.Serializable]
public class LevelData
{
    public string sceneName;
    public int worldNumber;
    public bool isLocked = true;
    public int starsEarned;
}
