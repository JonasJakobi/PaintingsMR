using System.Collections;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    [SerializeField] private float gestureDelay = 0.5f; // Delay in seconds
    [SerializeField] private float pictureRaycast = 2f; // Raycast distance for picture
    [SerializeField] private LayerMask paintingLayer;

    [SerializeField] private ThumbEffect thumbEffect;
    [SerializeField] private Transform centerEyeTransform, leftHandTransform, rightHandTransform;
    private bool isLeftThumbsUp, isRightThumbsUp, isLeftThumbsDown, isRightThumbsDown;

    private Coroutine coroutine;

    void Update()
    {
        Debug.DrawRay(centerEyeTransform.position, centerEyeTransform.forward * pictureRaycast, Color.red);
    }

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
        Vector3 hitPos = GetCenterRaycastToPainting();
        
        if(hitPos == Vector3.zero)
        {
            Debug.Log("No Picture Detected");
            ResetLeftThumbFlag();
            ResetRightThumbFlag();
            coroutine = null;
            yield break;
        }

        if (isLeftThumbsUp && isRightThumbsUp)
        {
            Debug.Log("Both Thumbs Up");
            SpawnThumb(true, leftHandTransform, hitPos);
            SpawnThumb(true, rightHandTransform, hitPos);
        }
        else if (isLeftThumbsUp)
        {
            Debug.Log("Left Thumbs Up");
            SpawnThumb(true, leftHandTransform, hitPos);
        }
        else if (isRightThumbsUp)
        {
            Debug.Log("Right Thumbs Up");
            SpawnThumb(true, rightHandTransform, hitPos);
        }

        coroutine = null;
    }

    // Coroutine to handle thumbs-down gestures
    private IEnumerator WaitAndCheckThumbsDownGesture()
    {
        yield return new WaitForSeconds(gestureDelay);
        Vector3 hitPos = GetCenterRaycastToPainting();

        if(hitPos == Vector3.zero)
        {
            Debug.Log("No Picture Detected");
            ResetLeftThumbFlag();
            ResetRightThumbFlag();
            coroutine = null;
            yield break;
        }

        if (isLeftThumbsDown && isRightThumbsDown)
        {
            Debug.Log("Both Thumbs Down");
            SpawnThumb(false, leftHandTransform, hitPos);
            SpawnThumb(false, rightHandTransform, hitPos);
        }
        else if (isLeftThumbsDown)
        {
            Debug.Log("Left Thumb Down");
            SpawnThumb(false, leftHandTransform, hitPos);
        }
        else if (isRightThumbsDown)
        {
            Debug.Log("Right Thumb Down");
            SpawnThumb(false, rightHandTransform, hitPos);
        }

        coroutine = null;
    }

    public void ResetLeftThumbFlag()
    {
        Debug.Log("Resetting Left Thumb Flag");
        isLeftThumbsUp = false;
        isLeftThumbsDown = false;
    }

    private Vector3 GetCenterRaycastToPainting()
    {
        if(Physics.Raycast(centerEyeTransform.position, centerEyeTransform.forward, out RaycastHit hit, pictureRaycast, paintingLayer))
        {
            if(hit.collider.CompareTag("Painting"))
                return hit.point;
        }
        return Vector3.zero;
    }

    public void ResetRightThumbFlag()
    {
        Debug.Log("Resetting Right Thumb Flag");
        isRightThumbsUp = false;
        isRightThumbsDown = false;
    }

    private void SpawnThumb(bool thumbsUp, Transform handTransform, Vector3 targetPos)
    {
        Quaternion adjustedRot = Quaternion.Euler(0, -handTransform.localRotation.eulerAngles.z, 0);
        Instantiate(thumbEffect, handTransform.position, adjustedRot).Setup(thumbsUp, targetPos);
    }
}
