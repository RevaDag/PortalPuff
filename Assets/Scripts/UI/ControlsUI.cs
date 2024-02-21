using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlsUI : MonoBehaviour
{
    public DialogManager dialogManager;

    private bool firstTime = true;
    private bool isJoystickActive = true;
    private GameObject leftJoystick;
    [SerializeField] private GameObject interactButton;
    [SerializeField] private GameObject jumpButton;

    [Header("Tutorials")]
    [SerializeField] private Animator movementTutorialAnim;
    private bool isMovementTutorialActive;
    private bool movementTutorialCompleted;

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
        leftJoystick = GameObject.Find("Left Joystick Canvas");
    }

    private void Start ()
    {
        if (OptionsManager.Instance != null)
            EnableJoystick(OptionsManager.Instance.JoystickEnabled);
        else
            EnableJoystick(false);

        if (LevelManager.Instance != null)
            if (LevelManager.Instance.GetLevelDataByNumber(LevelManager.Instance.currentLevelNumber).firstTime == false)
            {
                firstTime = false;
            }
    }

    private void FixedUpdate ()
    {
        if (isJoystickActive || !firstTime) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && isMovementTutorialActive)
        {
            SlideDown(0);
        }

        if (TouchController.Instance.IsUpSwiping && isJumpTutorialActive)
        {
            SlideDown(1);
        }

        if (TouchController.Instance.IsDownSwiping)
        {
            if (isPortalTutorialActive)
            {
                SlideDown(2);
            }

            if (isDoorTutorialActive)
            {
                SlideDown(3);
            }

            if (isLeverTutorialActive)
            {
                SlideDown(4);
            }
        }
    }

    private void SlideDown ( int tutorialPhase )
    {
        if (isJoystickActive || !firstTime) return;

        switch (tutorialPhase)
        {
            case 0:
                if (!movementTutorialCompleted)
                {
                    movementTutorialAnim?.SetTrigger("SlideDown");
                    movementTutorialCompleted = true;
                    isMovementTutorialActive = false;
                }
                break;

            case 1:
                if (!jumpTutorialCompleted)
                {
                    jumpTutorialAnim?.SetTrigger("SlideDown");
                    jumpTutorialCompleted = true;
                    isJumpTutorialActive = false;
                }
                break;

            case 2:
                if (!portalTutorialCompleted)
                {
                    portalTutorialAnim?.SetTrigger("SlideDown");
                    portalTutorialCompleted = true;
                    isPortalTutorialActive = false;
                }
                break;

            case 3:
                if (!doorTutorialCompleted)
                {
                    doorTutorialAnim?.SetTrigger("SlideDown");
                    doorTutorialCompleted = true;
                    isDoorTutorialActive = false;
                }
                break;

            case 4:
                if (!leverTutorialCompleted)
                {
                    leverTutorialAnim?.SetTrigger("SlideDown");
                    leverTutorialCompleted = true;
                    isLeverTutorialActive = false;
                }
                break;
        }
    }

    public void SlideUp ( int tutorialPhase )
    {
        if (isJoystickActive || !firstTime) return;
        if (!AreAllTutorialsInactive()) return;

        switch (tutorialPhase)
        {
            case 0:
                if (!movementTutorialCompleted)
                {
                    movementTutorialAnim?.SetTrigger("SlideUp");
                    isMovementTutorialActive = true;
                }
                break;

            case 1:
                if (!jumpTutorialCompleted)
                {
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
        if (!movementTutorialCompleted && !isJoystickActive)
            SlideUp(0);
    }

    public void ActivateCanvas ( bool _isActive )
    {
        PauseMenu.Instance?.ActivateCanvas( _isActive );
    }

    private void EnableJoystick ( bool isActive )
    {
        isJoystickActive = isActive;
        leftJoystick?.SetActive(isActive);
        interactButton?.SetActive(isActive);
        jumpButton?.SetActive(isActive);
        TouchController.Instance.gameObject.SetActive(!isActive);
    }

}
