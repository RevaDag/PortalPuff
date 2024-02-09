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
    [SerializeField] private PauseMenu pauseMenu;
    private string nextLevel;

    private void Awake ()
    {
        LevelManager.Instance.levelSummary = this;
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
        // Activate stars based on the number parameter
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < number)
                stars[i].SetActive(true);
            else
                stars[i].SetActive(false);
        }
    }

    public void SetNextLevel ( string _nextLevel )
    {
        nextLevel = _nextLevel;
    }

    public void OpenPauseMenu ()
    {
        pauseMenu.ActivateCanvas(true);
    }

    public void NextLevel ()
    {
        SceneManager.LoadScene(nextLevel);
        LevelManager.Instance.NextLevel();
    }

    public void ResetLevel ()
    {
        pauseMenu.ReloadCurrentScene();
        LevelManager.Instance.ResetStars();
    }
}
