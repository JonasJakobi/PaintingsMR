using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PrefrenceFrameUpdaterManager : MonoBehaviour
{
    private FrameManager frameManager;

    private Dictionary<PaintingTag, int> likesTally = new Dictionary<PaintingTag, int>();

    public float updateInterval = 10f;

    public Canvas notificationCanvas;
    public Text notificationText; void Start()
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
            List<PaintingData> matchingPaintings = frameManager.paintings
                .Where(p => p.tags.Intersect(sortedTags).Any() && p.isLandsape == frame.isLandsape())
                .Where(p => !frameManager.pickedPaintings.Contains(p) && p != frame.frameData.paintingData)
                .ToList();

            if (matchingPaintings.Count > 0)
            {
                var selectedPainting = matchingPaintings[UnityEngine.Random.Range(0, matchingPaintings.Count)];
                frame.frameData.paintingData = selectedPainting;
                frameManager.pickedPaintings.Add(selectedPainting);
                frame.LoadPaintingData(selectedPainting);
                Debug.Log($"Frame updated with painting '{selectedPainting.name}' based on preferences.");

                StartCoroutine(DisplayUpdateNotification(frame));
                Debug.Log("Updated painting for frame " + frame.name);
                Debug.Log("list of matching paints: " + matchingPaintings);

            }
        }
    }

    IEnumerator DisplayUpdateNotification(PaintingObject frame)
    {
        notificationText.text = "Painting updated according to preference!";
        notificationCanvas.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        notificationCanvas.gameObject.SetActive(false);
    }
}
