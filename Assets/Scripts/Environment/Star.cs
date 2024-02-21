using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    //private Animator _anim;
    private ParticleSystem _particleSystem;

    private void Start ()
    {
        //_anim = GetComponent<Animator>();
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        // Check if the collider is tagged as Player
        if (collision.CompareTag("Player"))
        {
            Collect(collision.transform);
        }
    }

    private void Collect ( Transform _player )
    {
        GetComponent<Collider2D>().enabled = false;
        LevelManager.Instance?.CollectStar();

        AudioManager.Instance?.PlaySFX("StarCollected");
        _particleSystem.Play();
        StartCoroutine(MoveToPlayer(_player));

        // Play a collect sound if you have one
        // AudioSource.PlayClipAtPoint(collectSound, transform.position);
    }

    private IEnumerator MoveToPlayer ( Transform playerTransform )
    {
        //_anim.enabled = false;
        float duration = 1f; // Duration in seconds over which the star moves to the player
        float elapsedTime = 0f; // Elapsed time since the start of the interpolation
        Vector3 startPosition = transform.position; // Starting position of the star


        while (elapsedTime < duration)
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);


            // Calculate the new position based on elapsed time
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait until the next frame
        }


        // Call to increment the star count, if not already done
        // LevelManager.Instance.CollectStar();

        // Destroy the star object after reaching the player
        Destroy(gameObject);
    }

}
