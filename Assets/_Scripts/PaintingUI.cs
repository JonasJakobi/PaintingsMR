using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class PaintingUI : MonoBehaviour
{
    public PaintingObject frame;
    // Start is called before the first frame update

    public TextMeshProUGUI title;
    public TextMeshProUGUI author;
    public TextMeshProUGUI year;
    public TextMeshProUGUI description;

    public Toggle toggle;
    public Image toggleSymbol;

     
    Transform headset;

    public float disappearDistance = 1.8f;
    public float charShowSpeed = 0.01f;

    void Start()
    {
        headset = GameObject.Find("CenterEyeAnchor").transform;
        GetComponent<Canvas>().worldCamera = headset.GetComponent<Camera>();
        frame = GetComponentInParent<PaintingObject>();
        frame.RegisterUI(this);
        //find frame in parent recursively
        /*Transform parent = transform.parent;
        while (parent != null)
        {
            frame = parent.GetComponent<PaintingObject>();
            if (frame != null)
            {
                break;
            }
            parent = parent.parent;
        }
        frame.GetComponent<PaintingObject>().RegisterUI(this);*/
    }
    private void Update() {
        //only x and z
        Vector3 framePos = frame.transform.position;
        Vector3 headPos = headset.position;
        framePos.y = 0;
        headPos.y = 0;
        if(Vector3.Distance(framePos,headPos) > disappearDistance){
            GetComponent<Canvas>().enabled = false;
            if(toggle.isOn){
                TurnOffMovePainting();
            }
            
        }
        else{
            GetComponent<Canvas>().enabled = true;
        }
    }

 

    public void DeleteFrame(){
        frame.OnEraseButtonPressed();
    }

    public void SetNewPainting(PaintingData data){
        if(GetComponent<Canvas>().enabled == false) {
            return;
        }
        title.text = "<b>Title: </b>" +  data.title;
        author.text = "<b>Author: </b>" + data.artist;
        if(data.yearMade == 0 || data.yearMade == -1 || data.yearMade == 9999){
            year.text = "<b>Year: </b> <i> Unknown </i>";
        }
        else{
            year.text = "<b>Year: </b>" + data.yearMade;
        }
        
        if(data.description == ""){
            description.text = "<b>Description: </b> <i> No description available </i>";
        }
        else{
            description.text = "<b>Description: </b>" + data.description;
        }
        StopAllCoroutines();
        StartCoroutine(ShowText(title));
        StartCoroutine(ShowText(author));
        StartCoroutine(ShowText(year));
        StartCoroutine(ShowText(description));
    }

    private IEnumerator ShowText(TextMeshProUGUI text){
        text.maxVisibleCharacters = 0;
        int totalChars = text.text.Length;
        for(int i = 0; i < totalChars; i++){
            text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(charShowSpeed);
        }
    }


    public void ToggleMovePainting(bool isOn){
        isOn = toggle.isOn;

        if(isOn){
            Frame.Instance.SetObjectToMove(frame.gameObject);
            toggleSymbol.color = Color.green;
        }
        else{
            Frame.Instance.DisableObjectToMove();
            toggleSymbol.color = Color.white;
        }
    }
    public void TurnOffMovePainting(){
        toggleSymbol.color = Color.white;
        Frame.Instance.DisableObjectToMove();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
