using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarodevController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private ScreenFader screenFader;

    [SerializeField]
    private string sceneToLoad; // Scene name to load, set this in the Inspector

    private bool isInterating;

    public enum DoorState
    {
        Close,
        Open,
        PreviousLevel,
        Locked
    }

    [SerializeField] private DoorState currentDoorState;

    private Animator _anim;

    private void Awake ()
    {
        _anim = GetComponent<Animator>();
    }

    private void Start ()
    {
        switch (currentDoorState)
        {
            case DoorState.Close:
                CloseDoor();
                break;

            case DoorState.Open:
                OpenDoor();
                break;

            case DoorState.PreviousLevel:
                OpenDoor();
                CloseDoor();
                break;


        }
    }

    public void ToggleDoor ()
    {
        if (currentDoorState == DoorState.Open)
        {
            CloseDoor();
            return;
        }
        else if (currentDoorState == DoorState.Close)
        {
            OpenDoor();
            return;
        }
    }

    public async void Interact ( GameObject player )
    {
        if (isInterating) return;
        if (currentDoorState != DoorState.Open) return;

        isInterating = true;

        player.GetComponent<PlayerController>().ActivateGatherInput(false);
        player.GetComponentInChildren<PlayerAnimator>().Fade(1, 0);
        await Task.Delay(1000);

        CloseDoor();
        await Task.Delay(1000);

        LoadScene();
    }

    private async void CloseDoor ()
    {
        _anim.SetTrigger("Close");

        currentDoorState = DoorState.Close;
        await Task.Delay(1000);

        _anim?.ResetTrigger("Close");
    }

    private async void OpenDoor ()
    {
        _anim.SetTrigger("Open");

        currentDoorState = DoorState.Open;
        await Task.Delay(1000);

        _anim?.ResetTrigger("Open");
    }

    private async void LoadScene ()
    {
        screenFader.FadeOut();
        await Task.Delay(1000);


        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            LevelManager.Instance.UnlockLevelBySceneName(sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is not set in the DoorScript.");
        }
    }
}
