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
    [SerializeField] private bool isActive = true;


    void Update ()
    {
        if (!isActive) { return; }

        timeCounter += speed * Time.deltaTime;
        float t = Mathf.PingPong(timeCounter, 1);
        transform.position = Vector3.Lerp(pointA, pointB, t);
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance?.PlaySFX("Shock");
            collision.GetComponent<PlayerController>().Die();
        }
    }

    public void ActivateBeam ( bool _isActive )
    {
        isActive = _isActive;
    }
}
