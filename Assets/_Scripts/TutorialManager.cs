using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Public fields to be set in the Unity Inspector
    [SerializeField] private List<string> tutorialMessages;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject tutorialObject;  // The tutorial game object
    [SerializeField] private float pauseTime;

    private Coroutine tutorialCoroutine;
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Start the tutorial coroutine
        tutorialCoroutine = StartCoroutine(DisplayTutorialMessages());
    }

    // Coroutine that cycles through the tutorial messages
    private IEnumerator DisplayTutorialMessages()
    {
        while (currentIndex < tutorialMessages.Count)
        {
            // Set the text field to the current message
            tutorialText.text = tutorialMessages[currentIndex];

            // Wait for the specified time before moving to the next message
            yield return new WaitForSeconds(pauseTime);

            // Move to the next message
            currentIndex++;
        }

        // Optionally, you could disable the tutorial after all messages are shown
        DisableTutorial();
    }

    // Method to stop the tutorial and disable the tutorial game object
    public void DisableTutorial()
    {
        // Stop the coroutine if it's running
        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
        }

        // Disable the tutorial game object
        tutorialObject.SetActive(false);
    }

    // Method to skip to the next message immediately
    public void SkipToNextMessage()
    {
        // Stop the current coroutine
        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
        }

        // Move to the next message
        currentIndex++;

        // If there are more messages to display, restart the coroutine
        if (currentIndex < tutorialMessages.Count)
        {
            tutorialCoroutine = StartCoroutine(DisplayTutorialMessages());
        }
        else
        {
            // If no more messages, disable the tutorial
            DisableTutorial();
        }
    }
}
