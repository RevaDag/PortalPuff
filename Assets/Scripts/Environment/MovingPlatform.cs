using TarodevController;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 0.25f;

    private float timeCounter = 0;
    [SerializeField] private bool isActive;

    private Vector3 previousPosition;
    private Vector2 velocity;

    void Start ()
    {
        previousPosition = transform.position;
    }


    void Update ()
    {
        if (!isActive) { return; }

        timeCounter += speed * Time.deltaTime;
        float t = Mathf.PingPong(timeCounter, 1);
        transform.position = Vector3.Lerp(pointA, pointB, t);

        velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
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
    }

    public void TogglePlatform ()
    {
        isActive = !isActive;
    }
}
