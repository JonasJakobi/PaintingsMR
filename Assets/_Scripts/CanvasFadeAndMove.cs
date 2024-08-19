using UnityEngine;
using System.Collections;

public class CanvasFadeAndMove : MonoBehaviour
{
    public float fadeDuration = 2f; // Time it takes to fade in/out
    public float moveDistance = -0.1f; // Distance to move in the z direction
    public float moveDuration = 2f; // Time it takes to move the distance

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Material[] materials; // Array to hold the materials of all child renderers

    private void Start()
    {
        // Store the initial position of the Canvas
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, 0, moveDistance);

        // Gather all materials under the GameObject
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            SetMaterialAlpha(materials[i], 0f); // Set initial alpha to 0
        }

        // Set the initial position
        transform.position = initialPosition;
    }

    // This method starts the fade-in and move coroutine
    public void StartFadeAndMove()
    {
        StartCoroutine(FadeInAndMoveCoroutine());
    }

    // This method starts the fade-out and move back coroutine
    public void StartFadeOutAndMoveBack()
    {
        StartCoroutine(FadeOutAndMoveBackCoroutine());
    }

    private IEnumerator FadeInAndMoveCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            // Set the alpha value for all materials
            foreach (Material material in materials)
            {
                SetMaterialAlpha(material, alpha);
            }

            // Calculate the current position in the z direction
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final state is set
        foreach (Material material in materials)
        {
            SetMaterialAlpha(material, 1f);
        }
        transform.position = targetPosition;

        Debug.Log("Fade-in and movement completed");
    }

    private IEnumerator FadeOutAndMoveBackCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Set the alpha value for all materials
            foreach (Material material in materials)
            {
                SetMaterialAlpha(material, alpha);
            }

            // Calculate the current position moving back in the z direction
            transform.position = Vector3.Lerp(targetPosition, initialPosition, elapsedTime / moveDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final state is set
        foreach (Material material in materials)
        {
            SetMaterialAlpha(material, 0f);
        }
        transform.position = initialPosition;

        Debug.Log("Fade-out and movement back completed");
    }

    // Helper method to set the alpha of a material
    private void SetMaterialAlpha(Material material, float alpha)
    {
        if (material.HasProperty("_Color"))
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }
        else if (material.HasProperty("_BaseColor")) // For URP/HDRP shaders
        {
            Color color = material.GetColor("_BaseColor");
            color.a = alpha;
            material.SetColor("_BaseColor", color);
        }
    }
}
