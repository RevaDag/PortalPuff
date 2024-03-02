using TarodevController;
using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
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


    void Awake ()
    {
        previousPosition = transform.position;
        startPosition = transform.position;
        ChooseInitialTarget(true);
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
            if (!movingTowardsB) // Reached pointB, moving towards pointA next
            {
                StartCoroutine(WaitAndChangeTarget(pointA));
            }
            else // Reached pointA, moving towards pointB next
            {
                StartCoroutine(WaitAndChangeTarget(pointB));
            }
        }
    }

    private IEnumerator WaitAndChangeTarget ( Vector3 newTarget )
    {
        isActive = false;
        yield return new WaitForSeconds(secondsDelay);
        targetPoint = newTarget;
        movingTowardsB = !movingTowardsB;
        startPosition = transform.position;
        startTime = Time.time;
        isActive = true;
    }

    private void ChooseInitialTarget ( bool initializing )
    {
        if (!initializing) // If not initializing, update start position to current position
        {
            startPosition = transform.position;
            startTime = Time.time;
        }

        // Determine the direction to move based on the current or initial position
        if (Vector3.Distance(transform.position, pointA) < Vector3.Distance(transform.position, pointB))
        {
            targetPoint = movingTowardsB ? pointB : pointA;
        }
        else
        {
            targetPoint = movingTowardsB ? pointA : pointB;
        }

        movingTowardsB = !movingTowardsB;
    }


    private void OnCollisionStay2D ( Collision2D other )
    {
        if (!isActive)
        {
            ResetPlayerVelocity(other.gameObject);
        }

        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.ApplyEnvironemntVelocity(velocity);
        }
    }


    private void OnCollisionExit2D ( Collision2D other )
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ResetPlayerVelocity(other.gameObject);
        }
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
            startTime = Time.time;
            startPosition = transform.position;
        }
    }

    public void TogglePlatform ()
    {
        ActivatePlatform(!isActive);
    }
}
