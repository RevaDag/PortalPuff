using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TarodevController;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    public void Interact ( GameObject player )
    {
        if (isInterating) return;
        if (currentDoorState != DoorState.Open) return;

        isInterating = true;

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Freeze();
        playerController.ActivateGatherInput(false);

        AudioManager.Instance?.PlaySFX("Success");
        AudioManager.Instance?.StopMusic();

        player.GetComponentInChildren<PlayerAnimator>().Fade(1, 0, () =>
        {
            Destroy(player);

            CloseDoor();
            CompleteLevel();
        });

    }

    private void CloseDoor ()
    {
        if (_anim == null) return;

        _anim.SetTrigger("Close");

        StartCoroutine(WaitForAnimation(_anim, "Close"));
    }

    private void OpenDoor ()
    {
        if (_anim == null) return;

        _anim.SetTrigger("Open");

        StartCoroutine(WaitForAnimation(_anim, "Open"));
    }

    private void CompleteLevel ()
    {
        LevelManager.Instance.CompleteLevel();
        LevelManager.Instance.UnlockNextLevel();
    }

    private IEnumerator WaitForAnimation ( Animator animator, string stateName )
    {
        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) ||
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        switch (stateName)
        {
            case "Open":
                currentDoorState = DoorState.Open;
                break;
            case "Close":
                currentDoorState = DoorState.Close;
                break;
        }

        _anim.ResetTrigger(stateName);
    }

}
