using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TouchController : MonoBehaviour
{
    public static TouchController Instance { get; private set; }


    private Vector2 touchStartPos;
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private float minSwipeDuration = 0.02f;
    private float swipeStartTime;

    public bool IsHoldingLeft { get; private set; }
    public bool IsHoldingRight { get; private set; }
    public bool IsUpSwiping { get; private set; }
    public bool IsHoldingUpSwipe { get; private set; }
    public bool IsDownSwiping { get; private set; }

    public Vector2 TouchMove { get; private set; }

    private GameObject touchIndicator;

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
        touchIndicator = GameObject.Find("Controls Canvas/Touch Indicator");
        touchIndicator?.SetActive(false);
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
    /*
        private void Start ()
        {
            touchIndicator = GameObject.Find("Controls Canvas/Touch Indicator");
            touchIndicator?.SetActive(false);
        }
    */
    void Update ()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch(Input.GetTouch(0));
        }
    }

    private void HandleTouch ( Touch touch )
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;
                swipeStartTime = Time.time;
                DetermineTouchDirection(touch.position);
                break;

            case TouchPhase.Stationary:
                DetermineTouchDirection(touch.position);
                ResetSwipes(touch);
                break;

            case TouchPhase.Moved:
                DetermineTouchDirection(touch.position);
                HandleSwipe(touch);
                break;

            case TouchPhase.Ended:
                ResetTouchControls();
                break;
        }
    }

    private void ResetSwipes ( Touch touch )
    {
        if (IsDownSwiping || IsUpSwiping)
        {
            IsUpSwiping = false;
            IsDownSwiping = false;
            touchStartPos = touch.position;
            swipeStartTime = Time.time;
        }
    }

    private void DetermineTouchDirection ( Vector2 position, bool updateTouchMove = true )
    {
        bool isLeftSide = position.x < Screen.width / 2;
        IsHoldingLeft = isLeftSide;
        IsHoldingRight = !isLeftSide;
        TouchMove = updateTouchMove ? new Vector2(isLeftSide ? -1 : 1, 0) : Vector2.zero;

        touchIndicator.SetActive(true);
        touchIndicator.transform.position = position;
    }

    private void HandleSwipe ( Touch touch )
    {
        Vector2 swipeDelta = touch.position - touchStartPos;
        float swipeDuration = Time.time - swipeStartTime;

        if (!IsUpSwiping && swipeDelta.y > swipeThreshold && swipeDuration > minSwipeDuration)
        {
            IsUpSwiping = true;
            IsHoldingUpSwipe = true;
            // Additional swipe up logic
        }

        if (!IsDownSwiping && swipeDelta.y < -swipeThreshold && swipeDuration > minSwipeDuration)
        {
            IsDownSwiping = true;
            IsHoldingUpSwipe = false;
            // Additional swipe down logic
        }


    }

    private void ResetTouchControls ()
    {
        touchIndicator.SetActive(false);
        IsUpSwiping = false;
        IsHoldingUpSwipe = false;
        IsDownSwiping = false;
        IsHoldingLeft = false;
        IsHoldingRight = false;
        TouchMove = Vector2.zero;
    }
}
