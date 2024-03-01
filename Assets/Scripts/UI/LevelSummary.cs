using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class LevelSummary : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Canvas canvas;

    private void Awake ()
    {
        if (LevelManager.Instance == null) return;
        LevelManager.Instance.levelSummary = this;
        LevelManager.Instance.ResetStars();
    }

    public void ActivateCanvas ( bool isActive )
    {
        canvas.enabled = isActive;
        FadeOut();

    }

    public void FadeIn ()
    {
        StartCoroutine(FadeScreen(0)); // Fade to transparent (alpha = 0)
    }

    public void FadeOut ()
    {
        StartCoroutine(FadeScreen(0.3f));
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

    public void UpdateLevelText ( string text )
    {
        if (levelText != null)
            levelText.text = text;
    }

    public void SetStars ( int number )
    {
        StartCoroutine(ActivateStarsWithDelay(number));
    }

    private IEnumerator ActivateStarsWithDelay ( int number )
    {
        // Ensure number is within bounds
        number = Mathf.Clamp(number, 0, stars.Length);

        for (int i = 0; i < number; i++)
        {
            stars[i].SetActive(true);
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance?.PlaySFX("StarUI");
        }

        // Deactivate any remaining stars
        for (int i = number; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
    }

    public void OpenPauseMenu ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        MainMenu.Instance?.ActivateCanvas(true);
        TouchController.Instance?.ActivateTouch(false);
    }

    public void NextLevel ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        LevelManager.Instance.NextLevel();
    }

    public void ResetLevel ()
    {
        AudioManager.Instance?.PlaySFX("Click");
        MainMenu.Instance?.ReloadCurrentScene();
        LevelManager.Instance.ResetStars();
        AudioManager.Instance?.PlayMusic("TrickyFox");
    }
}
