using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    private enum Tutorial
    {
        jump,
        portal,
        door
    }

    [SerializeField] private Tutorial tutorial;
    [SerializeField] private ControlsUI controlsUI;


    private void OnTriggerEnter2D ( Collider2D other )
    {
        if (other.CompareTag("Player"))
        {
            switch (tutorial)
            {
                case Tutorial.jump:
                    OnJumpColliderEntered();
                    break;
                case Tutorial.portal:
                    OnPortalInteractColliderEntered();
                    break;
                case Tutorial.door:
                    OnDoorInteractColliderEntered();
                    break;

            }
        }
    }

    private void OnJumpColliderEntered ()
    {
        controlsUI?.SlideUp(1);
    }

    private void OnPortalInteractColliderEntered ()
    {
        controlsUI?.SlideUp(2);
    }

    private void OnDoorInteractColliderEntered ()
    {
        controlsUI?.SlideUp(3);

    }
}
