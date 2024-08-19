using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class imageScaler : MonoBehaviour
{
    public GameObject cubePrefab;
    public Texture imageTexture;
    public float delayBeforeAppear = 4f;
    public float imageMargin = 0.1f;

    private GameObject cubeInstance;
    private GameObject imageInstance;
    private RawImage rawImage;
    private Vector3 lastCubeScale;

    void Start()
    {
        StartCoroutine(SpawnCubeWithDelay());
    }

    IEnumerator SpawnCubeWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeAppear);

        cubeInstance = Instantiate(cubePrefab);
        lastCubeScale = cubeInstance.transform.localScale;

        imageInstance = new GameObject("Image");
        imageInstance.transform.SetParent(cubeInstance.transform, false);
        rawImage = imageInstance.AddComponent<RawImage>();
        rawImage.texture = imageTexture;

        imageInstance.transform.localPosition = new Vector3(0, 0, cubeInstance.transform.localScale.z / 2 + 0.01f); // Slightly in front of the cube face
        imageInstance.transform.localRotation = Quaternion.identity;

        UpdateImageScale();
    }

    void Update()
    {
        if (cubeInstance != null && imageInstance != null)
        {
            if (cubeInstance.transform.localScale != lastCubeScale)
            {
                UpdateImageScale();
                lastCubeScale = cubeInstance.transform.localScale;
            }
        }
    }

    void UpdateImageScale()
    {
        if (rawImage == null || rawImage.texture == null) return;

        float cubeWidth = cubeInstance.transform.localScale.x;
        float cubeHeight = cubeInstance.transform.localScale.y;

        // Apply margin
        cubeWidth *= (1 - imageMargin);
        cubeHeight *= (1 - imageMargin);

        float imageAspect = (float)rawImage.texture.width / rawImage.texture.height;

        float scaleX, scaleY;
        if (cubeWidth / cubeHeight > imageAspect)
        {

            scaleY = cubeHeight;
            scaleX = scaleY * imageAspect;
        }
        else
        {
            scaleX = cubeWidth;
            scaleY = scaleX / imageAspect;
        }

        // Apply the calculated scale
        rawImage.rectTransform.sizeDelta = new Vector2(scaleX, scaleY);
    }
}