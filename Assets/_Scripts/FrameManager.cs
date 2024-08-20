using UnityEngine;
using OVR;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using Unity.VisualScripting;
//MRUK
using Meta.XR.MRUtilityKit;


public class FrameManager : MonoBehaviour {
      // This reusable buffer helps reduce pressure on the garbage collector
    List<OVRSpatialAnchor.UnboundAnchor> _unboundAnchors = new();

    public List<PaintingData> paintings = new();
    List<PaintingData> pickedPaintings = new();
    public List<PaintingObject> frames = new();
    private Dictionary<PaintingTag, int> tagsLikeCounter = new();

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

        //initialize the like counter
        foreach(PaintingTag tag in Enum.GetValues(typeof(PaintingTag))){
            tagsLikeCounter.Add(tag, 0);
        }
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            foreach(var frame in frames){
                frame.OnSaveButtonPressed();
            }
        }
        if(Input.GetKeyDown(KeyCode.L)){
            StartLoadingAnchors();
        }

        if(Input.GetKeyDown(KeyCode.R)){
            FindObjectsOfType<PaintingObject>().ToList().ForEach(p => p.OnEraseButtonPressed());
        }
    }
    public void StartLoadingAnchors(){
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
        PickPaintingFor(obj);
    }

    public IEnumerator PickNewPaintingsRoutine(){
        //Wait for the interval, pick new, go again
        while(true){
            yield return new WaitForSeconds(newPaintingsInterval);
            PickNewPaintings();
        }
    }

    public void PickNewPaintings(){
        Debug.Log("Picking new paintings");
        
        pickedPaintings = new List<PaintingData>();
        
        foreach(var frame in frames){
            if(frame == null){
                continue;
            }
            PickPaintingFor(frame);
        }
    }

    public void PickPaintingFor(PaintingObject frame){
        //filter for only landscape if frame is landscape
            List<PaintingData> availablePaintings = paintings.Where(p => !pickedPaintings.Contains(p)).Where(p => frame.frameData.paintingData != p).Where(p => p.isLandsape == frame.isLandsape()).ToList();
            //pick a random painting
            var pickedPainting = availablePaintings[UnityEngine.Random.Range(0, availablePaintings.Count)];
            frame.frameData.paintingData = pickedPainting;
            pickedPaintings.Add(pickedPainting);
            frame.LoadPaintingData(pickedPainting);
    }

    public void GiveLikes(PaintingTag[] tags, int value)
    {
        foreach (PaintingTag tag in tags)
        {
            tagsLikeCounter[tag] += value;
            Debug.Log("Giving likes to " + tag + " with value " + value + " is now " + tagsLikeCounter[tag]);
        }
    }
       






    //SPATIAL ANCHOR STUFF - ------
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
                        Frame.Instance.InitializeFrames();
                        Frame.Instance.UpdateFrames(frameData.startPosAtCreation, frameData.endPosAtCreation);
                        Frame.Instance.FinishFrame();
                        var frame = Frame.Instance.HaveFrameBePaintingObject(frameData.startPosAtCreation, frameData.endPosAtCreation, false);
                        frame.transform.SetParent(spatialAnchor.transform);
                        frame.transform.localPosition = frameData.offsetFromSpatialAnchor;
                        frames.Add(frame);
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