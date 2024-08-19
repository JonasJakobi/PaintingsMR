using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "New Painting Data", menuName = "Paintings/Painting Data")]
public class PaintingData : ScriptableObject
{
    public Sprite paintingSprite;
    public string artist;
    public string title;
    public string description;
    public int yearMade;
    //List of tags
    public PaintingTag[] tags;

    public bool isLandsape;

    //SMall comments
    
}
[System.Serializable]
public class FrameData 
{
    public Vector3 startPosAtCreation;
    public Vector3 endPosAtCreation;

    public Vector3 offsetFromSpatialAnchor;
    public PaintingData paintingData;
    
}
