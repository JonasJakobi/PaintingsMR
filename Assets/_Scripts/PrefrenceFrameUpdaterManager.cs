using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PrefrenceFrameUpdaterManager : MonoBehaviour
{
    [SerializeField]
    private FrameManager frameManager;

    private Dictionary<PaintingTag, int> likesTally = new Dictionary<PaintingTag, int>();

    public float updateInterval = 10f;


    public Canvas notificationCanvas;
    public Text notificationText;

    void Start()
    {
        frameManager = FrameManager.Instance;

        foreach (PaintingTag tag in System.Enum.GetValues(typeof(PaintingTag)))
        {
            likesTally[tag] = 0;
        }

        notificationCanvas.gameObject.SetActive(false);

        StartCoroutine(UpdateFramesBasedOnPreferences());
    }

    IEnumerator UpdateFramesBasedOnPreferences()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);

            UpdateLikesTally();

            UpdateFrames();
        }
    }

    void UpdateLikesTally()
    {
        foreach (PaintingTag tag in likesTally.Keys.ToList())
        {
            likesTally[tag] = frameManager.GetLikesForTag(tag);
        }
    }

    void UpdateFrames()
    {
        var sortedTags = likesTally.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).ToList();

        foreach (var frame in frameManager.frames)
        {
            var matchingPaintings = frameManager.paintings.Where(p => p.tags.Intersect(sortedTags).Any()).ToList();

            if (matchingPaintings.Count > 0)
            {
                var selectedPainting = matchingPaintings[UnityEngine.Random.Range(0, matchingPaintings.Count)];

                frame.frameData.paintingData = selectedPainting;
                frame.LoadPaintingData(selectedPainting);

                StartCoroutine(DisplayUpdateNotification(frame));
            }
        }
    }

    IEnumerator DisplayUpdateNotification(PaintingObject frame)
    {
        notificationText.text = "Paintings updated according to preference!";
        notificationCanvas.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        notificationCanvas.gameObject.SetActive(false);
    }
}
