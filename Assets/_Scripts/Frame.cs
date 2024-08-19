using UnityEngine;
using Meta.XR.MRUtilityKit;
using OVR;

public class Frame : MonoBehaviour {

    
    public GameObject framePartPrefab;
    public GameObject bgPrefab;
    public float offset = 0.02f;
    public float minSize = 0.1f;
    public float maxRatioDifference = 2f;

    GameObject framePart1;
    GameObject framePart2;
    GameObject framePart3;
    GameObject framePart4;

    GameObject bg;

    public OVRHand usingHand;

    public Vector3 startPos;
    public Transform indexTip;

    bool isPlacing = false;



    public float timeSinceNotPinching = 0f;
    public float pinchGraceTime = 0.1f;
    private void Start() {
        //create a frame
        //InitializeFrames();
        //UpdateFrames(new Vector3(0,0,0), new Vector3(2,5,5));
        //FinishFrame();
        //Find gameobject with name Hand_IndexTip in usingHand child
        indexTip = usingHand.transform.FindChildRecursive("Hand_IndexTip");
    }

    private void Update() {
        if(indexTip == null){
            Debug.Log("no index yet");
            indexTip = usingHand.transform.FindChildRecursive("Hand_IndexTip");
            return;
        }
        if(usingHand.GetFingerIsPinching(OVRHand.HandFinger.Index)){
            timeSinceNotPinching = 0f;
            if(!isPlacing){//start placing
                startPos = indexTip.position;
                InitializeFrames();
            }//update the frame
            isPlacing = true;
            UpdateFrames(startPos, indexTip.position);
        }
        else{
            if(isPlacing ){//Account for very short breaks in pinching for better placement
                if(timeSinceNotPinching > pinchGraceTime){
                    //end placing
                    FinishFrame();
                }
                else{
                    timeSinceNotPinching += Time.deltaTime;
                }
                


            }
        }
    }

    
    public void FinishFrame(){
        
        framePart1.transform.parent = bg.transform;
        framePart2.transform.parent = bg.transform;
        framePart3.transform.parent = bg.transform;
        framePart4.transform.parent = bg.transform;
        isPlacing = false;
        if(bg.transform.localScale.x + bg.transform.localScale.y < minSize){
            Destroy(bg.gameObject);
        }//else if the ratio between x scale and y scale is bigger than 
        else if(bg.transform.localScale.x / bg.transform.localScale.y > maxRatioDifference || bg.transform.localScale.y / bg.transform.localScale.x > maxRatioDifference){
            Destroy(bg.gameObject);
        }
        var obj = bg.AddComponent<PaintingObject>();
        obj.Initialize();
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
        //create 4 cubes for the frame

        //create the first cube
        
        framePart1.transform.position = new Vector3(startPos.x, (startPos.y + endPos.y ) / 2,startPos.z );
        framePart1.transform.localScale = new Vector3(offset, Mathf.Abs(endPos.y - startPos.y), offset);
        framePart1.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

        //create the second cube
        framePart2.transform.position = new Vector3(endPos.x, (startPos.y + endPos.y ) / 2,endPos.z );
        framePart2.transform.localScale = new Vector3(offset, Mathf.Abs(endPos.y - startPos.y), offset);
        framePart2.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

        //third and fourth cube should go along the top and bottom of the frame, le
        framePart3.transform.position = new Vector3((startPos.x + endPos.x) / 2, startPos.y, (startPos.z + endPos.z) / 2);
        framePart3.transform.localScale = new Vector3(offset + Mathf.Sqrt(Mathf.Abs(startPos.x - endPos.x) * Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.z - endPos.z) * Mathf.Abs(startPos.z - endPos.z)), offset, offset);
        framePart3.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
//
        //third and fourth cube should go along the top and bottom of the frame, length of distance between start and end along x and z axis
          framePart4.transform.position = new Vector3((startPos.x + endPos.x) / 2, endPos.y, (startPos.z + endPos.z) / 2);
        framePart4.transform.localScale = new Vector3(offset + Mathf.Sqrt(Mathf.Abs(startPos.x - endPos.x) * Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.z - endPos.z) * Mathf.Abs(startPos.z - endPos.z)), offset, offset);
        //        framePart.transform.localScale = new Vector3(Mathf.Abs(endPos.x - startPos.x), offset, offset);

        framePart4.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
    
        bg.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
        bg.transform.position = new Vector3((startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2, (startPos.z + endPos.z) / 2);
        bg.transform.localScale = new Vector3(offset / 2 + Mathf.Sqrt(Mathf.Abs(startPos.x - endPos.x) * Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.z - endPos.z) * Mathf.Abs(startPos.z - endPos.z)), Mathf.Abs(endPos.y - startPos.y) - 0.01f, 0.01f);
        
    }   


    
}