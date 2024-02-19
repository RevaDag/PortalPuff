using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TouchController : MonoBehaviour
{
    public static TouchController Instance { get; private set; }


    private Vector2 touchStartPos;
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private float minSwipeDuration = 0.1f;
    private float swipeStartTime;
    private bool swipeRecognized = false;

    public bool IsHoldingLeft { get; private set; }
    public bool IsHoldingRight { get; private set; }
    public bool IsUpSwiping { get; private set; }
    public bool IsHoldingUpSwipe { get; private set; }
    public bool IsDownSwiping { get; private set; }

    public Vector2 TouchMove { get; private set; }

    [SerializeField] private GameObject touchIndicator;

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
                swipeRecognized = false;
                break;

            case TouchPhase.Stationary:
                DetermineTouchDirection(touch.position);
                ResetSwipes(touch);
                break;

            case TouchPhase.Moved:
                DetermineTouchDirection(touch.position);

                if (!swipeRecognized)
                    HandleSwipe(touch);
                break;

            case TouchPhase.Ended:
                ResetTouchControls();
                break;
        }
    }

    private void ResetSwipes ( Touch touch )
    {
        IsUpSwiping = false;
        IsDownSwiping = false;
        IsHoldingUpSwipe = false;
        swipeRecognized = false;
        touchStartPos = touch.position;
        swipeStartTime = Time.time;
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
        if (!swipeRecognized)
        {
            if (!IsUpSwiping && swipeDelta.y > swipeThreshold && swipeDuration > minSwipeDuration)
            {
                IsUpSwiping = true;
                IsHoldingUpSwipe = true;
                swipeRecognized = true;
            }

            if (!IsDownSwiping && swipeDelta.y < -swipeThreshold && swipeDuration > minSwipeDuration)
            {
                IsDownSwiping = true;
                IsHoldingUpSwipe = false;
                swipeRecognized = true;
            }

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
        swipeRecognized = false;
    }
}
