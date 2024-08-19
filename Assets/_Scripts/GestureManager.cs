using System.Collections;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    [SerializeField] private float gestureDelay = 1f; // Delay in seconds
    private bool isLeftThumbsUp;
    private bool isRightThumbsUp;
    private bool isLeftThumbsDown;
    private bool isRightThumbsDown;

    private Coroutine coroutine;

    // Callbacks for thumbs up gestures
    public void OnLeftThumbsUp()
    {
        isLeftThumbsUp = true;
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(WaitAndCheckThumbsUpGesture());
    }

    public void OnRightThumbsUp()
    {
        isRightThumbsUp = true;
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(WaitAndCheckThumbsUpGesture());
    }

    // Callbacks for thumbs down gestures
    public void OnLeftThumbDown()
    {
        isLeftThumbsDown = true;
        if(coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(WaitAndCheckThumbsDownGesture());
    }

    public void OnRightThumbDown()
    {
        isRightThumbsDown = true;
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(WaitAndCheckThumbsDownGesture());
    }

    // Callbacks for swipe gestures (unaffected by thumbs gestures)
    public void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
    }

    public void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
    }

    // Coroutine to handle thumbs-up gestures
    private IEnumerator WaitAndCheckThumbsUpGesture()
    {
        yield return new WaitForSeconds(gestureDelay);

        if (isLeftThumbsUp && isRightThumbsUp)
        {
            Debug.Log("Both Thumbs Up");
        }
        else if (isLeftThumbsUp)
        {
            Debug.Log("Left Thumbs Up");
        }
        else if (isRightThumbsUp)
        {
            Debug.Log("Right Thumbs Up");
        }

        coroutine = null;
    }

    // Coroutine to handle thumbs-down gestures
    private IEnumerator WaitAndCheckThumbsDownGesture()
    {
        yield return new WaitForSeconds(gestureDelay);

        if (isLeftThumbsDown && isRightThumbsDown)
        {
            Debug.Log("Both Thumbs Down");
        }
        else if (isLeftThumbsDown)
        {
            Debug.Log("Left Thumb Down");
        }
        else if (isRightThumbsDown)
        {
            Debug.Log("Right Thumb Down");
        }

        coroutine = null;
    }

    public void ResetLeftThumbFlag()
    {
        Debug.Log("Resetting Left Thumb Flag");
        isLeftThumbsUp = false;
        isLeftThumbsDown = false;
    }

    public void ResetRightThumbFlag()
    {
        Debug.Log("Resetting Right Thumb Flag");
        isRightThumbsUp = false;
        isRightThumbsDown = false;
    }
}
