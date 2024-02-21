using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    public float fadeDuration = 1f; // Duration of the fade
    private Image fadeImage;


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
        fadeImage = GetComponent<Image>();
        fadeImage.enabled = true;
        FadeIn();
    }

    public void FadeIn ()
    {
        StartCoroutine(FadeScreen(0)); // Fade to transparent (alpha = 0)
    }

    public void FadeOut ()
    {
        StartCoroutine(FadeScreen(1)); // Fade to opaque (alpha = 1)
    }

    private IEnumerator FadeScreen ( float targetAlpha )
    {
        float alpha = fadeImage.color.a;

        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp(alpha, targetAlpha, t));
            fadeImage.color = newColor;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }
}
