using UnityEngine;

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

public class FrameData 
{
    public Vector3 scale;
    public PaintingData paintingData;
    
}
