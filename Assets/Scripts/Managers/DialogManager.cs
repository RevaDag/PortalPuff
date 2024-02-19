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
    [SerializeField] private string[] sentences;
    [SerializeField] private float secondsBetweenLetters = 0.05f;

    [Header("Animators")]
    [SerializeField] private Animator puffAnim;
    [SerializeField] private Animator cardAnim;

    private Queue<string> sentencesQueue; // Stores all sentences to display
    private string currentSentence = ""; // The current sentence being displayed
    private bool isTyping = false; // Flag to check if the typing effect is ongoing
    private bool dialogEnded;

    public event EventHandler DialogEnded;

    void Start ()
    {
        sentencesQueue = new Queue<string>();
        SlideInAnimation();
        StartDialog();
    }

    public void SlideInAnimation ()
    {
        puffAnim?.SetTrigger("SlideUp");
        cardAnim?.SetTrigger("SlideRight");
    }

    public void SlideOutAnimation ()
    {
        puffAnim?.SetTrigger("SlideDown");
        cardAnim?.SetTrigger("SlideLeft");
    }

    public void StartDialog ( string[] dialogSentences )
    {
        sentencesQueue.Clear();

        foreach (string sentence in dialogSentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void StartDialog ()
    {
        sentencesQueue.Clear();

        foreach (string sentence in sentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

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
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(secondsBetweenLetters);
        }
        isTyping = false;
    }

    private void CompleteSentence ()
    {
        StopAllCoroutines();
        dialogText.text = currentSentence;
        isTyping = false;
    }

    private void EndDialog ()
    {
        if (!dialogEnded)
        {
            SlideOutAnimation();
            OnDialogEnded(EventArgs.Empty);
            dialogEnded = true;
        }
    }

    protected virtual void OnDialogEnded ( EventArgs e )
    {
        DialogEnded?.Invoke(this, e);
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left mouse click
        {
            DisplayNextSentence();
        }
    }
}
