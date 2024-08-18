using UnityEngine;


public class Frame : MonoBehaviour {
    public GameObject framePartPrefab;
    public GameObject bgPrefab;



    private void Start() {
        //create a frame
        CreateFrame(new Vector3(0,0,0), new Vector3(1,1,1), new Vector3(-1,0,1));
    }
    public void CreateFrame(Vector3 startPos, Vector3 endPos, Vector3 surfaceNormal){
        //create 4 cubes for the frame


        //create the first cube
        GameObject framePart = Instantiate(framePartPrefab, startPos, Quaternion.identity);
        framePart.transform.position = new Vector3(startPos.x, (startPos.y + endPos.y ) / 2,startPos.z );
        framePart.transform.localScale = new Vector3(0.1f, Mathf.Abs(endPos.y - startPos.y), 0.1f);
        framePart.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

        //create the second cube
        framePart = Instantiate(framePartPrefab, startPos, Quaternion.identity);
        framePart.transform.position = new Vector3(endPos.x, (startPos.y + endPos.y ) / 2,endPos.z );
        framePart.transform.localScale = new Vector3(0.1f, Mathf.Abs(endPos.y - startPos.y), 0.1f);
        framePart.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

        //third and fourth cube should go along the top and bottom of the frame, length of distance between start and end along x and z axis
        framePart = Instantiate(framePartPrefab, startPos, Quaternion.identity);
        framePart.transform.position = new Vector3(Mathf.Abs(startPos.x + endPos.x) / 2, startPos.y, Mathf.Abs(startPos.z + endPos.z) / 2);
        framePart.transform.localScale = new Vector3(Mathf.Abs(endPos.x - startPos.x), 0.1f, 0.1f);
        framePart.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);

        //third and fourth cube should go along the top and bottom of the frame, length of distance between start and end along x and z axis
        framePart = Instantiate(framePartPrefab, startPos, Quaternion.identity);
        framePart.transform.position = new Vector3(Mathf.Abs(startPos.x + endPos.x) / 2, endPos.y, Mathf.Abs(startPos.z + endPos.z) / 2);
        framePart.transform.localScale = new Vector3(Mathf.Abs(endPos.x - startPos.x), 0.1f, 0.1f);
        framePart.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
    
        framePart = Instantiate(bgPrefab, startPos, Quaternion.identity);
        framePart.transform.rotation = Quaternion.LookRotation(surfaceNormal, Vector3.up);
        framePart.transform.position = new Vector3(Mathf.Abs(startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2, Mathf.Abs(startPos.z + endPos.z) / 2);
        framePart.transform.localScale = new Vector3(Mathf.Abs(endPos.x - startPos.x), Mathf.Abs(endPos.y - startPos.y), 0.02f);
    
    }   
}