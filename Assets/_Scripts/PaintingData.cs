using UnityEngine;

[CreateAssetMenu(fileName = "New Painting Data", menuName = "Paintings/Painting Data")]
public class PaintingData : ScriptableObject
{
    public Sprite paintingSprite;
    public string artist;
    public string description;
    public int yearMade;
    //List of tags
    public PaintingTag[] tags;
    
}