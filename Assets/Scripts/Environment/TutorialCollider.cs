using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    private enum Tutorial
    {
        jump,
        portal,
        door,
        lever
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
                    controlsUI?.SlideUp(1);
                    break;
                case Tutorial.portal:
                    controlsUI?.SlideUp(2);
                    break;
                case Tutorial.door:
                    controlsUI?.SlideUp(3);
                    break;
                case Tutorial.lever:
                    controlsUI?.SlideUp(4);
                    break;
            }
        }
    }

}
