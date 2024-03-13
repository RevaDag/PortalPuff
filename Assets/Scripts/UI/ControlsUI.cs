using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsUI : MonoBehaviour
{
    private DialogManager dialogManager;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction interactAction;

    private bool firstTime = true;

    [Header("Controls")]
    [SerializeField] private GameObject rightControls;
    [SerializeField] private GameObject leftControls;

    [Header("Tutorials")]

    public int tutorialPhase;

    [SerializeField] private Animator movementTutorialAnim;
    private bool isMovementTutorialActive;
    [SerializeField] private bool movementTutorialCompleted;

    [SerializeField] private Animator jumpTutorialAnim;
    private bool isJumpTutorialActive;
    private bool jumpTutorialCompleted;

    [SerializeField] private Animator portalTutorialAnim;
    private bool isPortalTutorialActive;
    private bool portalTutorialCompleted;

    [SerializeField] private Animator doorTutorialAnim;
    private bool isDoorTutorialActive;
    private bool doorTutorialCompleted;

    [SerializeField] private Animator leverTutorialAnim;
    private bool isLeverTutorialActive;
    private bool leverTutorialCompleted;

    private void OnEnable ()
    {
        if (dialogManager != null)
            dialogManager.DialogEnded += DialogEnded;
    }

    private void OnDisable ()
    {
        if (dialogManager != null)
            dialogManager.DialogEnded -= DialogEnded;
    }

    private void Awake ()
    {
        if (LevelManager.Instance != null)
            firstTime = !LevelManager.Instance.GetLevelDataByNumber(LevelManager.Instance.currentLevelNumber).isTutorialShown;

        dialogManager = FindFirstObjectByType<DialogManager>();
        playerInput = FindFirstObjectByType<PlayerInput>();
        var actionMap = playerInput.actions.FindActionMap("Player");



        moveAction = actionMap.FindAction("Move");
        jumpAction = actionMap.FindAction("Jump");
        interactAction = actionMap.FindAction("Interact");

        if (TouchController.Instance != null)
        {
            TouchController.Instance.controlsUI = this;
        }

        jumpAction.performed += OnJumpPerformed;
        interactAction.performed += OnInteractPerformed;
        moveAction.performed += OnMovePerformed;
    }

    private void OnDestroy ()
    {
        moveAction.performed -= OnMovePerformed;
        jumpAction.performed -= OnJumpPerformed;
        interactAction.performed -= OnInteractPerformed;
    }

    public void PauseButton ()
    {
        MainMenu.Instance?.ShowPauseMenu(true);
    }

    private void OnMovePerformed ( InputAction.CallbackContext context )
    {
        if (!firstTime) return;

        if (isMovementTutorialActive)
        {
            movementTutorialAnim?.SetTrigger("SlideDown");
            movementTutorialCompleted = true;
            isMovementTutorialActive = false;
        }


    }

    private void OnJumpPerformed ( InputAction.CallbackContext context )
    {
        if (!firstTime) return;

        switch (tutorialPhase)
        {
            case 1:
                if (isJumpTutorialActive)
                {
                    jumpTutorialAnim?.SetTrigger("SlideDown");
                    jumpTutorialCompleted = true;
                    isJumpTutorialActive = false;
                }
                break;
        }
    }

    private void OnInteractPerformed ( InputAction.CallbackContext context )
    {
        if (!firstTime) return;

        switch (tutorialPhase)
        {
            case 2:
                if (isPortalTutorialActive)
                {
                    portalTutorialAnim?.SetTrigger("SlideDown");
                    portalTutorialCompleted = true;
                    isPortalTutorialActive = false;
                }
                break;

            case 3:
                if (isDoorTutorialActive)
                {
                    doorTutorialAnim?.SetTrigger("SlideDown");
                    doorTutorialCompleted = true;
                    isDoorTutorialActive = false;
                }
                break;

            case 4:
                if (isLeverTutorialActive)
                {
                    leverTutorialAnim?.SetTrigger("SlideDown");
                    leverTutorialCompleted = true;
                    isLeverTutorialActive = false;
                }
                break;
        }
    }



    private bool AreAllTutorialsInactive ()
    {
        return !isMovementTutorialActive &&
                !isJumpTutorialActive &&
                !isPortalTutorialActive &&
                !isDoorTutorialActive &&
                !isLeverTutorialActive;

    }

    private void DialogEnded ( object sender, EventArgs e )
    {
        UpdateTutorialPhase(0);
    }

    public void ActivateCanvas ( bool _isActive )
    {
        MainMenu.Instance?.ShowPauseMenu(_isActive);
    }

    public void ShowPlayerControls ( bool _isLeftTouch )
    {
        if (rightControls != null)
            rightControls.SetActive(_isLeftTouch);

        if (leftControls != null)
            leftControls.SetActive(!_isLeftTouch);
    }

    public async void UpdateTutorialPhase ( int _tutorialPhase )
    {
        if (!firstTime) return;
        if (!AreAllTutorialsInactive()) return;

        //if (movementTutorialCompleted)
        tutorialPhase = _tutorialPhase;

        switch (tutorialPhase)
        {
            case 0:
                if (!movementTutorialCompleted)
                {
                    movementTutorialAnim?.SetTrigger("SlideUp");
                    await Task.Delay(3000);
                    isMovementTutorialActive = true;
                }
                break;

            case 1:
                if (!jumpTutorialCompleted)
                {
                    if (!movementTutorialCompleted)
                    {
                        movementTutorialAnim?.SetTrigger("SlideDown");
                        isMovementTutorialActive = false;
                        movementTutorialCompleted = true;
                    }
                    jumpTutorialAnim?.SetTrigger("SlideUp");
                    isJumpTutorialActive = true;
                }
                break;

            case 2:
                if (!portalTutorialCompleted)
                {
                    portalTutorialAnim?.SetTrigger("SlideUp");
                    isPortalTutorialActive = true;
                }
                break;

            case 3:
                if (!doorTutorialCompleted)
                {
                    doorTutorialAnim?.SetTrigger("SlideUp");
                    isDoorTutorialActive = true;
                }
                break;

            case 4:
                if (!leverTutorialCompleted)
                {
                    leverTutorialAnim?.SetTrigger("SlideUp");
                    isLeverTutorialActive = true;
                }
                break;
        }
    }


}
