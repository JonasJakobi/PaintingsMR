using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public CanvasFadeAndMove canvasFadeAndMove; // Reference to the CanvasFadeAndMove script

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered");
        if (other.CompareTag("UIInteractor"))
        {
            // Start the fade-in and move effect on the canvas
            canvasFadeAndMove.StartFadeAndMove();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UIInteractor"))
        {
            // Start the fade-out and move back effect on the canvas
            canvasFadeAndMove.StartFadeOutAndMoveBack();
        }
    }
}
