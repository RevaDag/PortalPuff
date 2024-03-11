using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class TouchController : MonoBehaviour
{
    public static TouchController Instance { get; private set; }

    [SerializeField] private LayerMask controlsLayerMask;

    public bool IsHoldingLeft { get; private set; }
    public bool IsHoldingRight { get; private set; }
    public Vector2 TouchMove { get; private set; }

    private GameObject touchIndicator;
    private Vector3 indicatorStartPos;

    [Header("Joystick")]
    [SerializeField] private Transform joystick;
    private Vector2 joystickStartPos;
    [SerializeField]
    private float movementThreshold = 30f;


    public ControlsUI controlsUI;

    public event Action OnTouchDirectionDetermined;


    private void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded ( Scene scene, LoadSceneMode mode )
    {
        ResetTouchControls();
    }

    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start ()
    {
        touchIndicator = transform.GetChild(0).gameObject;

        if (touchIndicator != null)
            indicatorStartPos = touchIndicator.transform.position;

    }


    void Update ()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (!IsTouchOverControls(touch))
            {
                if (touch.position.x < Screen.width / 2)
                    HandleTouch(touch);
                else
                    ResetTouchControls();
            }
        }
    }

    private bool IsTouchOverControls ( Touch touch )
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touch.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (var result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("Controls"))
                return true;
        }
        return false;
    }


    private void HandleTouch ( Touch touch )
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                SetTouchIndicatorPosition(touch.position);
                SetJoystickPosition(touch.position);
                ShowIndicator(true);
                break;

            case TouchPhase.Stationary:
                break;

            case TouchPhase.Moved:
                DetermineJoystickDirection(touch.position);
                SetJoystickPosition(touch.position);
                break;

            case TouchPhase.Ended:
                {
                    ResetTouchControls();
                }
                break;
        }
    }



    private void SetJoystickPosition ( Vector2 position )
    {
        joystick.position = position;
    }

    private void DetermineJoystickDirection ( Vector2 position, bool updateTouchMove = true )
    {
        float distanceFromStart = Vector2.Distance(position, joystickStartPos);

        if (distanceFromStart > movementThreshold)
        {
            bool isLeftSide = position.x < joystickStartPos.x;
            IsHoldingLeft = isLeftSide;
            IsHoldingRight = !isLeftSide;

            if (updateTouchMove)
            {
                Vector2 direction = (position - joystickStartPos).normalized;
                float scaledDistance = Mathf.Clamp01(distanceFromStart / movementThreshold);
                TouchMove = direction * scaledDistance;
            }
        }

        OnTouchDirectionDetermined?.Invoke();
    }


    private void SetTouchIndicatorPosition ( Vector2 position )
    {
        if (!MainMenu.Instance.isActive)
        {
            joystickStartPos = position;
            touchIndicator.transform.position = position;
        }
    }



    public void ResetTouchControls ()
    {
        IsHoldingLeft = false;
        IsHoldingRight = false;
        TouchMove = Vector2.zero;

        if (touchIndicator != null)
            touchIndicator.transform.position = indicatorStartPos;

        joystickStartPos = Vector3.zero;
        joystick.position = Vector3.zero;

        ShowIndicator(false);
    }

    private void ShowIndicator ( bool _isActive )
    {
        touchIndicator?.SetActive(_isActive);
    }

    public void ActivateTouch ( bool _isActive )
    {
        ShowIndicator(_isActive);
        ResetTouchControls();
    }
}
