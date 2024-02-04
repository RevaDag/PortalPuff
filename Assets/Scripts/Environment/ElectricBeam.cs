using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class ElectricBeam : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 0.25f;

    private float timeCounter = 0;
    private bool isActive = true;

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

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Die();
        }
    }
}
