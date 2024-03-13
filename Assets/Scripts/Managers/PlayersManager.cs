using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance;

    public List<GameObject> players { get; private set; }


    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


        players = new List<GameObject>();
    }

    void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start ()
    {
    }

    void OnSceneLoaded ( Scene scene, LoadSceneMode mode )
    {
        players.Clear();
        players = new List<GameObject>();
    }

    public void CreateNewDuplication ( GameObject _playerToDuplicate )
    {

        if (players.Count >= 3) return;

        GameObject newPlayer = Instantiate(_playerToDuplicate);
    }

    public void AddNewPlayer ( GameObject _newPlayer )
    {
        players.Add(_newPlayer);
        _newPlayer.GetComponent<PlayerController>().ActivateGatherInput(true);
    }

    public void RemovePlayer ( GameObject _playerToRemove )
    {
        players.Remove(_playerToRemove);

        if (players.Count == 0)
        {
            MainMenu.Instance?.ReloadCurrentScene();
        }
    }

    public void ActivateInputs ( bool _isActive )
    {
        foreach (GameObject player in players)
        {
            if (player != null)
                player.GetComponent<PlayerController>().ActivateGatherInput(_isActive);

        }
    }
}
