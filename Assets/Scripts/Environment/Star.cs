using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void Start ()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
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
    }

    private IEnumerator MoveToPlayer ( Transform playerTransform )
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;


        while (elapsedTime < duration)
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
