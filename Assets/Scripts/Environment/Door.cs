using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarodevController;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour, IInteractable
{
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

    [SerializeField] private TextMeshProUGUI doorNumberTMP;

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

        SetDoorNumber();
    }

    private void SetDoorNumber ()
    {
        if (LevelManager.Instance == null) return;

        int currentLevelNumber = LevelManager.Instance.currentLevelNumber;
        if (gameObject.name == "Start Door")
            doorNumberTMP.text = currentLevelNumber.ToString();
        else
            doorNumberTMP.text = (currentLevelNumber + 1).ToString();
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

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Freeze();
        playerController.ActivateGatherInput(false);

        AudioManager.Instance?.PlaySFX("Success");
        AudioManager.Instance?.StopMusic();

        player.GetComponentInChildren<PlayerAnimator>().Fade(1, 0);
        await Task.Delay(1000);
        Destroy(player);

        CloseDoor();
        await Task.Delay(1000);

        CompleteLevel();
    }

    private async void CloseDoor ()
    {
        if (_anim == null) return;

        _anim.SetTrigger("Close");

        currentDoorState = DoorState.Close;
        await Task.Delay(1000);

        _anim.ResetTrigger("Close");
    }

    private async void OpenDoor ()
    {
        if (_anim == null) return;

        _anim.SetTrigger("Open");

        currentDoorState = DoorState.Open;
        await Task.Delay(1000);

        _anim.ResetTrigger("Open");
    }

    private void CompleteLevel ()
    {
        LevelManager.Instance.CompleteLevel();
        LevelManager.Instance.UnlockNextLevel();
    }
}
