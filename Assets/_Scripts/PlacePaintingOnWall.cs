using UnityEngine;
using OVR;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;

using Unity.XR.Oculus;
using System.Collections;

public class PlacePaintingOnWall : MonoBehaviour
{
    public OVRHand rightHand;
    public float pinchThreshold = 0.95f;
    public float pinchDuration = 1f;
    public float maxPlacementDistance = 2f;
    public GameObject paintingPrefab;

    private bool isPinching = false;
    private float pinchStartTime;
    private GameObject currentPainting;
    private Vector3 initialPinchPosition;

    void Update()
    {
        if (rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if (!isPinching)
            {
                StartPinch();
            }
            else
            {
                UpdatePinch();
            }
        }
        else
        {
            if (isPinching)
            {
                EndPinch();
            }
        }
    }

    void StartPinch()
    {
        isPinching = true;
        pinchStartTime = Time.time;
        initialPinchPosition = rightHand.PointerPose.position;
    }

    void UpdatePinch()
    {
        if (Time.time - pinchStartTime >= pinchDuration)
        {
            if (currentPainting == null)
            {
                TryPlacePainting();
            }
            else
            {
                ResizePainting();
            }
        }
    }

    void EndPinch()
    {
        isPinching = false;
        currentPainting = null;
    }

    void TryPlacePainting()
    {
        RaycastHit raycastResult;
        if (Physics.Raycast(rightHand.PointerPose.position, rightHand.PointerPose.forward, out raycastResult, maxPlacementDistance))
        {
            currentPainting = Instantiate(paintingPrefab, raycastResult.point, Quaternion.LookRotation(-raycastResult.normal));
            currentPainting.transform.localScale = Vector3.one * 0.1f; // Start with a small size
        
        }
    }

    void ResizePainting()
    {
        if (currentPainting != null)
        {
            Vector3 currentPinchPosition = rightHand.PointerPose.position;
            float distance = Vector3.Distance(initialPinchPosition, currentPinchPosition);
            currentPainting.transform.localScale = Vector3.one * distance;
        }
    }
}