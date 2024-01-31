using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerTouchMovement : MonoBehaviour
{
    private PlayerTouchMovement instance;

    [SerializeField]
    private Vector2 JoystickSize = new Vector2(300, 300);
    [SerializeField]
    private FloatingJoystick Joystick;

    [SerializeField] private PauseMenu pauseMenu;

    private Vector2 startPos;

    private Finger MovementFinger;

    private void OnEnable ()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable ()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void Start ()
    {
        startPos = Joystick.RectTransform.anchoredPosition;
    }

    private void HandleFingerMove ( Finger MovedFinger )
    {
        if (MovedFinger == MovementFinger)
        {
            Vector2 knobPosition;
            float maxMovement = JoystickSize.x / 2f;
            ETouch.Touch currentTouch = MovedFinger.currentTouch;

            if (Vector2.Distance(
                    currentTouch.screenPosition,
                    Joystick.RectTransform.anchoredPosition
                ) > maxMovement)
            {
                knobPosition = (
                    currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition
                    ).normalized
                    * maxMovement;
            }
            else
            {
                knobPosition = currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;
            }

            Joystick.Knob.anchoredPosition = knobPosition;
        }
    }

    private void HandleLoseFinger ( Finger LostFinger )
    {
        if (LostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            Joystick.RectTransform.anchoredPosition = startPos;
        }
    }

    private void HandleFingerDown ( Finger TouchedFinger )
    {
        if (pauseMenu.isActive) { return; }

        if (MovementFinger == null && TouchedFinger.screenPosition.x <= Screen.width / 2f && TouchedFinger.screenPosition.y <= Screen.height / 2f)
        {
            MovementFinger = TouchedFinger;
            Joystick.RectTransform.sizeDelta = JoystickSize;
            Joystick.RectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);
        }
    }

    private Vector2 ClampStartPosition ( Vector2 StartPosition )
    {
        if (StartPosition.x < JoystickSize.x / 2)
        {
            StartPosition.x = JoystickSize.x / 2;
        }

        if (StartPosition.y < JoystickSize.y / 2)
        {
            StartPosition.y = JoystickSize.y / 2;
        }
        else if (StartPosition.y > Screen.height - JoystickSize.y / 2)
        {
            StartPosition.y = Screen.height - JoystickSize.y / 2;
        }

        return StartPosition;
    }
    /*
        private void OnGUI ()
        {
            GUIStyle labelStyle = new GUIStyle()
            {
                fontSize = 24,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                }
            };
            if (MovementFinger != null)
            {
                GUI.Label(new Rect(10, 35, 500, 20), $"Finger Start Position: {MovementFinger.currentTouch.startScreenPosition}", labelStyle);
                GUI.Label(new Rect(10, 65, 500, 20), $"Finger Current Position: {MovementFinger.currentTouch.screenPosition}", labelStyle);
            }
            else
            {
                GUI.Label(new Rect(10, 35, 500, 20), "No Current Movement Touch", labelStyle);
            }

            GUI.Label(new Rect(10, 10, 500, 20), $"Screen Size ({Screen.width}, {Screen.height})", labelStyle);
        }*/
}