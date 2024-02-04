using UnityEngine;
using UnityEngine.UI; // Make sure to include this for UI elements

public class LevelManager : MonoBehaviour
{
    public LevelData[] levels; // Array to store data for each level
    public GameObject levelButtonPrefab; // Prefab for level buttons
    public Transform levelsContainer; // Parent object for level buttons

    private void Start ()
    {
        InitializeLevelMenu();
    }

    void InitializeLevelMenu ()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            // Instantiate a new level button for each level
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelsContainer);
            LevelButton button = buttonObj.GetComponent<LevelButton>(); // Assuming you have a LevelButton script attached to your prefab

            // Set the level number, locked state, and stars earned
            button.SetLevelData(i + 1, levels[i].isLocked, levels[i].starsEarned, levels[i].sceneName);
        }
    }

    // Call this method to unlock levels or update stars, then call InitializeLevelMenu() again to refresh the UI
    public void UpdateLevelData ( int levelIndex, bool isLocked, int starsEarned )
    {
        if (levelIndex < levels.Length)
        {
            levels[levelIndex].isLocked = isLocked;
            levels[levelIndex].starsEarned = starsEarned;
        }
    }
}

[System.Serializable]
public class LevelData
{
    public string sceneName;
    public bool isLocked = true;
    public int starsEarned = 0;
}