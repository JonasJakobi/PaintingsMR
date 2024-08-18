using UnityEngine;
using OVR;
using System.Collections.Generic;
using System.Collections;
using System;


public class PaintingObject : MonoBehaviour
{
    SpriteRenderer paintingRenderer;
    public PaintingData paintingData;

    private void Start() {
        StartCoroutine(CreateSpatialAnchor());


    }

    public void LoadPaintingData(PaintingData data)
    {
        paintingData = data;
        GetComponentInChildren<SpriteRenderer>().sprite = data.paintingSprite;
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
            PlayerPrefs.SetString(anchor.Uuid.ToString(), JsonUtility.ToJson(paintingData));
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