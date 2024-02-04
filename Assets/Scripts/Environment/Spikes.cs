using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Access the PlayerController script and call the Die function
            collision.gameObject.GetComponent<PlayerController>().Die();
        }
    }
}
