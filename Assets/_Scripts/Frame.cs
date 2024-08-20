using UnityEngine;
using Meta.XR.MRUtilityKit;
using OVR;
using System.Collections.Generic;
using DG.Tweening;
public class Frame : MonoBehaviour {
    public static Frame Instance; 
    
    public GameObject framePartPrefab;
    public GameObject bgPrefab;
    public GameObject uiPrefab;
    public float offset = 0.02f;
    public float minSize = 0.1f;
    public float maxRatioDifference = 2f;

    GameObject framePart1;
    GameObject framePart2;
    GameObject framePart3;
    GameObject framePart4;

    public GameObject bg;

    public OVRHand usingHand;

    public Vector3 startPos;
    public Transform indexTip;

    bool isPlacing = false;

    public GameObject objectToMove;
    public bool movingIsOn = false;



    public float timeSinceNotPinching = 0f;
    public float timeSincePinching = 0f;
    public float pinchGraceTime = 0.07f;
    public float startPlacingTime = 1f;

    public float middleFingerTime = 0f;

    private void Start() {
        Instance = this;
        //create a frame
        /*InitializeFrames();
        TestWithoutIndex(new Vector3(1,1,1), new Vector3(2,2,3));
        FinishFrame();*/
        //Find gameobject with name Hand_IndexTip in usingHand child
        indexTip = usingHand.transform.FindChildRecursive("Hand_IndexTip");
    }

    private void Update() {
        if(indexTip == null){
            //Debug.Log("no index yet");
            indexTip = usingHand.transform.FindChildRecursive("Hand_IndexTip");
            return;
        }

        
        if(movingIsOn){
            MovingInputs();
        }
        else{
            PlacementInputs();
        }
        if(usingHand.GetFingerIsPinching(OVRHand.HandFinger.Middle)){
            middleFingerTime += Time.deltaTime;
            if(middleFingerTime > 2f){
                Debug.Log("Deleting everything!");
                PlayerPrefs.DeleteAll();

            }
        }
        else{
            middleFingerTime = 0f;
        }
        
    }
    public void SetObjectToMove(GameObject obj){
        objectToMove = obj;
        movingIsOn = true;
    }
    public void DisableObjectToMove(){
        objectToMove = null;
        movingIsOn = false;
    }

    private void MovingInputs(){
        if(usingHand.GetFingerIsPinching(OVRHand.HandFinger.Index)){
            objectToMove.transform.position = ClosestWallPos(indexTip.position);
            bg.GetComponent<PaintingObject>().frameData.offsetFromSpatialAnchor = bg.transform.localPosition;
        }
    }
    private void PlacementInputs(){
        
        if(usingHand.GetFingerIsPinching(OVRHand.HandFinger.Index)){
            timeSincePinching += Time.deltaTime;
            timeSinceNotPinching = 0f;
            if(timeSincePinching < startPlacingTime){ // we stop here
                return;
            }
            if(!isPlacing){//start placing
                startPos = ClosestWallPos(indexTip.position);
                InitializeFrames();
            }//update the frame
            isPlacing = true;
            UpdateFrames(startPos, ClosestWallPos(indexTip.position));
            
        }
        else{
            if(isPlacing ){//Account for very short breaks in pinching for better placement
                if(timeSinceNotPinching > pinchGraceTime){
                    timeSincePinching = 0f;
                    //end placing
                    FinishFrame();
                    HaveFrameBePaintingObject(startPos, ClosestWallPos(indexTip.position));
                }
                else{
                    timeSinceNotPinching += Time.deltaTime;
                }
            }
        }
    }
    

    public Vector3 ClosestWallPos(Vector3 pos){
        
        LabelFilter filter = LabelFilter.Included(MRUKAnchor.SceneLabels.WALL_FACE);
  
        MRUK.Instance.GetCurrentRoom().TryGetClosestSurfacePosition(pos, out Vector3 output,out MRUKAnchor anchor, filter);
        return output;
        
    }

    
    public void FinishFrame(){
        
        framePart1.transform.SetParent(bg.transform);
        framePart2.transform.SetParent(bg.transform);
        framePart3.transform.SetParent(bg.transform);
        framePart4.transform.SetParent(bg.transform);
        isPlacing = false;
        if(bg.transform.localScale.x + bg.transform.localScale.y < minSize){
            Debug.Log("Too small, destroying painting");
            Destroy(bg.gameObject);
            return;
        }//else if the ratio between x scale and y scale is bigger than 
        else if(bg.transform.localScale.x / bg.transform.localScale.y > maxRatioDifference || bg.transform.localScale.y / bg.transform.localScale.x > maxRatioDifference){
            Debug.Log("ratio too big, destroying painting");
            Destroy(bg.gameObject);
            return;
        }
        
        
    }
    public PaintingObject HaveFrameBePaintingObject(Vector3 startPos, Vector3 endPos, bool needsAnchor = true){
        var obj = bg.AddComponent<PaintingObject>();
        obj.Initialize();
        obj.SetStartAndEndPointWhenFinishingCreation(startPos, endPos);
        if(needsAnchor){
            StartCoroutine(obj.CreateSpatialAnchor());
        }
        InitPaintingUI( obj);
        bg = obj.gameObject;
        var scale = bg.transform.localScale;
        bg.transform.localScale = new Vector3(0, 0, 0);
        bg.transform.DOScale(scale, 1f).SetEase(Ease.OutBack);
        

        return obj;
    }

    public void InitPaintingUI(PaintingObject obj){
        //init the ui , then change its position 
        Transform uispot = obj.transform.FindChildRecursive("UISpot");
        var ui = Instantiate(uiPrefab, uispot.position, uispot.rotation);
        ui.transform.parent = uispot;
       // ui.transform.localPosition = new Vector3(0,0,0);
        //TODO populate data
        ui.GetComponentInChildren<PaintingUI>().SetNewPainting(obj.frameData.paintingData);

    }

   

    public Vector3 GetClosestMRUK(Vector3 point){
        
        MRUK.Instance.GetCurrentRoom().TryGetClosestSurfacePosition(point, out Vector3 closest, out MRUKAnchor closestAnchor);
        return closest;
    }


    public void InitializeFrames(){
       framePart1 = Instantiate(framePartPrefab, new Vector3(0,0,0), Quaternion.identity);
         framePart2 = Instantiate(framePartPrefab, new Vector3(0,0,0), Quaternion.identity);
         framePart3 = Instantiate(framePartPrefab, new Vector3(0,0,0), Quaternion.identity);
         framePart4 = Instantiate(framePartPrefab, new Vector3(0,0,0), Quaternion.identity);
         bg = Instantiate(bgPrefab, new Vector3(0,0,0), Quaternion.identity);
         
  
    }
    public void UpdateFrames(Vector3 startPos, Vector3 endPos){
        Vector3 surfaceNormal = Vector3.Cross(endPos - startPos, Vector3.up).normalized;
        //check if the surface normal is pointing towards the user more then away from the user
        bool pointingInHandDiretion = Vector3.Dot(surfaceNormal, (new Vector3(0,0,0) - startPos).normalized) < 0;
        if(!pointingInHandDiretion){
            surfaceNormal = -surfaceNormal;
        }
        //create 4 cubes for the frame

        //create the first cube
        
        framePart1.transform.position = new Vector3(startPos.x, (startPos.y + endPos.y ) / 2,startPos.z );
        framePart1.transform.localScale = new Vector3(offset, Mathf.Abs(endPos.y - startPos.y) - offset, offset);
        framePart1.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

        //create the second cube
        framePart2.transform.position = new Vector3(endPos.x, (startPos.y + endPos.y ) / 2,endPos.z );
        framePart2.transform.localScale = new Vector3(offset, Mathf.Abs(endPos.y - startPos.y) - offset, offset);
        framePart2.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

        //third and fourth cube should go along the top and bottom of the frame, le
        framePart3.transform.position = new Vector3((startPos.x + endPos.x) / 2, startPos.y, (startPos.z + endPos.z) / 2); // Positioned between frame 1 and 2 on X-axis, at the start Y position
        framePart3.transform.localScale = new Vector3(offset, offset + Mathf.Sqrt(Mathf.Abs(startPos.x - endPos.x) * Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.z - endPos.z) * Mathf.Abs(startPos.z - endPos.z)), offset); // Scaling along the Y-axis (now Y component)
        framePart3.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.right); // Rotating by 90 degrees around Z-axis

        // Frame Part 4
        framePart4.transform.position = new Vector3((startPos.x + endPos.x) / 2, endPos.y, (startPos.z + endPos.z) / 2); // Positioned between frame 1 and 2 on X-axis, at the end Y position
        framePart4.transform.localScale = new Vector3(offset, offset + Mathf.Sqrt(Mathf.Abs(startPos.x - endPos.x) * Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.z - endPos.z) * Mathf.Abs(startPos.z - endPos.z)), offset); // Scaling along the Y-axis (now Y component)
        framePart4.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.right); // Rotating by 90 degrees around Z-axis
    
        bg.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
        bg.transform.position = new Vector3((startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2, (startPos.z + endPos.z) / 2);
        bg.transform.localScale = new Vector3(Mathf.Sqrt(Mathf.Abs(startPos.x - endPos.x) * Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.z - endPos.z) * Mathf.Abs(startPos.z - endPos.z)) - offset/2, Mathf.Abs(endPos.y - startPos.y) - offset/2, 0.01f);
        
    }
}