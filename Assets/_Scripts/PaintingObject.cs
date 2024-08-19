using UnityEngine;
using OVR;
using System.Collections.Generic;
using System.Collections;
using System;


public class PaintingObject : MonoBehaviour
{
    public SpriteRenderer paintingRenderer;
    public FrameData frameData;

    
    private void Start() {
        
        StartCoroutine(CreateSpatialAnchor());
        //Instantiate a blank gameobject

        paintingRenderer =new GameObject("Painting Renderer").AddComponent<SpriteRenderer>();
        paintingRenderer.transform.position = this.transform.position;
        paintingRenderer.transform.rotation = this.transform.rotation;
        

        if(isLandsape()){
            paintingRenderer.transform.localScale = new Vector3(transform.localScale.y,transform.localScale.y,1);
        }
        else{
            paintingRenderer.transform.localScale = new Vector3(transform.localScale.x,transform.localScale.x,1);

        }
        paintingRenderer.transform.parent = this.transform;
        paintingRenderer.transform.localPosition = new Vector3(paintingRenderer.transform.localPosition.x,paintingRenderer.transform.localPosition.y,paintingRenderer.transform.localPosition.z-1f);



    }
    public void Initialize( ){
        frameData = new FrameData();
        frameData.scale = transform.localScale;
    }

    public void LoadPaintingData(PaintingData data)
    {
        frameData.paintingData = data;
        GetComponentInChildren<SpriteRenderer>().sprite = data.paintingSprite;
    }
    public bool isLandsape(){
        return transform.localScale.x > transform.localScale.y;
    }

    private IEnumerator CreateSpatialAnchor()
    {
        var anchor = gameObject.AddComponent<OVRSpatialAnchor>();

        // Wait for the async creation
        yield return new WaitUntil(() => anchor.Created);

        Debug.Log($"Created anchor {anchor.Uuid}");
        OnSaveButtonPressed(anchor);
    }

    public async void OnSaveButtonPressed(OVRSpatialAnchor anchor)
    {
        var result = await anchor.SaveAnchorAsync();
        if (result.Success)
        {
            Debug.Log($"Anchor {anchor.Uuid} saved successfully.");
            //save the paintingdata with the uuid of the anchor to the playerprefs
            PlayerPrefs.SetString(anchor.Uuid.ToString(), JsonUtility.ToJson(frameData.paintingData));
            //save the uuid to the playerprefs
            PlayerPrefs.SetString("SavedAnchors", PlayerPrefs.GetString("SavedAnchors", "") + anchor.Uuid + ",");
        }
        else
        {
            Debug.LogError($"Anchor {anchor.Uuid} failed to save with error {result.Status}");
        }
    }

    public async void OnEraseButtonPressed()
    {
        PlayerPrefs.DeleteKey(GetComponent<OVRSpatialAnchor>().Uuid.ToString());
        PlayerPrefs.SetString("SavedAnchors", PlayerPrefs.GetString("SavedAnchors", "").Replace(GetComponent<OVRSpatialAnchor>().Uuid.ToString() + ",", ""));
        var result = await GetComponent<OVRSpatialAnchor>().EraseAnchorAsync();
        if (result.Success)
        {
            Debug.Log($"Successfully erased anchor.");
            Destroy(gameObject);
        }
    }

  
}