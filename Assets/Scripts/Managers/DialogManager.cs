using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private Canvas dialogCanvas;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField][TextArea(3, 10)] private string[] sentences;
    [SerializeField] private float secondsBetweenLetters = 0.05f;

    [Header("Animators")]
    [SerializeField] private Animator puffAnim;
    [SerializeField] private Animator cardAnim;

    private Queue<string> sentencesQueue;
    private string currentSentence = "";
    private bool isTyping = false;
    private bool dialogEnded;
    private bool isDialogActive;

    [SerializeField] private bool thankYou;

    public event EventHandler DialogEnded;

    void Start ()
    {
        sentencesQueue = new Queue<string>();

        if (!thankYou)
        {
            if (LevelManager.Instance != null)
                if (LevelManager.Instance.GetLevelDataByNumber(LevelManager.Instance.currentLevelNumber).isDialogShown)
                {
                    EndDialog();
                    return;
                }

            SlideInAnimation();

        }

        StartDialog();
    }

    public void SlideInAnimation ()
    {
        if (!isDialogActive)
        {
            puffAnim?.SetTrigger("SlideUp");
            cardAnim?.SetTrigger("SlideRight");
            isDialogActive = true;
        }
    }

    public void SlideOutAnimation ()
    {
        if (isDialogActive)
        {
            puffAnim?.SetTrigger("SlideDown");
            cardAnim?.SetTrigger("SlideLeft");
            isDialogActive = false;
        }
    }


    public void StartDialog ()
    {
        sentencesQueue.Clear();

        foreach (string sentence in sentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

        PlayersManager.Instance?.ActivateInputs(false);
        DisplayNextSentence();
    }


    public void DisplayNextSentence ()
    {
        if (isTyping)
        {
            CompleteSentence();
            return;
        }

        if (sentencesQueue.Count == 0)
        {
            EndDialog();
            return;
        }

        currentSentence = sentencesQueue.Dequeue();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence ( string sentence )
    {
        AudioManager.Instance?.PlaySFX("Type");
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(secondsBetweenLetters);
        }
        isTyping = false;
        AudioManager.Instance?.StopSFX("Type");
    }

    private void CompleteSentence ()
    {
        StopAllCoroutines();
        AudioManager.Instance?.StopSFX("Type");
        dialogText.text = currentSentence;
        isTyping = false;
    }

    private void EndDialog ()
    {
        AudioManager.Instance?.StopSFX("Type");
        SlideOutAnimation();

        PlayersManager.Instance?.ActivateInputs(true);

        if (!thankYou)
            if (LevelManager.Instance != null)
                LevelManager.Instance.GetLevelDataByNumber(LevelManager.Instance.currentLevelNumber).isDialogShown = true;

        DialogEnded?.Invoke(this, EventArgs.Empty);
        dialogEnded = true;
    }

    public void LoadMainMenu ()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void Update ()
    {
        if (dialogEnded) return;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            if (!dialogEnded)
                DisplayNextSentence();
    }
}
