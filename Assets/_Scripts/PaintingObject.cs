using UnityEngine;
using OVR;
using System.Collections.Generic;
using System.Collections;
using System;
using DG.Tweening;


public class PaintingObject : MonoBehaviour
{
    public SpriteRenderer paintingRenderer;
    public FrameData frameData;

    //-2: 2 dislikes, -1: 1 dislike, 1: 1 like, 2: 2 likes
    private int prevLikeValue = 0;
    private PaintingUI paintingUI;
     public void RegisterUI(PaintingUI ui){
        paintingUI = ui;
    }
    
    public void Initialize( ){
        frameData = new FrameData();
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

        FrameManager.Instance.RegisterNewFrame(this);


    }

    public void SetStartAndEndPointWhenFinishingCreation(Vector3 startpos, Vector3 endPos){
        frameData.startPosAtCreation = startpos;
        frameData.endPosAtCreation = endPos;
    }

    public void LoadPaintingData(PaintingData data)
    {
        prevLikeValue = 0;
        var rend = GetComponentInChildren<SpriteRenderer>();
        float initialdelay = 0.8f;
        if(rend.sprite == null){
            initialdelay = 0;
        }
        //DO fade to transparent, change sprite, fade back to visible
        rend.DOColor(new Color(1,1,1,0), initialdelay).OnComplete(() => {
            frameData.paintingData = data;
            rend.sprite = data.paintingSprite;
            rend.DOColor(new Color(1,1,1,1), 0.8f);
            paintingUI.SetNewPainting(data);
        });
    }
    public bool isLandsape(){
        return transform.localScale.x > transform.localScale.y;
    }

    public bool CanGiveLike(int likeValue)
    {
        return likeValue != prevLikeValue;
    }

    public void GiveLike(int likeValue){
        //nothing changed, should've been checked before
        if(likeValue == prevLikeValue){
            Debug.LogWarning("Trying to give like that was already given for value " + likeValue);
            return;
        }
        if(prevLikeValue != 0){
            Debug.Log("Removing previous like value " + prevLikeValue);
        }
        //reset previous like value
        int likeAdjusted = -prevLikeValue + likeValue;
        FrameManager.Instance.GiveLikes(frameData.paintingData.tags, likeAdjusted);
        prevLikeValue = likeValue;
    }















    //---------------Spatial Anchor Stuff -----------
    public IEnumerator CreateSpatialAnchor()
    {   
        //Instantiate an anchor
        var anchor = new GameObject("Anchor");
        anchor.transform.position = transform.position;
        anchor.AddComponent<OVRSpatialAnchor>();
        transform.parent = anchor.transform;


        // Wait for the async creation
        yield return new WaitUntil(() => anchor.GetComponent<OVRSpatialAnchor>().Created);
        OnSaveButtonPressed();

    }

    public async void OnSaveButtonPressed()
    {
        OVRSpatialAnchor anchor = transform.parent.GetComponent<OVRSpatialAnchor>();
        var result = await anchor.SaveAnchorAsync();
        if (result.Success)
        {
            Debug.Log($"Anchor {anchor.Uuid} saved successfully.");
            //save the paintingdata with the uuid of the anchor to the playerprefs
            PlayerPrefs.SetString(anchor.Uuid.ToString(), JsonUtility.ToJson(frameData));
            //if the uuid is not in the playerprefs yet
            //save the uuid to the playerprefs
            if (!PlayerPrefs.GetString("SavedAnchors", "").Contains(anchor.Uuid.ToString()))
                PlayerPrefs.SetString("SavedAnchors", PlayerPrefs.GetString("SavedAnchors", "") + anchor.Uuid + ",");
        }
        else
        {
            Debug.LogError($"Anchor {anchor.Uuid} failed to save with error {result.Status}");
        }
    }

    public async void OnEraseButtonPressed()
    {
        Debug.Log("Erasing anchor");
        FrameManager.Instance.frames.Remove(this);
        //disable all children adn this renderer
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        GetComponent<MeshRenderer>().enabled = false;
        PlayerPrefs.DeleteKey(transform.parent.GetComponent<OVRSpatialAnchor>().Uuid.ToString());
        PlayerPrefs.SetString("SavedAnchors", PlayerPrefs.GetString("SavedAnchors", "").Replace(transform.parent.GetComponent<OVRSpatialAnchor>().Uuid.ToString() + ",", ""));
        var result = await transform.parent.GetComponent<OVRSpatialAnchor>().EraseAnchorAsync();
        if (result.Success)
        {
            Debug.Log($"Successfully erased anchor.");
            Destroy(gameObject);
        }
    }

  
}