using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class PreferencesUpdatedUI : MonoBehaviour
{
    Transform centerEye;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        //start fading out the text over 4 seconds
        text = GetComponent<TextMeshProUGUI>();
        Destroy(gameObject, 2f);
        centerEye = GameObject.Find("CenterEyeAnchor").transform;
        transform.LookAt(centerEye, Vector3.up);
    }

 
}
