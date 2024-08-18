using UnityEngine;
using OVR;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.VisualScripting;


public class PaintingManager : MonoBehaviour {
      // This reusable buffer helps reduce pressure on the garbage collector
    List<OVRSpatialAnchor.UnboundAnchor> _unboundAnchors = new();


    List<PaintingObject> paintings = new List<PaintingObject>();

    public void Start()
    {
        //get all keys currently saved in playerprefs
        var keys = PlayerPrefs.GetString("SavedAnchors", "").Split(',');
        if(keys.Length > 0)
        {
            //convert the keys to Guids
            var uuids = new List<Guid>();
            foreach (var key in keys)
            {
                if (key != "")
                {
                    uuids.Add(new Guid(key));
                }
            }
            LoadAnchorsByUuid(uuids);
        }
        

    }

    async void LoadAnchorsByUuid(IEnumerable<Guid> uuids)
    {
        // Step 1: Load
        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(uuids, _unboundAnchors);

        if (result.Success)
        {
            Debug.Log($"Anchors loaded successfully.");

            // Note result.Value is the same as _unboundAnchors
            foreach (var unboundAnchor in result.Value)
            {
                // Step 2: Localize
                unboundAnchor.LocalizeAsync().ContinueWith((success, anchor) =>
                {
                    if (success)
                    {
                        // Create a new game object with an OVRSpatialAnchor component
                        var spatialAnchor = new GameObject($"Anchor {unboundAnchor.Uuid}")
                            .AddComponent<OVRSpatialAnchor>();

                        // Step 3: Bind
                        // Because the anchor has already been localized, BindTo will set the
                        // transform component immediately.
                        unboundAnchor.BindTo(spatialAnchor);
                        FrameData frameData = JsonUtility.FromJson<FrameData>(PlayerPrefs.GetString(unboundAnchor.Uuid.ToString()));
                        spatialAnchor.AddComponent<PaintingObject>().LoadPaintingData(frameData.paintingData);
                        spatialAnchor.transform.localScale = frameData.scale;
                        paintings.Add(spatialAnchor.GetComponent<PaintingObject>());
                    }
                    else
                    {
                        Debug.LogError($"Localization failed for anchor {unboundAnchor.Uuid}");
                    }
                }, unboundAnchor);
            }
        }
        else
        {
            Debug.LogError($"Load failed with error {result.Status}.");
        }
    }
}