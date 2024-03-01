using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class TouchController : MonoBehaviour
{
    public static TouchController Instance { get; private set; }

    [SerializeField] private LayerMask controlsLayerMask;

    public bool IsHoldingLeft { get; private set; }
    public bool IsHoldingRight { get; private set; }
    public Vector2 TouchMove { get; private set; }

    private GameObject touchIndicator;


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
    }


    void Update ()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (!IsTouchOverControls(touch))
                HandleTouch(touch);
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
            {
                return true;
            }
        }
        return false;
    }


    private void HandleTouch ( Touch touch )
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                DetermineTouchDirection(touch.position);
                break;

            case TouchPhase.Stationary:
                DetermineTouchDirection(touch.position);
                break;

            case TouchPhase.Moved:
                SetTouchIndicatorPosition(touch.position, touchIndicator);
                break;

            case TouchPhase.Ended:
                {
                    ResetTouchControls();
                    ShowIndicator(false);
                }
                break;
        }
    }



    private void DetermineTouchDirection ( Vector2 position, bool updateTouchMove = true )
    {
        bool isLeftSide = position.x < Screen.width / 2;
        IsHoldingLeft = isLeftSide;
        IsHoldingRight = !isLeftSide;
        TouchMove = updateTouchMove ? new Vector2(isLeftSide ? -1 : 1, 0) : Vector2.zero;

        OnTouchDirectionDetermined?.Invoke();

        controlsUI?.ShowPlayerControls(isLeftSide);
        SetTouchIndicatorPosition(position, touchIndicator);
    }

    private void SetTouchIndicatorPosition ( Vector2 position, GameObject indicator )
    {
        if (!MainMenu.Instance.isActive)
        {
            indicator.SetActive(true);
            indicator.transform.position = position;
        }
    }



    private void ResetTouchControls ()
    {
        IsHoldingLeft = false;
        IsHoldingRight = false;
        TouchMove = Vector2.zero;
    }

    private void ShowIndicator ( bool _isActive )
    {
        touchIndicator.SetActive(_isActive);
    }

    public void ActivateTouch ( bool _isActive )
    {
        ShowIndicator(_isActive);
        ResetTouchControls();
    }
}
