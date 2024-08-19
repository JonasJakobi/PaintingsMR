using UnityEngine;
using OVR;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using Unity.VisualScripting;


public class FrameManager : MonoBehaviour {
      // This reusable buffer helps reduce pressure on the garbage collector
    List<OVRSpatialAnchor.UnboundAnchor> _unboundAnchors = new();

    public List<PaintingData> paintings = new List<PaintingData>();
    public List<PaintingObject> frames = new List<PaintingObject>();

    public static FrameManager Instance;

    public float newPaintingsInterval = 10f;
    
    
    
    public void Start()
    {
        //get all painting data
        var paintingData = Resources.LoadAll<PaintingData>("Paintings");
        //add all painting data to the list
        foreach (var painting in paintingData)
        {
            paintings.Add(painting);
        }
        Instance = this;
        StartCoroutine(PickNewPaintingsRoutine());

        //----------------------------RETURNING RN -------->
        return;
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

    public void RegisterNewFrame(PaintingObject obj){
        frames.Add(obj);
    }

    public IEnumerator PickNewPaintingsRoutine(){
        //Wait for the interval, pick new, go again
        while(true){
            yield return new WaitForSeconds(newPaintingsInterval);
            PickNewPaintings();
        }
    }

    public void PickNewPaintings(){
        
        List<PaintingData> pickedPaintings = new List<PaintingData>();
        
        foreach(var frame in frames){
            //filter for only landscape if frame is landscape
            List<PaintingData> availablePaintings = paintings.Where(p => !pickedPaintings.Contains(p)).Where(p => p.isLandsape == frame.isLandsape()).ToList();
            //pick a random painting
            var pickedPainting = availablePaintings[UnityEngine.Random.Range(0, availablePaintings.Count)];
            pickedPaintings.Add(pickedPainting);
            frame.LoadPaintingData(pickedPainting);
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
                        frames.Add(spatialAnchor.GetComponent<PaintingObject>());
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