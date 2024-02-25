using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TouchController : MonoBehaviour
{
    public static TouchController Instance { get; private set; }


    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private float minSwipeDuration = 0.1f;

    public bool IsHoldingLeft { get; private set; }
    public bool IsHoldingRight { get; private set; }
    public bool IsUpSwiping { get; private set; }
    public bool IsHoldingUpSwipe { get; private set; }
    public bool IsDownSwiping { get; private set; }

    public Vector2 TouchMove { get; private set; }

    private GameObject firstTouchIndicator;
    private GameObject secondTouchIndicator;

    private bool isTouchActive = true;

    private int firstTouchId = -1;
    private Vector2 firstTouchStartPos;
    private float firstTouchSwipeStartTime;

    private Vector2 secondTouchStartPos;
    private float secondSwipeStartTime;
    private bool swipeRecognized = false;

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
        firstTouchIndicator = transform.GetChild(0).gameObject;
        secondTouchIndicator = transform.GetChild(1).gameObject;
    }

    void Update ()
    {
        if (!isTouchActive) return;

        if (Input.touchCount > 0 && Input.touchCount <= 2)
        {
            foreach (Touch touch in Input.touches)
            {
                if (firstTouchId == -1 || touch.fingerId == firstTouchId)
                {
                    HandleFirstTouch(touch);
                }
                else
                {
                    HandleAdditionalTouches(touch);
                }
            }
        }
    }

    private void HandleFirstTouch ( Touch touch )
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                firstTouchStartPos = touch.position;
                firstTouchSwipeStartTime = Time.time;
                firstTouchId = touch.fingerId;
                DetermineTouchDirection(touch.position);
                break;

            case TouchPhase.Stationary:
                firstTouchStartPos = touch.position;
                firstTouchSwipeStartTime = Time.time;
                DetermineTouchDirection(touch.position);
                break;

            case TouchPhase.Moved:
                SetTouchIndicatorPosition(touch.position, firstTouchIndicator);
                HandleSwipe(touch, firstTouchStartPos, firstTouchSwipeStartTime);
                break;

            case TouchPhase.Ended:
                if (touch.fingerId == firstTouchId)
                {
                    ResetTouchControls();
                    ShowIndicator(false, firstTouchIndicator);
                    ShowIndicator(false, secondTouchIndicator);
                    firstTouchId = -1;
                }
                break;
        }
    }


    private void HandleAdditionalTouches ( Touch touch )
    {
        if (firstTouchId == -1)
        {
            HandleFirstTouch(touch);
            return;
        }

        switch (touch.phase)
        {
            case TouchPhase.Began:
                secondTouchStartPos = touch.position;
                secondSwipeStartTime = Time.time;
                SetTouchIndicatorPosition(touch.position, secondTouchIndicator);
                break;

            case TouchPhase.Stationary:
                secondTouchStartPos = touch.position;
                secondSwipeStartTime = Time.time;
                break;


            case TouchPhase.Moved:
                HandleSwipe(touch, secondTouchStartPos, secondSwipeStartTime);
                SetTouchIndicatorPosition(touch.position, secondTouchIndicator);
                break;

            case TouchPhase.Ended:
                ShowIndicator(false, secondTouchIndicator);
                ResetSwipes(touch);
                break;
        }
    }

    private void ResetSwipes ( Touch touch )
    {
        IsUpSwiping = false;
        IsDownSwiping = false;
        IsHoldingUpSwipe = false;
        swipeRecognized = false;
        secondTouchStartPos = touch.position;
        secondSwipeStartTime = Time.time;
    }

    private void DetermineTouchDirection ( Vector2 position, bool updateTouchMove = true )
    {
        bool isLeftSide = position.x < Screen.width / 2;
        IsHoldingLeft = isLeftSide;
        IsHoldingRight = !isLeftSide;
        TouchMove = updateTouchMove ? new Vector2(isLeftSide ? -1 : 1, 0) : Vector2.zero;

        SetTouchIndicatorPosition(position, firstTouchIndicator);
    }

    private void SetTouchIndicatorPosition ( Vector2 position, GameObject indicator )
    {
        indicator.SetActive(true);
        indicator.transform.position = position;
    }


    private void HandleSwipe ( Touch touch, Vector2 startTouchPosition, float startSwipeTime )
    {
        Vector2 swipeDelta = touch.position - startTouchPosition;
        float swipeDuration = Time.time - startSwipeTime;
        if (!swipeRecognized)
        {
            if (swipeDelta.y > swipeThreshold && swipeDuration > minSwipeDuration)
            {
                IsUpSwiping = true;
                swipeRecognized = true;
            }
            else if (swipeDelta.y < -swipeThreshold && swipeDuration > minSwipeDuration)
            {
                IsDownSwiping = true;
                swipeRecognized = true;
            }
        }
        else
        {
            if (IsUpSwiping)
            {
                IsHoldingUpSwipe = true;
                IsUpSwiping = false;
            }

            IsDownSwiping = false;
        }
    }

    private void ResetTouchControls ()
    {
        IsUpSwiping = false;
        IsHoldingUpSwipe = false;
        IsDownSwiping = false;
        IsHoldingLeft = false;
        IsHoldingRight = false;
        TouchMove = Vector2.zero;
        swipeRecognized = false;
    }

    private void ShowIndicator ( bool _isActive, GameObject indicator )
    {
        indicator.SetActive(_isActive);
    }


    public void ActivateTouch ( bool _isActive )
    {
        isTouchActive = _isActive;
        ResetTouchControls();
    }
}
