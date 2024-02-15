using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDuplicationsManager : MonoBehaviour
{
    public List<GameObject> players { get; private set; }
    [SerializeField] private PauseMenu pauseMenu;

    private void Awake ()
    {
        players = new List<GameObject>();
    }

    public void CreateNewDuplication ( GameObject _playerToDuplicate )
    {
        if (players.Count >= 3) return;

        GameObject newPlayer = Instantiate(_playerToDuplicate);
        //players.Add(newPlayer);
    }

    public void AddNewPlayer ( GameObject _newPlayer )
    {
        players.Add(_newPlayer);
    }

    public void RemovePlayer ( GameObject _playerToRemove )
    {
        players.Remove(_playerToRemove);

        if (players.Count == 0)
        {
            pauseMenu.ReloadCurrentScene();
        }
    }
}
