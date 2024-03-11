using TarodevController;
using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    public enum Type
    {
        Automatic,
        Toggle
    }

    [SerializeField] private Type currrentType;

    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 0.25f;
    [SerializeField] private float secondsDelay = 2f;

    [SerializeField] private bool isActive;

    private Vector3 targetPoint;
    private Vector3 startPosition;
    private float startTime;

    private Vector3 previousPosition;
    private Vector2 velocity;

    private bool movingTowardsB;
    private bool inDelay;
    private bool firstMovement = true;


    void Awake ()
    {
        previousPosition = transform.position;
        startPosition = transform.position;
        ChooseInitialTarget();
    }

    void Start ()
    {
        if (isActive)
            ActivatePlatform(true);
    }

    void Update ()
    {
        if (!isActive) return;

        if (targetPoint == null) return;

        float timeSinceStarted = (Time.time - startTime) * speed;
        float distance = Vector3.Distance(startPosition, targetPoint);

        float journeyFraction = timeSinceStarted / distance;

        if (float.IsNaN(journeyFraction)) return;

        transform.position = Vector3.Lerp(startPosition, targetPoint, journeyFraction);

        velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        if (journeyFraction >= 1)
        {
            if (firstMovement)
            {
                firstMovement = false;
                if (currrentType == Type.Automatic)
                {
                    StartCoroutine(WaitAndChangeTarget(movingTowardsB ? pointB : pointA));
                }
                return;
            }

            if (currrentType == Type.Toggle && !inDelay)
            {
                isActive = false; // Stop further movement until toggled again.
            }
            else if (!inDelay)
            {
                StartCoroutine(WaitAndChangeTarget(movingTowardsB ? pointA : pointB));
            }
        }

    }

    private IEnumerator WaitAndChangeTarget ( Vector3 newTarget )
    {
        inDelay = true;
        yield return new WaitForSeconds(secondsDelay);
        if (currrentType == Type.Automatic || isActive)
        {
            SetTarget(newTarget);
        }
        inDelay = false;
    }

    private void SetTarget ( Vector3 newTarget )
    {
        targetPoint = newTarget;
        movingTowardsB = !movingTowardsB;
        startPosition = transform.position;
        startTime = Time.time;
    }

    private void ChooseInitialTarget ()
    {
        // Removed the initializing parameter since it was only used here and the behavior is now consistent
        startPosition = transform.position;
        startTime = Time.time;

        if (Vector3.Distance(transform.position, pointA) < Vector3.Distance(transform.position, pointB))
            targetPoint = pointB;
        else
            targetPoint = pointA;

        movingTowardsB = (targetPoint == pointB);
    }



    private void OnCollisionStay2D ( Collision2D other )
    {
        if (!isActive)
            ResetPlayerVelocity(other.gameObject);

        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.ApplyEnvironemntVelocity(velocity);
        }
    }


    private void OnCollisionExit2D ( Collision2D other )
    {
        if (other.gameObject.CompareTag("Player"))
            ResetPlayerVelocity(other.gameObject);
    }

    private void ResetPlayerVelocity ( GameObject _player )
    {
        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.ApplyEnvironemntVelocity(Vector2.zero);
    }

    public void ActivatePlatform ( bool _isActive )
    {
        isActive = _isActive;
        if (isActive)
        {
            ChooseInitialTarget(); // Ensure the platform has a target and starts moving immediately
        }
        else
        {
            StopAllCoroutines();
        }

        inDelay = false;
    }

    public void TogglePlatform ()
    {
        ActivatePlatform(!isActive);
    }
}
