using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    public float fadeDuration = 1f; // Duration of the fade
    [SerializeField] private Image fadeImage;


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

    private void Start ()
    {
        fadeImage.enabled = true;
        FadeIn();
    }

    public void FadeIn ( Action onComplete = null )
    {
        StartCoroutine(FadeScreen(0, onComplete));
    }

    public void FadeOut ( Action onComplete = null )
    {
        StartCoroutine(FadeScreen(1, onComplete));
    }


    private IEnumerator FadeScreen ( float targetAlpha, Action onComplete = null )
    {
        float alpha = fadeImage.color.a;

        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp(alpha, targetAlpha, t));
            fadeImage.color = newColor;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
        onComplete?.Invoke(); // Invoke the callback if it's not null
    }

    public IEnumerator FadeOutInRoutine ( float waitTimeBetween = 0.5f, Action onComplete = null )
    {
        // First, fade out to the specified color (usually black)
        yield return StartCoroutine(FadeScreen(1)); // Fade to opaque

        // Wait for a specified amount of time in the fully faded state
        yield return new WaitForSeconds(waitTimeBetween);

        // Then, fade back in to transparent
        yield return StartCoroutine(FadeScreen(0)); // Fade to transparent

        onComplete?.Invoke();
    }
}
