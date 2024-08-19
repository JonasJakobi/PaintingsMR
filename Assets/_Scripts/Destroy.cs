using UnityEngine;

public class Destroy : MonoBehaviour
{
    /// <summary>
    /// Called from animator
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
