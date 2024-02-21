using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private Canvas dialogCanvas;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField][TextArea(3, 10)] private string[] sentences;
    [SerializeField] private float secondsBetweenLetters = 0.05f;

    [Header("Animators")]
    [SerializeField] private Animator puffAnim;
    [SerializeField] private Animator cardAnim;

    private Queue<string> sentencesQueue; // Stores all sentences to display
    private string currentSentence = ""; // The current sentence being displayed
    private bool isTyping = false; // Flag to check if the typing effect is ongoing
    private bool dialogEnded;
    private bool isDialogActive;

    public event EventHandler DialogEnded;

    void Start ()
    {
        sentencesQueue = new Queue<string>();

        if (LevelManager.Instance != null)
            if (LevelManager.Instance.GetLevelDataByNumber(LevelManager.Instance.currentLevelNumber).firstTime == false)
            {
                EndDialog();
                return;
            }

        SlideInAnimation();
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
        if (!dialogEnded)
        {
            AudioManager.Instance?.StopSFX("Type");
            SlideOutAnimation();
            PlayersManager.Instance?.ActivateInputs(true);

            DialogEnded?.Invoke(this, EventArgs.Empty);
            dialogEnded = true;
        }
    }


    void Update ()
    {
        if (dialogEnded) return;
        if (Input.GetMouseButtonDown(0)) // Detect left mouse click
        {
            DisplayNextSentence();
        }
    }
}
