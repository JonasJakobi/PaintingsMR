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

    void Start()
    {
        GetComponent<Canvas>().worldCamera = GameObject.Find("CenterEyeAnchor").GetComponent<Camera>();
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
