using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PaintingUI : MonoBehaviour
{
    public PaintingObject frame;
    // Start is called before the first frame update

    public TextMeshProUGUI title;
    public TextMeshProUGUI author;
    public TextMeshProUGUI year;
    public TextMeshProUGUI description;
    Transform headset;

    public float disappearDistance = 1.8f;

    void Start()
    {
        headset = GameObject.Find("CenterEyeAnchor").transform;
        GetComponent<Canvas>().worldCamera = headset.GetComponent<Camera>();
        //find frame in parent recursively
        Transform parent = transform.parent;
        while (parent != null)
        {
            frame = parent.GetComponent<PaintingObject>();
            if (frame != null)
            {
                break;
            }
            parent = parent.parent;
        }
        frame.GetComponent<PaintingObject>().RegisterUI(this);
    }
    private void Update() {
        //only x and z
        Vector3 framePos = frame.transform.position;
        Vector3 headPos = headset.position;
        framePos.y = 0;
        headPos.y = 0;
        if(Vector3.Distance(framePos,headPos) > disappearDistance){
            GetComponent<Canvas>().enabled = false;
        }
        else{
            GetComponent<Canvas>().enabled = true;
        }
    }

 

    public void DeleteFrame(){
        frame.OnEraseButtonPressed();
    }

    public void SetNewPainting(PaintingData data){
        title.text = "<b> Title: </b>" +  data.title;
        author.text = "<b> Author: </b>" + data.artist;
        year.text = "<b> Year: </b>" + data.yearMade;
        description.text = "<b> Description: </b>" + data.description;

    }
}
