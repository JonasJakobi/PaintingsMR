using UnityEngine;
using OVR;
using System.Collections;

public class PlacePaintingOnWall : MonoBehaviour
{
    public OVRHand rightHand; // Reference to the right-hand controller (OVRHand)
    public float pinchThreshold = 0.95f; // Threshold to detect a pinch gesture
    public float pinchDuration = 1f; // Duration the pinch gesture needs to be held before placing or resizing
    public float maxPlacementDistance = 2f; // Maximum distance for placing the cube on the wall
    public GameObject cubePrefab; // Prefab of the cube to be placed
    public GameObject paintingPrefab; // Prefab of the painting to be placed on top of the cube

    private bool isPinching = false; // Tracks whether the user is currently pinching
    private float pinchStartTime; // Stores the time when the pinch gesture started
    private GameObject currentCube; // Reference to the currently placed cube
    private Vector3 initialPinchPosition; // Position of the hand when the pinch started
    private Vector3 initialScale = new Vector3(0.2f, 0.2f, 0.001f); // Initial scale for the cube

    void Update()
    {
        // Check if the user is pinching with the index finger
        if (rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if (!isPinching) // If pinch just started
            {
                StartPinch(); // Start pinch logic
            }
            else // If already pinching
            {
                UpdatePinch(); // Update pinch logic
            }
        }
        else
        {
            if (isPinching) // If the user stops pinching
            {
                EndPinch(); // End pinch logic
            }
        }
    }

    void StartPinch()
    {
        isPinching = true; // Set the pinching flag to true
        pinchStartTime = Time.time; // Record the time when the pinch started
        initialPinchPosition = rightHand.PointerPose.position; // Store the initial position of the pinch

        // Try placing the cube when the pinch starts
        TryPlaceCube();
    }

    void UpdatePinch()
    {
        // Check if the pinch duration has been met or exceeded
        if (Time.time - pinchStartTime >= pinchDuration && currentCube != null)
        {
            ResizeCube(); // Resize the cube while pinching
        }
    }

    void EndPinch()
    {
        isPinching = false; // Reset the pinching flag

        if (currentCube != null) // If there is a cube that was resized
        {
            PlacePaintingOnCube(); // Place the painting on top of the cube
            currentCube = null; // Clear the current cube reference
        }
    }

    void TryPlaceCube()
    {
        RaycastHit raycastResult;
        // Cast a ray from the hand position forward and check if it hits within the max placement distance
        if (Physics.Raycast(rightHand.PointerPose.position, rightHand.PointerPose.forward, out raycastResult, maxPlacementDistance))
        {
            // Instantiate the cube at the hit point, oriented to face away from the wall
            currentCube = Instantiate(cubePrefab, raycastResult.point, Quaternion.LookRotation(-raycastResult.normal));
            currentCube.transform.localScale = Vector3.zero; // Start with a scale of zero
            currentCube.SetActive(true); // Activate the cube when it's placed
        }
    }

    void ResizeCube()
    {
        if (currentCube != null) // If there is a cube to resize
        {
            Vector3 currentPinchPosition = rightHand.PointerPose.position; // Get the current hand position
            float distance = Vector3.Distance(initialPinchPosition, currentPinchPosition); // Calculate the movement distance
            currentCube.transform.localScale = initialScale * distance; // Scale the cube based on the movement distance
        }
    }

    void PlacePaintingOnCube()
    {
        if (currentCube != null && paintingPrefab != null) // Ensure there is a cube and painting prefab
        {
            // Calculate the final scale of the painting
            Vector3 paintingScale = currentCube.transform.localScale - new Vector3(0.01f, 0.01f, 0.0f);

            // Ensure the painting scale does not go negative due to the subtraction
            paintingScale = new Vector3(
                Mathf.Max(0.01f, paintingScale.x),
                Mathf.Max(0.01f, paintingScale.y),
                Mathf.Max(0.001f, paintingScale.z)
            );

            // Instantiate the painting prefab at the same position and orientation as the cube
            GameObject painting = Instantiate(paintingPrefab, currentCube.transform.position, currentCube.transform.rotation);

            // Set the painting's scale slightly smaller than the cube
            painting.transform.localScale = paintingScale;
        }
    }
}
