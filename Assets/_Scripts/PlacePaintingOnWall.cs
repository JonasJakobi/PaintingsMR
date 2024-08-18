using UnityEngine;
using OVR;
using System.Collections;

public class PlacePaintingOnWall : MonoBehaviour
{
    public OVRHand rightHand;
    public float pinchThreshold = 0.95f;
    public float pinchDuration = 1f;
    public float maxPlacementDistance = 2f;
    public GameObject cubePrefab;
    public GameObject paintingPrefab;

    private bool isPinching = false;
    private float pinchStartTime;
    private GameObject currentCube;
    private Vector3 initialPinchPosition;

    private Vector3 startPoint, endPoint;
    private Vector3 center, size;

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

        startPoint = rightHand.PointerPose.position;

        TryPlaceCube();
    }

    void UpdatePinch()
    {
        if (Time.time - pinchStartTime >= pinchDuration && currentCube != null)
        {
            endPoint = rightHand.PointerPose.position;

            UpdateCenterAndSize();

            UpdatePreviewRectangle();
        }
    }

    void EndPinch()
    {
        isPinching = false;

        if (currentCube != null)
        {
            PlacePaintingOnCube();
        }
    }

    void TryPlaceCube()
    {
        RaycastHit raycastResult;
        if (Physics.Raycast(rightHand.PointerPose.position, rightHand.PointerPose.forward, out raycastResult, maxPlacementDistance))
        {
            currentCube = Instantiate(cubePrefab, raycastResult.point, Quaternion.LookRotation(-raycastResult.normal));
            currentCube.SetActive(true);
        }
    }

    void UpdateCenterAndSize()
    {
        center = (startPoint + endPoint) / 2f;
        size = new Vector3(Mathf.Abs(endPoint.x - startPoint.x), Mathf.Abs(endPoint.y - startPoint.y), Mathf.Abs(endPoint.z - startPoint.z));
    }

    void UpdatePreviewRectangle()
    {
        if (currentCube != null)
        {
            currentCube.transform.position = center;
            currentCube.transform.localScale = size;
        }
    }

    void PlacePaintingOnCube()
    {
        if (currentCube != null && paintingPrefab != null)
        {
            Vector3 paintingScale = currentCube.transform.localScale - new Vector3(0.0014f, 0.0014f, 0.0f);

            paintingScale = new Vector3(
                Mathf.Max(0.001f, paintingScale.x),
                Mathf.Max(0.001f, paintingScale.y),
                paintingScale.z
            );

            GameObject painting = Instantiate(paintingPrefab, currentCube.transform.position, currentCube.transform.rotation);
            painting.transform.localScale = paintingScale;

            currentCube.transform.localScale = paintingScale + new Vector3(0.0014f, 0.0014f, 0.0f);
        }
    }
}
