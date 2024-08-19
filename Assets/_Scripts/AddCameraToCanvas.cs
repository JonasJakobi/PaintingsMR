using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCameraToCanvas : MonoBehaviour
{
    public PaintingObject frame;
    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteFrame(){
        frame.OnEraseButtonPressed();
    }
}
